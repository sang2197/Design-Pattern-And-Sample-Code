# Singleton Pattern

## 1. Khái niệm cơ bản

**Singleton** là một Creational Design Pattern, đảm bảo một class **chỉ có đúng một instance** duy nhất trong toàn bộ ứng dụng, đồng thời cung cấp một **điểm truy cập toàn cục** (global access point) tới instance đó.

Chỉ có một thành phần:

- **Singleton class:** tự quản lý instance duy nhất của chính nó — giấu constructor (đặt `private`) để bên ngoài không thể `new` trực tiếp, và cung cấp một static property/method để lấy về instance đó.

## 2. Bài toán

Một số tài nguyên trong hệ thống, về bản chất, chỉ nên tồn tại đúng một lần trong suốt vòng đời ứng dụng — ví dụ bộ đọc cấu hình, logger dùng chung, bộ đếm toàn cục. Nếu để code tự do `new` class đó ở nhiều nơi, mỗi nơi sẽ có một instance riêng, dẫn tới:

- **Trạng thái không đồng nhất:** mỗi instance giữ dữ liệu khác nhau, dù đáng lẽ mọi nơi phải nhìn thấy cùng một trạng thái dùng chung.
- **Lãng phí tài nguyên:** nếu việc khởi tạo class đó tốn kém (đọc file, mở kết nối...), tạo nhiều instance đồng nghĩa lặp lại chi phí đó nhiều lần không cần thiết.
- **Không kiểm soát được số lượng instance:** không có cơ chế nào ngăn code ở một nơi khác vô tình tạo thêm một instance mới, phá vỡ giả định "chỉ có một" mà phần còn lại của hệ thống đang dựa vào.

## 3. Ý nghĩa của Singleton

- Đảm bảo **toàn bộ ứng dụng luôn thao tác trên cùng một trạng thái dùng chung**, không bị phân mảnh giữa nhiều instance.
- Giấu constructor giúp **ngăn chặn hoàn toàn** việc tạo thêm instance ngoài ý muốn — đây là ràng buộc được đảm bảo bởi chính compiler, không phải chỉ là quy ước.
- Cung cấp **một điểm truy cập duy nhất, thống nhất** trong toàn bộ codebase, dễ dàng biết chỗ nào đang thao tác với tài nguyên dùng chung này.
- **Cần cẩn trọng khi dùng:** Singleton dễ bị lạm dụng thành "global state" — nếu dùng tràn lan, code sẽ khó unit test (khó thay thế bằng mock/stub) và tạo phụ thuộc ẩn giữa các phần không liên quan. Chỉ nên dùng khi có lý do thực sự cần đúng một instance, không dùng Singleton chỉ vì tiện lợi khi truy cập.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: nêu rõ khi nào nên dùng, cách sử dụng, và cách hiện thực (`Lazy<T>` để đảm bảo khởi tạo trễ và an toàn luồng), kèm một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Đọc cấu hình ứng dụng một lần duy nhất

**Khi nào dùng:** đọc file cấu hình (connection string, API key...) là thao tác tốn chi phí và kết quả không đổi trong suốt vòng đời ứng dụng — chỉ nên đọc đúng một lần rồi dùng lại.

**Cách sử dụng:** gọi `AppSettings.Instance` ở bất kỳ đâu cần cấu hình, thay vì tự `new AppSettings()`.

**Cách hiện thực:** constructor `private`, dùng `Lazy<AppSettings>` để việc khởi tạo chỉ xảy ra ở lần truy cập đầu tiên và an toàn khi nhiều luồng cùng gọi.

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

// Chạy thử
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

Lưu ý dòng log `"Da doc cau hinh tu file"` chỉ in ra **đúng một lần**, dù `Instance` được gọi hai lần — chứng minh `settings2` không tạo instance mới mà tái sử dụng lại `settings1`.

### Ví dụ 2 — Bộ đếm dùng chung trên toàn ứng dụng

**Khi nào dùng:** đếm tổng số request toàn hệ thống, được tăng lên từ nhiều nơi khác nhau trong code — nếu mỗi nơi giữ một biến đếm riêng thì con số cuối cùng sẽ sai.

**Cách sử dụng:** mọi nơi cần tăng bộ đếm chỉ cần gọi `RequestCounter.Instance.Increment()`.

**Cách hiện thực:** tương tự Ví dụ 1, nhưng Singleton lần này giữ một trạng thái có thể thay đổi (`_count`) dùng chung.

```csharp
public sealed class RequestCounter
{
    private static readonly Lazy<RequestCounter> _instance = new Lazy<RequestCounter>(() => new RequestCounter());
    private int _count;

    private RequestCounter()
    {
    }

    public static RequestCounter Instance => _instance.Value;

    public void Increment()
    {
        _count++;
    }

    public int GetCount() => _count;
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        RequestCounter.Instance.Increment();
        RequestCounter.Instance.Increment();
        RequestCounter.Instance.Increment();

        Console.WriteLine(RequestCounter.Instance.GetCount());
    }
}
// 3
```

### Ví dụ 3 — Logger dùng chung cho nhiều service khác nhau

**Khi nào dùng:** nhiều service độc lập (`OrderService`, `PaymentService`...) đều cần ghi log vào cùng một nơi tập trung, thay vì mỗi service tự tạo một logger riêng.

**Cách sử dụng:** các service không giữ tham chiếu logger qua constructor, mà gọi thẳng `AppLogger.Instance.Log(...)` khi cần.

**Cách hiện thực:** giống Ví dụ 1 và 2, minh họa việc nhiều class không liên quan (`OrderService`, `PaymentService`) cùng chia sẻ một Singleton mà không cần biết tới nhau.

```csharp
public sealed class AppLogger
{
    private static readonly Lazy<AppLogger> _instance = new Lazy<AppLogger>(() => new AppLogger());
    private readonly List<string> _logs = new List<string>();

    private AppLogger()
    {
    }

    public static AppLogger Instance => _instance.Value;

    public void Log(string message)
    {
        _logs.Add(message);
        Console.WriteLine($"[Log] {message}");
    }

    public int LogCount => _logs.Count;
}

public class OrderService
{
    public void CreateOrder(string orderId)
    {
        AppLogger.Instance.Log($"Tao don hang {orderId}");
    }
}

public class PaymentService
{
    public void Pay(string orderId)
    {
        AppLogger.Instance.Log($"Thanh toan don hang {orderId}");
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var orderService = new OrderService();
        var paymentService = new PaymentService();

        orderService.CreateOrder("DH0001");
        paymentService.Pay("DH0001");

        Console.WriteLine($"Tong so log: {AppLogger.Instance.LogCount}");
    }
}
// [Log] Tao don hang DH0001
// [Log] Thanh toan don hang DH0001
// Tong so log: 2
```
