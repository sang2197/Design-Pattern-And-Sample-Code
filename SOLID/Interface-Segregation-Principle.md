# Interface Segregation Principle (ISP)

## 1. Khái niệm cơ bản

**Interface Segregation Principle** phát biểu: **không nên buộc Client phải phụ thuộc vào những method mà nó không dùng tới.** Thay vì gom quá nhiều method không liên quan vào một interface lớn ("fat interface"), nên tách thành nhiều interface nhỏ, mỗi interface phục vụ đúng một nhóm nhu cầu — class chỉ cần hiện thực đúng những interface phù hợp với khả năng thực tế của nó.

Dấu hiệu vi phạm dễ nhận thấy nhất: một class phải override một method của interface bằng cách để trống hoặc ném `NotSupportedException`, vì bản thân class đó không thực sự hỗ trợ hành vi đó.

## 2. Khi nào nên dùng

Dùng khi:

✅ Một interface đang gom quá nhiều method phục vụ nhiều nhóm Client khác nhau

✅ Một class phải hiện thực method mà nó không dùng tới, chỉ để throw exception hoặc để trống

✅ Client chỉ cần dùng 1-2 method nhưng buộc phải biết/phụ thuộc vào toàn bộ interface lớn

✅ Muốn thêm khả năng mới cho một nhóm class mà không ảnh hưởng tới các class khác đang dùng interface gốc

## 3. Code examples

### Ví dụ 1 — Máy in: tách In / Quét / Fax thành các interface riêng

**Bài toán:** Interface `IMultiFunctionPrinter` gộp chung `Print()`, `Scan()`, `Fax()`. Một máy in đời cũ (`OldPrinter`) chỉ hỗ trợ in, nhưng vẫn buộc phải hiện thực đủ `IMultiFunctionPrinter` để dùng chung ở những nơi cần in ấn, nên phải override `Scan()`/`Fax()` bằng cách ném `NotSupportedException` — vừa thừa code, vừa tiềm ẩn lỗi runtime nếu ai đó vô tình gọi nhầm.

**Ý nghĩa của ISP trong ví dụ này:** Tách `IMultiFunctionPrinter` thành 3 interface nhỏ `IPrinter`, `IScanner`, `IFax`. `OldPrinter` chỉ hiện thực `IPrinter` — đúng với khả năng thực tế của nó; `ModernPrinter` (máy in đa chức năng thật) hiện thực cả 3. Client cần in tài liệu chỉ khai báo phụ thuộc vào `IPrinter`, không cần biết tới `IScanner`/`IFax`.

**Cách implementation (C#):**

```csharp
public interface IPrinter
{
    void Print(string document);
}

public interface IScanner
{
    void Scan(string document);
}

public interface IFax
{
    void Fax(string document);
}

public class OldPrinter : IPrinter
{
    public void Print(string document) => Console.WriteLine($"[OldPrinter] In: {document}");
}

public class ModernPrinter : IPrinter, IScanner, IFax
{
    public void Print(string document) => Console.WriteLine($"[ModernPrinter] In: {document}");
    public void Scan(string document) => Console.WriteLine($"[ModernPrinter] Quet: {document}");
    public void Fax(string document) => Console.WriteLine($"[ModernPrinter] Fax: {document}");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IPrinter oldPrinter = new OldPrinter();
        oldPrinter.Print("Hop dong.pdf");

        var modernPrinter = new ModernPrinter();
        modernPrinter.Print("Hop dong.pdf");
        modernPrinter.Scan("Hop dong.pdf");
        modernPrinter.Fax("Hop dong.pdf");
    }
}
// [OldPrinter] In: Hop dong.pdf
// [ModernPrinter] In: Hop dong.pdf
// [ModernPrinter] Quet: Hop dong.pdf
// [ModernPrinter] Fax: Hop dong.pdf
```

### Ví dụ 2 — Nhân viên/Robot: tách khả năng làm việc và ăn uống

**Bài toán:** Interface `IWorker` gộp `Work()` và `Eat()`. Khi hệ thống bổ sung `RobotWorker` (không cần ăn), class này buộc phải hiện thực đủ `IWorker`, override `Eat()` để trống hoặc ném exception — dù về logic, Robot không hề "ăn".

**Ý nghĩa của ISP trong ví dụ này:** Tách `IWorker` thành `IWorkable` (`Work()`) và `IFeedable` (`Eat()`). `HumanWorker` hiện thực cả hai, `RobotWorker` chỉ hiện thực `IWorkable`. Nơi tính giờ nghỉ ăn trưa chỉ cần phụ thuộc `IFeedable`, tự động chỉ áp dụng đúng cho những worker thực sự cần ăn.

**Cách implementation (C#):**

```csharp
public interface IWorkable
{
    void Work();
}

public interface IFeedable
{
    void Eat();
}

public class HumanWorker : IWorkable, IFeedable
{
    public void Work() => Console.WriteLine("[Human] Dang lam viec");
    public void Eat() => Console.WriteLine("[Human] Dang an trua");
}

public class RobotWorker : IWorkable
{
    public void Work() => Console.WriteLine("[Robot] Dang lam viec");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var workers = new List<IWorkable> { new HumanWorker(), new RobotWorker() };
        foreach (var worker in workers)
        {
            worker.Work();
        }

        // Chi nhung worker thuc su can an moi xuat hien o day
        var feedableWorkers = workers.OfType<IFeedable>();
        foreach (var feedable in feedableWorkers)
        {
            feedable.Eat();
        }
    }
}
// [Human] Dang lam viec
// [Robot] Dang lam viec
// [Human] Dang an trua
```

### Ví dụ 3 — Cổng thanh toán: tách Thu tiền / Hoàn tiền / Lưu thẻ

**Bài toán:** Interface `IPaymentGateway` gộp `Charge()`, `Refund()`, `SaveCard()`. Một số cổng thanh toán đơn giản (`CashOnDeliveryGateway` — thanh toán khi nhận hàng) không hỗ trợ hoàn tiền qua hệ thống lẫn lưu thẻ, nhưng vẫn phải hiện thực đủ `IPaymentGateway`, override 2 method còn lại bằng exception.

**Ý nghĩa của ISP trong ví dụ này:** Tách thành `IChargeable` (`Charge()`), `IRefundable` (`Refund()`), `ICardStorage` (`SaveCard()`). `CashOnDeliveryGateway` chỉ hiện thực `IChargeable`; `CreditCardGateway` hiện thực cả 3. Nơi cần hoàn tiền chỉ phụ thuộc `IRefundable`, không còn rủi ro gọi nhầm vào cổng không hỗ trợ.

**Cách implementation (C#):**

```csharp
public interface IChargeable
{
    string Charge(decimal amount);
}

public interface IRefundable
{
    string Refund(decimal amount);
}

public interface ICardStorage
{
    void SaveCard(string cardToken);
}

public class CashOnDeliveryGateway : IChargeable
{
    public string Charge(decimal amount) => $"[COD] Thu {amount:N0} khi giao hang";
}

public class CreditCardGateway : IChargeable, IRefundable, ICardStorage
{
    public string Charge(decimal amount) => $"[CreditCard] Da tru {amount:N0}";
    public string Refund(decimal amount) => $"[CreditCard] Da hoan {amount:N0}";
    public void SaveCard(string cardToken) => Console.WriteLine($"[CreditCard] Da luu the {cardToken}");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IChargeable cod = new CashOnDeliveryGateway();
        Console.WriteLine(cod.Charge(200000));

        var creditCard = new CreditCardGateway();
        Console.WriteLine(creditCard.Charge(500000));
        Console.WriteLine(creditCard.Refund(100000));
        creditCard.SaveCard("tok_123");
    }
}
// [COD] Thu 200,000 khi giao hang
// [CreditCard] Da tru 500,000
// [CreditCard] Da hoan 100,000
// [CreditCard] Da luu the tok_123
```
