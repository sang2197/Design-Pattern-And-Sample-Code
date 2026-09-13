# Adapter Pattern

## 1. Khái niệm

**Adapter** là một structural design pattern, dùng để chuyển interface của một class có sẵn (`Adaptee`) thành interface mà `Client` đang mong đợi (`Target`).

Nhờ đó, hai thành phần có interface không tương thích vẫn có thể làm việc với nhau mà không cần sửa code gốc của `Adaptee`.

Pattern gồm 3 thành phần:

- **Target (interface):** interface mà Client mong muốn sử dụng.

- **Adaptee:** class đã tồn tại nhưng có interface không tương thích với Target, thường là code legacy hoặc thư viện bên thứ ba.

- **Adapter:** hiện thực `Target`, giữ tham chiếu tới `Adaptee` và chuyển lời gọi từ Target sang lời gọi phù hợp trên Adaptee.

## 2. Ý nghĩa

- **Vấn đề nếu không dùng Adapter:** hệ thống đang sử dụng một interface chung như `ILogger` hoặc `IPaymentGateway`, nhưng thư viện bên thứ ba lại có method và kiểu dữ liệu khác. Nếu Client gọi trực tiếp thư viện đó, Client sẽ bị phụ thuộc vào interface riêng của provider.

- **Adapter giải quyết bằng cách tạo một lớp trung gian:** Client vẫn chỉ làm việc với `Target`, còn Adapter chịu trách nhiệm chuyển đổi method, tham số hoặc kết quả để gọi `Adaptee`.

- Cho phép **tái sử dụng code cũ hoặc thư viện bên ngoài** mà không cần sửa source của chúng.

- Giúp Client **chỉ phụ thuộc vào abstraction (`Target`)**, nên khi đổi provider chỉ cần tạo Adapter mới mà không phải sửa logic nghiệp vụ.

## 3. Code mẫu

### Ví dụ 1 — Target interface và Adaptee

```csharp id="pjwmd4"
// Target - interface mà hệ thống đang sử dụng
public interface ISmsSender
{
    void Send(string phoneNumber, string message);
}

// Adaptee - thư viện bên thứ ba có interface khác
public class ThirdPartySmsClient
{
    public void DeliverMessage(string content, string toNumber, bool isUrgent)
    {
        Console.WriteLine($"[ThirdParty] Gui '{content}' den {toNumber} (urgent={isUrgent})");
    }
}
```

### Ví dụ 2 — Adapter chuyển đổi giữa Target và Adaptee

```csharp id="yznld1"
public class ThirdPartySmsAdapter : ISmsSender
{
    private readonly ThirdPartySmsClient _client;

    public ThirdPartySmsAdapter(ThirdPartySmsClient client)
    {
        _client = client;
    }

    public void Send(string phoneNumber, string message)
    {
        _client.DeliverMessage(message, phoneNumber, isUrgent: false);
    }
}
```

### Ví dụ 3 — Client chỉ phụ thuộc Target

```csharp id="rcyrql"
public class OrderNotificationService
{
    private readonly ISmsSender _smsSender;

    public OrderNotificationService(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }

    public void NotifyOrderCreated(string phoneNumber, string orderId)
    {
        _smsSender.Send(phoneNumber, $"Don hang {orderId} da duoc tao thanh cong");
    }
}
```

```csharp id="bs45qi"
ISmsSender smsSender = new ThirdPartySmsAdapter(new ThirdPartySmsClient());

var service = new OrderNotificationService(smsSender);

service.NotifyOrderCreated("0900000000", "DH0001");
```

Sau này nếu đổi sang nhà cung cấp SMS khác, chỉ cần tạo Adapter mới. `OrderNotificationService` không cần sửa.