# Singleton Pattern

## 1. Khái niệm cơ bản

**Singleton** là một Creational Design Pattern, đảm bảo một class **chỉ có đúng một instance** duy nhất trong toàn bộ ứng dụng, đồng thời cung cấp một **điểm truy cập toàn cục** (global access point) tới instance đó.

Chỉ có một thành phần:

- **Singleton class:** tự quản lý instance duy nhất của chính nó — giấu constructor (đặt `private`) để bên ngoài không thể `new` trực tiếp, và cung cấp một static property/method để lấy về instance đó. Trong C#, thường dùng `Lazy<T>` để việc khởi tạo chỉ xảy ra ở lần truy cập đầu tiên và an toàn khi nhiều luồng cùng gọi.

**Lưu ý:** Singleton dễ bị lạm dụng thành "global state" — nếu dùng tràn lan, code sẽ khó unit test (khó thay thế bằng mock/stub) và tạo phụ thuộc ẩn giữa các phần không liên quan. Chỉ nên dùng khi thực sự cần đúng một instance, không dùng chỉ vì tiện lợi khi truy cập.

## 2. Khi nào nên dùng

Dùng khi:

✅ Tài nguyên/đối tượng về bản chất chỉ nên tồn tại đúng một instance trong toàn ứng dụng

✅ Nhiều nơi trong code cần truy cập cùng một trạng thái dùng chung

✅ Việc khởi tạo tốn kém (đọc file, gọi API, mở kết nối...) và chỉ muốn thực hiện một lần

✅ Cần kiểm soát chặt số lượng instance, không cho code bên ngoài tự `new`

## 3. Code examples

### Ví dụ 1 — Đọc cấu hình ứng dụng một lần duy nhất

**Bài toán:** Ứng dụng cần đọc file cấu hình (connection string, API key...) khi chạy. Đây là thao tác tốn chi phí và kết quả không đổi trong suốt vòng đời ứng dụng. Nếu mỗi module tự `new AppSettings()` để đọc cấu hình, file sẽ bị đọc lặp lại nhiều lần không cần thiết, và nếu cấu hình được nạp ở những thời điểm khác nhau, mỗi module có thể nhìn thấy một giá trị khác nhau.

**Ý nghĩa của Singleton trong ví dụ này:** `AppSettings` giấu constructor (`private`) và chỉ cho lấy instance qua `AppSettings.Instance`. Nhờ `Lazy<AppSettings>`, file cấu hình chỉ được đọc đúng một lần ở lần truy cập đầu tiên, và mọi nơi trong ứng dụng đều nhận về cùng một instance — cùng một giá trị cấu hình.

**Cách implementation (C#):**

```csharp
public sealed class AppSettings
{
    private static readonly Lazy<AppSettings> _instance = new Lazy<AppSettings>(() => new AppSettings());

    public string ConnectionString { get; }

    private AppSettings()
    {
        // Gia lap doc file cau hinh - chi chay dung 1 lan
        ConnectionString = "Server=localhost;Database=MyApp;";
        Console.WriteLine("[AppSettings] Da doc cau hinh tu file");
    }

    public static AppSettings Instance => _instance.Value;
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var settings1 = AppSettings.Instance;
        var settings2 = AppSettings.Instance;

        Console.WriteLine(settings1.ConnectionString);
        Console.WriteLine(ReferenceEquals(settings1, settings2));
    }
}
// [AppSettings] Da doc cau hinh tu file
// Server=localhost;Database=MyApp;
// True
```

### Ví dụ 2 — Cache tỷ giá dùng chung cho nhiều service

**Bài toán:** Hệ thống thương mại điện tử có nhiều service độc lập (`CheckoutService`, `InvoiceService`...) đều cần tỷ giá ngoại tệ để quy đổi sang VND, mà tỷ giá lấy từ một API bên ngoài rất tốn thời gian. Nếu mỗi service tự tạo cache tỷ giá riêng, API sẽ bị gọi lặp lại nhiều lần, và hai service có thể đang dùng hai bộ tỷ giá khác nhau tại cùng một thời điểm.

**Ý nghĩa của Singleton trong ví dụ này:** `ExchangeRateCache` chỉ có một instance duy nhất; API tỷ giá chỉ được gọi một lần khi instance được tạo, và mọi service dùng chung cùng một bộ tỷ giá. `CheckoutService` và `InvoiceService` không cần biết tới nhau, chỉ cần gọi `ExchangeRateCache.Instance`.

**Cách implementation (C#):**

```csharp
public sealed class ExchangeRateCache
{
    private static readonly Lazy<ExchangeRateCache> _instance = new Lazy<ExchangeRateCache>(() => new ExchangeRateCache());
    private readonly Dictionary<string, decimal> _rates;

    private ExchangeRateCache()
    {
        // Gia lap goi API ty gia ton kem - chi chay dung 1 lan
        Console.WriteLine("[ExchangeRateCache] Da tai ty gia tu API");
        _rates = new Dictionary<string, decimal> { ["USD"] = 25000m, ["EUR"] = 27000m };
    }

    public static ExchangeRateCache Instance => _instance.Value;

    public decimal ToVnd(string currency, decimal amount) => _rates[currency] * amount;
}

public class CheckoutService
{
    public decimal ConvertUsd(decimal usd) => ExchangeRateCache.Instance.ToVnd("USD", usd);
}

public class InvoiceService
{
    public decimal ConvertEur(decimal eur) => ExchangeRateCache.Instance.ToVnd("EUR", eur);
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var checkout = new CheckoutService();
        var invoice = new InvoiceService();

        Console.WriteLine(checkout.ConvertUsd(10));
        Console.WriteLine(invoice.ConvertEur(5));
    }
}
// [ExchangeRateCache] Da tai ty gia tu API
// 250000
// 135000
```

### Ví dụ 3 — Print Manager quản lý hàng đợi in dùng chung

**Bài toán:** Một ứng dụng văn phòng có nhiều module (`ReportService`, `InvoiceService`...) đều có thể gửi tài liệu tới cùng một máy in. Các tài liệu cần được đưa vào **một hàng đợi chung** và xử lý lần lượt. Nếu mỗi module tự `new PrintManager()`, hệ thống sẽ có nhiều hàng đợi độc lập, không còn một nơi duy nhất kiểm soát thứ tự các tài liệu đang chờ in.

**Ý nghĩa của Singleton trong ví dụ này:** `PrintManager` được đảm bảo chỉ có một instance trong ứng dụng, vì vậy mọi module đều gửi tài liệu vào **cùng một hàng đợi**. Constructor được đặt `private` để bên ngoài không thể tự tạo thêm `PrintManager`, còn `PrintManager.Instance` cung cấp điểm truy cập tới instance duy nhất đó. `Lazy<PrintManager>` đảm bảo instance chỉ được tạo khi cần sử dụng lần đầu và an toàn khi nhiều luồng cùng truy cập.

**Cách implementation (C#):**

```csharp
public sealed class PrintManager
{
    private static readonly Lazy<PrintManager> _instance =
        new Lazy<PrintManager>(() => new PrintManager());

    private readonly Queue<string> _printQueue = new Queue<string>();
    private readonly object _lock = new object();

    private PrintManager()
    {
    }

    public static PrintManager Instance => _instance.Value;

    public void AddJob(string document)
    {
        lock (_lock)
        {
            _printQueue.Enqueue(document);
        }
    }

    public void PrintNext()
    {
        lock (_lock)
        {
            if (_printQueue.Count == 0)
                return;

            var document = _printQueue.Dequeue();
            Console.WriteLine($"Dang in: {document}");
        }
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class ReportService
{
    public void PrintReport()
    {
        PrintManager.Instance.AddJob("BaoCaoThang.pdf");
    }
}

public class InvoiceService
{
    public void PrintInvoice()
    {
        PrintManager.Instance.AddJob("HoaDon001.pdf");
    }
}

public class Program
{
    public static void Main()
    {
        var reportService = new ReportService();
        var invoiceService = new InvoiceService();

        reportService.PrintReport();
        invoiceService.PrintInvoice();

        PrintManager.Instance.PrintNext();
        PrintManager.Instance.PrintNext();
    }
}

// Dang in: BaoCaoThang.pdf
// Dang in: HoaDon001.pdf
```
