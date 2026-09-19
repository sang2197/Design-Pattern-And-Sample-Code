# Dependency Inversion Principle (DIP)

## 1. Khái niệm cơ bản

**Dependency Inversion Principle** phát biểu: **module cấp cao (high-level, chứa logic nghiệp vụ) không nên phụ thuộc trực tiếp vào module cấp thấp (low-level, chi tiết hạ tầng); cả hai nên phụ thuộc vào abstraction. Abstraction không nên phụ thuộc vào chi tiết; chi tiết nên phụ thuộc vào abstraction.**

Nói cách khác: nên viết code nghiệp vụ theo interface, còn implementation cụ thể (database, API bên ngoài, hệ thống file...) được **inject** từ bên ngoài vào — thường qua constructor. Đây chính là nền tảng của kỹ thuật **Dependency Injection**.

## 2. Khi nào nên dùng

Dùng khi:

✅ Logic nghiệp vụ cấp cao đang `new()` trực tiếp một class hạ tầng cụ thể (database, file, API bên ngoài...)

✅ Muốn đổi implementation hạ tầng (đổi provider, đổi công nghệ lưu trữ...) mà không sửa logic nghiệp vụ

✅ Muốn unit test logic nghiệp vụ mà không cần phụ thuộc hạ tầng thật (dùng fake/mock qua interface)

✅ Nhiều module cấp cao khác nhau cùng cần dùng chung một loại hạ tầng, muốn thống nhất qua một abstraction

## 3. Code examples

### Ví dụ 1 — Đặt hàng: tách abstraction cho nơi lưu trữ đơn hàng

**Bài toán:** `OrderService` (module cấp cao, chứa logic nghiệp vụ đặt hàng) tự `new SqlOrderRepository()` ngay bên trong để lưu đơn hàng. Khi công ty muốn đổi sang lưu trên MongoDB, hoặc muốn viết unit test cho `OrderService` mà không đụng tới database thật, đều không thể làm được — vì `OrderService` đang phụ thuộc cứng vào chi tiết hạ tầng cụ thể (`SqlOrderRepository`).

**Ý nghĩa của DIP trong ví dụ này:** `OrderService` chỉ phụ thuộc vào abstraction `IOrderRepository`; implementation cụ thể (`SqlOrderRepository` hay `MongoOrderRepository`) được truyền vào từ bên ngoài qua constructor. Đổi hạ tầng lưu trữ chỉ cần đổi implementation được inject, không sửa gì trong `OrderService`.

**Cách implementation (C#):**

```csharp
public interface IOrderRepository
{
    void Save(string orderId, decimal amount);
}

public class SqlOrderRepository : IOrderRepository
{
    public void Save(string orderId, decimal amount) => Console.WriteLine($"[SQL] Da luu don {orderId}: {amount:N0}");
}

public class MongoOrderRepository : IOrderRepository
{
    public void Save(string orderId, decimal amount) => Console.WriteLine($"[Mongo] Da luu don {orderId}: {amount:N0}");
}

// Module cap cao - chi phu thuoc abstraction IOrderRepository
public class OrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public void PlaceOrder(string orderId, decimal amount)
    {
        Console.WriteLine($"[OrderService] Xu ly don hang {orderId}");
        _repository.Save(orderId, amount);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var sqlService = new OrderService(new SqlOrderRepository());
        sqlService.PlaceOrder("DH0001", 500000);

        // Doi sang Mongo - khong sua gi trong OrderService
        var mongoService = new OrderService(new MongoOrderRepository());
        mongoService.PlaceOrder("DH0002", 300000);
    }
}
// [OrderService] Xu ly don hang DH0001
// [SQL] Da luu don DH0001: 500,000
// [OrderService] Xu ly don hang DH0002
// [Mongo] Da luu don DH0002: 300,000
```

### Ví dụ 2 — Gửi mã đặt lại mật khẩu: tách abstraction cho kênh gửi

**Bài toán:** `PasswordResetService` (module cấp cao) tự `new EmailSender()` bên trong để gửi email đặt lại mật khẩu. Khi cần gửi thêm qua SMS, hoặc cần test `PasswordResetService` mà không thực sự gửi email, phải sửa trực tiếp vào `PasswordResetService` vì nó đang phụ thuộc cứng vào `EmailSender`.

**Ý nghĩa của DIP trong ví dụ này:** `PasswordResetService` chỉ phụ thuộc `INotificationChannel`; `EmailSender`/`SmsSender` hiện thực interface này và được inject từ ngoài vào qua constructor. Muốn đổi kênh gửi hoặc test với một channel giả, chỉ cần thay implementation được truyền vào.

**Cách implementation (C#):**

```csharp
public interface INotificationChannel
{
    void Send(string recipient, string message);
}

public class EmailSender : INotificationChannel
{
    public void Send(string recipient, string message) => Console.WriteLine($"[Email->{recipient}] {message}");
}

public class SmsSender : INotificationChannel
{
    public void Send(string recipient, string message) => Console.WriteLine($"[SMS->{recipient}] {message}");
}

// Module cap cao - chi phu thuoc abstraction INotificationChannel
public class PasswordResetService
{
    private readonly INotificationChannel _channel;

    public PasswordResetService(INotificationChannel channel)
    {
        _channel = channel;
    }

    public void SendResetCode(string recipient, string code)
    {
        _channel.Send(recipient, $"Ma dat lai mat khau cua ban: {code}");
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var emailService = new PasswordResetService(new EmailSender());
        emailService.SendResetCode("user@example.com", "123456");

        var smsService = new PasswordResetService(new SmsSender());
        smsService.SendResetCode("0900000000", "654321");
    }
}
// [Email->user@example.com] Ma dat lai mat khau cua ban: 123456
// [SMS->0900000000] Ma dat lai mat khau cua ban: 654321
```

### Ví dụ 3 — Xuất báo cáo: tách abstraction cho nơi ghi file

**Bài toán:** `MonthlyReportService` gọi thẳng API ghi file của hệ điều hành để ghi báo cáo ra đĩa ngay trong logic tạo báo cáo. Viết unit test cho logic tạo nội dung báo cáo buộc phải ghi ra file thật trên đĩa mỗi lần chạy test — chậm và dễ vướng quyền truy cập file khi chạy trên CI.

**Ý nghĩa của DIP trong ví dụ này:** Tách `IFileWriter` làm abstraction cho việc ghi file; `MonthlyReportService` chỉ phụ thuộc `IFileWriter`, còn `DiskFileWriter` (ghi thật ra đĩa) hoặc `InMemoryFileWriter` (dùng khi test) được inject vào từ ngoài, không đụng tới ổ đĩa thật khi test.

**Cách implementation (C#):**

```csharp
public interface IFileWriter
{
    void Write(string path, string content);
}

public class DiskFileWriter : IFileWriter
{
    public void Write(string path, string content) => Console.WriteLine($"[Disk] Da ghi file {path}");
}

public class InMemoryFileWriter : IFileWriter
{
    public Dictionary<string, string> Files { get; } = new Dictionary<string, string>();

    public void Write(string path, string content) => Files[path] = content;
}

// Module cap cao - chi phu thuoc abstraction IFileWriter
public class MonthlyReportService
{
    private readonly IFileWriter _fileWriter;

    public MonthlyReportService(IFileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }

    public void GenerateReport(string month, decimal revenue)
    {
        var content = $"Bao cao thang {month}: doanh thu {revenue:N0}";
        _fileWriter.Write($"report-{month}.txt", content);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var service = new MonthlyReportService(new DiskFileWriter());
        service.GenerateReport("09-2026", 1_200_000_000);

        // Dung InMemoryFileWriter khi test - khong dung toi o dia that
        var inMemoryWriter = new InMemoryFileWriter();
        var testService = new MonthlyReportService(inMemoryWriter);
        testService.GenerateReport("10-2026", 900_000_000);
        Console.WriteLine(inMemoryWriter.Files["report-10-2026.txt"]);
    }
}
// [Disk] Da ghi file report-09-2026.txt
// Bao cao thang 10-2026: doanh thu 900,000,000
```
