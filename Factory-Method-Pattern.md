# Factory Method Pattern

## 1. Khái niệm cơ bản

**Factory Method** là một Creational Design Pattern, định nghĩa một method để tạo đối tượng, nhưng **cho phép lớp con (subclass) quyết định lớp cụ thể nào sẽ được khởi tạo**, thay vì để logic dùng chung trực tiếp `new` các đối tượng cụ thể.

Pattern gồm 4 thành phần:

- **Product (interface/abstract class):** khai báo interface chung cho các đối tượng được tạo ra.
- **ConcreteProduct:** các implementation cụ thể của `Product`.
- **Creator (abstract class):** khai báo `FactoryMethod()` trả về `Product`. Creator thường chứa logic xử lý dùng chung và sử dụng Product thông qua abstraction mà không cần biết Product cụ thể là gì.
- **ConcreteCreator:** override/implement `FactoryMethod()` và quyết định `ConcreteProduct` cụ thể nào sẽ được tạo ra.

## 2. Bài toán

Logic nghiệp vụ dùng chung (ví dụ luồng thanh toán, luồng xuất báo cáo) thường cần khởi tạo một đối tượng cụ thể để hoàn thành công việc, nhưng đối tượng cụ thể đó có thể thay đổi tùy tình huống — ví dụ `new VnPayPayment()` hay `new MomoPayment()` tùy phương thức thanh toán khách chọn.

Nếu để lời gọi `new` nằm ngay trong logic dùng chung, hai vấn đề sẽ xảy ra:

- Logic dùng chung bị **trộn lẫn** với chi tiết khởi tạo từng biến thể cụ thể, thường dưới dạng `if/switch` liệt kê tất cả các loại.
- Mỗi khi bổ sung một biến thể mới, phải **sửa lại chính method dùng chung** đó — vi phạm Open/Closed Principle, và có nguy cơ ảnh hưởng tới các biến thể đã hoạt động ổn định trước đó.

## 3. Ý nghĩa của Factory Method

- **Factory Method tách việc tạo đối tượng khỏi logic sử dụng đối tượng:** `Creator` định nghĩa luồng xử lý chung và làm việc với abstraction `Product`. Việc quyết định `ConcreteProduct` nào được tạo được giao cho `ConcreteCreator` thông qua `FactoryMethod()`.
- Khi cần thêm một biến thể mới, chỉ cần tạo thêm `ConcreteProduct` và `ConcreteCreator` tương ứng mà không cần sửa workflow chung trong `Creator`.
- Tuân thủ **Open/Closed Principle**: phần workflow ổn định được giữ nguyên trong khi hệ thống vẫn có thể mở rộng bằng các implementation mới.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: có Product, ConcreteProduct, Creator, ConcreteCreator và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Xử lý thanh toán theo phương thức (VNPay / Momo)

**Khi nào dùng:** luồng checkout (`Checkout()`) giống nhau bất kể phương thức thanh toán nào, chỉ khác ở bước tạo `IPaymentMethod` cụ thể.

**Cách sử dụng:** Client chỉ cần chọn đúng `ConcreteCreator` (`VnPayCheckoutProcessor` hoặc `MomoCheckoutProcessor`), sau đó gọi `Checkout()` như nhau.

**Cách hiện thực:** `CheckoutProcessor` (Creator) định nghĩa `Checkout()` dùng chung, ủy quyền việc tạo `IPaymentMethod` cho `FactoryMethod()` trừu tượng; mỗi `ConcreteCreator` override để trả về đúng `ConcreteProduct`.

```csharp
// Product
public interface IPaymentMethod
{
    string Pay(decimal amount);
}

// ConcreteProduct
public class VnPayPayment : IPaymentMethod
{
    public string Pay(decimal amount) => $"Thanh toan {amount:N0} qua VNPay";
}

public class MomoPayment : IPaymentMethod
{
    public string Pay(decimal amount) => $"Thanh toan {amount:N0} qua Momo";
}

// Creator
public abstract class CheckoutProcessor
{
    // Factory Method - để lớp con quyết định tạo IPaymentMethod nào
    protected abstract IPaymentMethod CreatePaymentMethod();

    // Logic nghiệp vụ dùng chung, chỉ phụ thuộc abstraction IPaymentMethod
    public string Checkout(decimal amount)
    {
        var payment = CreatePaymentMethod();
        return $"[Checkout] {payment.Pay(amount)}";
    }
}

// ConcreteCreator
public class VnPayCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePaymentMethod() => new VnPayPayment();
}

public class MomoCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePaymentMethod() => new MomoPayment();
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        CheckoutProcessor vnpayProcessor = new VnPayCheckoutProcessor();
        CheckoutProcessor momoProcessor = new MomoCheckoutProcessor();

        Console.WriteLine(vnpayProcessor.Checkout(100000)); // [Checkout] Thanh toan 100,000 qua VNPay
        Console.WriteLine(momoProcessor.Checkout(50000));   // [Checkout] Thanh toan 50,000 qua Momo
    }
}
```

### Ví dụ 2 — Xuất báo cáo theo định dạng file (PDF / Excel)

**Khi nào dùng:** bước tổng hợp nội dung báo cáo giống nhau, chỉ khác ở định dạng file xuất ra cuối cùng (PDF hay Excel).

**Cách sử dụng:** Client chọn `PdfReportGenerator` hoặc `ExcelReportGenerator`, gọi `GenerateReport()` mà không cần biết `IReportExporter` cụ thể nào được dùng bên trong.

**Cách hiện thực:** tương tự Ví dụ 1 — `ReportGenerator` (Creator) chứa `GenerateReport()` dùng chung, `CreateExporter()` là Factory Method để lớp con quyết định `ConcreteProduct`.

```csharp
// Product
public interface IReportExporter
{
    string Export(string content);
}

// ConcreteProduct
public class PdfReportExporter : IReportExporter
{
    public string Export(string content) => $"[PDF] {content}";
}

public class ExcelReportExporter : IReportExporter
{
    public string Export(string content) => $"[EXCEL] {content}";
}

// Creator
public abstract class ReportGenerator
{
    // Factory Method - để lớp con quyết định tạo IReportExporter nào
    protected abstract IReportExporter CreateExporter();

    // Logic nghiệp vụ dùng chung, chỉ phụ thuộc abstraction IReportExporter
    public string GenerateReport(string content)
    {
        var exporter = CreateExporter();
        return exporter.Export(content);
    }
}

// ConcreteCreator
public class PdfReportGenerator : ReportGenerator
{
    protected override IReportExporter CreateExporter() => new PdfReportExporter();
}

public class ExcelReportGenerator : ReportGenerator
{
    protected override IReportExporter CreateExporter() => new ExcelReportExporter();
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        ReportGenerator pdfGenerator = new PdfReportGenerator();
        ReportGenerator excelGenerator = new ExcelReportGenerator();

        Console.WriteLine(pdfGenerator.GenerateReport("Bao cao doanh thu thang 9"));   // [PDF] Bao cao doanh thu thang 9
        Console.WriteLine(excelGenerator.GenerateReport("Bao cao doanh thu thang 9")); // [EXCEL] Bao cao doanh thu thang 9
    }
}
```

### Ví dụ 3 — Gửi thông báo qua nhiều kênh (Email / SMS)

**Khi nào dùng:** cần gửi thông báo qua nhiều kênh khác nhau, và có thể có thêm kênh mới trong tương lai (Zalo, Push notification...) mà không muốn sửa vào luồng gửi chung.

**Cách sử dụng:** Client giữ một danh sách `NotificationService` (có thể trộn nhiều loại), gọi `Notify()` trên từng phần tử như nhau.

**Cách hiện thực:** `NotificationService` (Creator) định nghĩa `Notify()` dùng chung; `EmailNotificationService`/`SmsNotificationService` (ConcreteCreator) override `CreateNotifier()` để quyết định `INotifier` cụ thể.

```csharp
// Product
public interface INotifier
{
    string Send(string message);
}

// ConcreteProduct
public class EmailNotifier : INotifier
{
    public string Send(string message) => $"[Email] {message}";
}

public class SmsNotifier : INotifier
{
    public string Send(string message) => $"[SMS] {message}";
}

// Creator
public abstract class NotificationService
{
    // Factory Method - để lớp con quyết định tạo INotifier nào
    protected abstract INotifier CreateNotifier();

    // Logic nghiệp vụ dùng chung, chỉ phụ thuộc abstraction INotifier
    public string Notify(string message)
    {
        var notifier = CreateNotifier();
        return notifier.Send(message);
    }
}

// ConcreteCreator
public class EmailNotificationService : NotificationService
{
    protected override INotifier CreateNotifier() => new EmailNotifier();
}

public class SmsNotificationService : NotificationService
{
    protected override INotifier CreateNotifier() => new SmsNotifier();
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var services = new List<NotificationService>
        {
            new EmailNotificationService(),
            new SmsNotificationService()
        };

        foreach (var service in services)
        {
            Console.WriteLine(service.Notify("Don hang DH0001 da duoc xac nhan"));
        }
        // [Email] Don hang DH0001 da duoc xac nhan
        // [SMS] Don hang DH0001 da duoc xac nhan
    }
}
```
