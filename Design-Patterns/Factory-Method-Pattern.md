# Factory Method Pattern

## 1. Khái niệm cơ bản

**Factory Method** là một Creational Design Pattern, định nghĩa một method để tạo đối tượng, nhưng **cho phép lớp con (subclass) quyết định lớp cụ thể nào sẽ được khởi tạo**, thay vì để logic dùng chung trực tiếp `new` các đối tượng cụ thể.

Pattern gồm 4 thành phần:

- **Product (interface/abstract class):** khai báo interface chung cho các đối tượng được tạo ra.
- **ConcreteProduct:** các implementation cụ thể của `Product`.
- **Creator (abstract class):** khai báo `FactoryMethod()` trả về `Product`. Creator thường chứa logic xử lý dùng chung và sử dụng Product thông qua abstraction mà không cần biết Product cụ thể là gì.
- **ConcreteCreator:** override/implement `FactoryMethod()` và quyết định `ConcreteProduct` cụ thể nào sẽ được tạo ra.

## 2. Khi nào nên dùng

Dùng khi:

✅ Không muốn code nghiệp vụ phụ thuộc class cụ thể

✅ Hệ thống có thể mở rộng thêm loại object mới

✅ Muốn tuân thủ Open/Closed Principle (thêm mới không sửa code cũ)

✅ Creator có quy trình xử lý chung nhưng muốn cho subclass quyết định loại Product cụ thể được sử dụng.

## 3. Code examples

### Ví dụ 1 — Xử lý thanh toán theo phương thức (VNPay / Momo)

**Bài toán:** Hệ thống checkout cần hỗ trợ nhiều phương thức thanh toán (VNPay, Momo...), nhưng luồng xử lý (tính tiền, gọi thanh toán, trả kết quả) giống nhau ở mọi phương thức. Nếu đặt `if/switch` chọn phương thức ngay trong luồng checkout, mỗi lần thêm phương thức mới (ví dụ ZaloPay) đều phải sửa lại chính hàm checkout dùng chung.

**Ý nghĩa của Factory Method trong ví dụ này:** `CheckoutProcessor` định nghĩa `Checkout()` dùng chung và chỉ làm việc qua abstraction `IPaymentMethod`, không biết đó là VNPay hay Momo. Việc quyết định tạo `IPaymentMethod` nào được giao cho `ConcreteCreator` thông qua Factory Method `CreatePaymentMethod()`. Muốn thêm phương thức thanh toán mới, chỉ cần thêm một `ConcreteProduct` + `ConcreteCreator` mới mà không đụng vào `Checkout()`.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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

**Bài toán:** Ứng dụng cần xuất cùng một nội dung báo cáo ra nhiều định dạng file khác nhau (PDF, Excel...). Bước tổng hợp nội dung báo cáo là như nhau, chỉ khác ở bước xuất file cuối cùng. Nếu chọn định dạng bằng `if/switch` ngay trong hàm tạo báo cáo, việc thêm định dạng mới (Word, CSV...) sẽ buộc phải sửa lại hàm đó.

**Ý nghĩa của Factory Method trong ví dụ này:** `ReportGenerator` định nghĩa `GenerateReport()` dùng chung, chỉ thao tác qua abstraction `IReportExporter` mà không biết cụ thể là PDF hay Excel. Factory Method `CreateExporter()` được giao cho từng `ConcreteCreator` quyết định trả về `ConcreteProduct` nào. Thêm định dạng mới chỉ cần thêm `IReportExporter` + `ReportGenerator` con tương ứng.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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

### Ví dụ 3 — Hệ thống Logistics theo phương thức vận chuyển (Đường bộ / Đường biển)

**Bài toán:** Hệ thống logistics ban đầu chỉ hỗ trợ vận chuyển hàng hóa bằng đường bộ với xe tải. Sau này doanh nghiệp mở rộng vận chuyển quốc tế và cần hỗ trợ thêm đường biển bằng tàu, trong tương lai có thể tiếp tục thêm đường hàng không. Quy trình xử lý giao hàng (lập kế hoạch, chọn phương tiện, thực hiện vận chuyển, trả kết quả) về cơ bản giống nhau, nhưng loại phương tiện được sử dụng khác nhau. Nếu dùng `if/switch` để trực tiếp tạo `Truck`, `Ship`... trong quy trình giao hàng, mỗi khi thêm phương thức vận chuyển mới đều phải sửa logic dùng chung.

**Ý nghĩa của Factory Method trong ví dụ này:** `Logistics` định nghĩa `PlanDelivery()` là quy trình xử lý giao hàng dùng chung và chỉ làm việc thông qua abstraction `ITransport`. Việc quyết định phương tiện cụ thể được tạo ra được giao cho các `ConcreteCreator` thông qua Factory Method `CreateTransport()`. `RoadLogistics` tạo `Truck`, còn `SeaLogistics` tạo `Ship`. Khi cần thêm phương thức vận chuyển mới, ví dụ đường hàng không, có thể thêm `Plane` và `AirLogistics` mà không cần sửa logic `PlanDelivery()` đang có.

**Cách implementation (C#):**

```csharp
// Product
public interface ITransport
{
    string Deliver();
}

// ConcreteProduct
public class Truck : ITransport
{
    public string Deliver() => "Giao hang bang xe tai";
}

public class Ship : ITransport
{
    public string Deliver() => "Giao hang bang tau bien";
}

// Creator
public abstract class Logistics
{
    // Factory Method - để lớp con quyết định tạo ITransport nào
    protected abstract ITransport CreateTransport();

    // Logic nghiệp vụ dùng chung, chỉ phụ thuộc abstraction ITransport
    public string PlanDelivery()
    {
        var transport = CreateTransport();
        return $"[Delivery] {transport.Deliver()}";
    }
}

// ConcreteCreator
public class RoadLogistics : Logistics
{
    protected override ITransport CreateTransport() => new Truck();
}

public class SeaLogistics : Logistics
{
    protected override ITransport CreateTransport() => new Ship();
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        Logistics roadLogistics = new RoadLogistics();
        Logistics seaLogistics = new SeaLogistics();

        Console.WriteLine(roadLogistics.PlanDelivery());
        Console.WriteLine(seaLogistics.PlanDelivery());

        // [Delivery] Giao hang bang xe tai
        // [Delivery] Giao hang bang tau bien
    }
}
```