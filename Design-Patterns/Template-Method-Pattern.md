# Template Method Pattern

## 1. Khái niệm cơ bản

**Template Method** là một Behavioral Design Pattern, định nghĩa **bộ khung (skeleton) của một thuật toán** trong một method ở lớp cha (`AbstractClass`) — trong đó một số bước cố định được hiện thực sẵn, còn một số bước được để trống (`abstract`/`virtual`) cho lớp con override, mà **không làm thay đổi thứ tự/cấu trúc tổng thể** của thuật toán.

Pattern gồm 2 thành phần:

- **AbstractClass:** định nghĩa `TemplateMethod()` chứa đúng thứ tự các bước cố định của thuật toán; các bước cần tùy biến được khai báo dưới dạng `abstract` method, hoặc `virtual` method có sẵn implementation mặc định (gọi là **hook**) mà lớp con không bắt buộc phải override.
- **ConcreteClass:** kế thừa `AbstractClass`, chỉ override đúng những bước cần tùy biến, không đụng vào `TemplateMethod()`.

## 2. Khi nào nên dùng

Dùng khi:

✅ Nhiều class có chung một quy trình xử lý tổng thể, chỉ khác nhau ở vài bước cụ thể

✅ Muốn định nghĩa thứ tự các bước cố định một lần, tránh lặp lại ở nhiều class con

✅ Muốn kiểm soát chặt cấu trúc thuật toán, chỉ cho phép lớp con tùy biến đúng phần được phép (qua hook)

✅ Muốn tuân thủ Open/Closed Principle (thêm biến thể mới không sửa bộ khung thuật toán)

## 3. Code examples

### Ví dụ 1 — Quy trình tạo báo cáo (CSV / JSON)

**Bài toán:** Nhiều loại báo cáo (CSV, JSON...) đều phải trải qua đúng 3 bước theo thứ tự cố định: đọc dữ liệu, xử lý dữ liệu, rồi xuất kết quả — chỉ khác nhau ở cách đọc/xử lý/xuất của từng định dạng. Nếu để mỗi loại report tự viết lại toàn bộ quy trình từ đầu, thứ tự các bước có thể bị viết sai hoặc thiếu bước ở một vài class, và khi cần sửa/thêm một bước dùng chung phải sửa ở tất cả các nơi.

**Ý nghĩa của Template Method trong ví dụ này:** `ReportGenerator.Generate()` là Template Method, cố định đúng thứ tự `ReadData()` → `ProcessData()` → `Export()` và không cho lớp con thay đổi thứ tự này. `CsvReportGenerator`/`JsonReportGenerator` chỉ cần override 3 method abstract, không cần biết hay lặp lại logic điều phối thứ tự các bước.

**Cách implementation (C#):**

```csharp
// AbstractClass
public abstract class Report
{
    // Template Method - dinh nghia thu tu cac buoc, khong cho subclass sua
    public void Generate()
    {
        var rawData = ReadData();
        var processedData = ProcessData(rawData);
        Export(processedData);
    }

    protected abstract string ReadData();
    protected abstract string ProcessData(string rawData);
    protected abstract void Export(string data);
}

// ConcreteClass
public class CsvReport : Report
{
    protected override string ReadData() => "id,name\n1,Product A";

    protected override string ProcessData(string rawData) => rawData.Replace(",", " | ");

    protected override void Export(string data) => Console.WriteLine($"[CSV] {data}");
}

public class JsonReport : Report
{
    protected override string ReadData() => "{\"id\":1,\"name\":\"Product A\"}";

    protected override string ProcessData(string rawData) => rawData;

    protected override void Export(string data) => Console.WriteLine($"[JSON] {data}");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        Report csv = new CsvReport();
        csv.Generate();

        Report json = new JsonReport();
        json.Generate();
    }
}
// [CSV] id | name
// 1 | Product A
// [JSON] {"id":1,"name":"Product A"}
```

### Ví dụ 2 — Xử lý thanh toán với bước tùy chọn (hook)

**Bài toán:** Nhiều phương thức thanh toán (thẻ tín dụng, ví điện tử...) đều phải qua đúng quy trình validate → charge → (tùy chọn) xử lý sau thanh toán, nhưng bước "xử lý sau thanh toán" không phải phương thức nào cũng cần tùy biến. Nếu bắt buộc mọi `ConcreteClass` đều phải override đủ mọi bước kể cả bước không cần dùng, code sẽ có nhiều override rỗng không cần thiết.

**Ý nghĩa của Template Method trong ví dụ này:** `PaymentProcessor.Process()` là Template Method cố định quy trình `ValidateAmount()` → `Charge()` → `AfterPayment()`. `Charge()` là `abstract`, bắt buộc override; còn `AfterPayment()` là **hook** (`virtual`, có sẵn implementation rỗng) — `CreditCardPaymentProcessor` không cần override hook này, trong khi `EWalletPaymentProcessor` override để thêm hành vi riêng mà không phá vỡ quy trình chung.

**Cách implementation (C#):**

```csharp
// AbstractClass
public abstract class Payment
{
    // Template Method
    public void Process(decimal amount)
    {
        if (!ValidateAmount(amount))
        {
            Console.WriteLine("[Payment] So tien khong hop le");
            return;
        }

        Charge(amount);
        AfterPayment(amount);
    }

    protected virtual bool ValidateAmount(decimal amount) => amount > 0;

    protected abstract void Charge(decimal amount);

    // Hook - co san implementation mac dinh, subclass khong bat buoc override
    protected virtual void AfterPayment(decimal amount)
    {
    }
}

// ConcreteClass
public class CreditCardPayment : Payment
{
    protected override void Charge(decimal amount) => Console.WriteLine($"[CreditCard] Da tru {amount:N0} tu the");
}

public class EWalletPayment : Payment
{
    protected override void Charge(decimal amount) => Console.WriteLine($"[EWallet] Da tru {amount:N0} tu vi dien tu");

    // Override hook de gui thong bao rieng - khong bat buoc nhung co the tuy chinh
    protected override void AfterPayment(decimal amount) => Console.WriteLine("[EWallet] Da gui thong bao vao app");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        PaymentProcessor creditCard = new CreditCardPaymentProcessor();
        creditCard.Process(200000);

        PaymentProcessor eWallet = new EWalletPaymentProcessor();
        eWallet.Process(150000);

        creditCard.Process(-1000);
    }
}
// [CreditCard] Da tru 200,000 tu the
// [EWallet] Da tru 150,000 tu vi dien tu
// [EWallet] Da gui thong bao vao app
// [Payment] So tien khong hop le
```

### Ví dụ 3 — Duyệt đơn xin nghỉ phép (nghỉ phép thường / nghỉ ốm)

**Bài toán:** Quy trình duyệt đơn nghỉ phép luôn theo đúng 2 bước: kiểm tra điều kiện, rồi tính số ngày nghỉ được duyệt — nhưng điều kiện và cách tính ngày nghỉ khác nhau giữa nghỉ phép thường và nghỉ ốm. Nếu viết if/switch theo loại đơn ngay trong một method duyệt đơn duy nhất, method đó ngày càng phình to khi công ty bổ sung thêm loại nghỉ phép mới.

**Ý nghĩa của Template Method trong ví dụ này:** `LeaveRequestProcessor.Process()` là Template Method định nghĩa đúng thứ tự `CheckEligibility()` → `CalculateDays()` → thông báo kết quả, cố định cho mọi loại đơn. `NormalLeaveProcessor`/`SickLeaveProcessor` chỉ override `CheckEligibility()`/`CalculateDays()` theo đúng quy tắc riêng của loại nghỉ phép đó, không cần biết hay lặp lại bước thông báo kết quả dùng chung.

**Cách implementation (C#):**

```csharp
// AbstractClass
public abstract class LeaveRequest
{
    // Template Method
    public void Process(string employeeName, int requestedDays)
    {
        if (!CheckEligibility(requestedDays))
        {
            Console.WriteLine($"[{employeeName}] Don bi tu choi: khong du dieu kien");
            return;
        }

        int approvedDays = CalculateDays(requestedDays);
        Console.WriteLine($"[{employeeName}] Duoc duyet {approvedDays} ngay nghi");
    }

    protected abstract bool CheckEligibility(int requestedDays);
    protected abstract int CalculateDays(int requestedDays);
}

// ConcreteClass
public class NormalLeave : LeaveRequest
{
    protected override bool CheckEligibility(int requestedDays) => requestedDays <= 12;

    protected override int CalculateDays(int requestedDays) => requestedDays;
}

public class SickLeave : LeaveRequest
{
    protected override bool CheckEligibility(int requestedDays) => requestedDays <= 30;

    // Nghi om duoc tinh giam 50% vao quy phep nam
    protected override int CalculateDays(int requestedDays) => (int)Math.Ceiling(requestedDays * 0.5);
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        LeaveRequest normal = new NormalLeave();
        normal.Process("Nam", 3);
        normal.Process("Nam", 15);

        LeaveRequest sick = new SickLeave();
        sick.Process("Lan", 4);
    }
}
// [Nam] Duoc duyet 3 ngay nghi
// [Nam] Don bi tu choi: khong du dieu kien
// [Lan] Duoc duyet 2 ngay nghi
```
