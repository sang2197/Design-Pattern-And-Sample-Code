# Adapter Pattern

## 1. Khái niệm

**Adapter** là một structural design pattern, chuyển đổi interface của một class đã có sẵn (Adaptee — thường là code cũ, thư viện ngoài, hoặc third-party mà không thể/không nên sửa) thành một interface khác (Target) mà nơi gọi (Client) đang mong đợi, giúp hai bên vốn không tương thích có thể làm việc được với nhau.

Pattern gồm 3 thành phần:

- **Target (interface):** interface mà Client đang sử dụng/mong đợi.
- **Adaptee:** class đã tồn tại, có interface khác, không tương thích trực tiếp với Target.
- **Adapter:** hiện thực `Target`, bên trong giữ tham chiếu tới `Adaptee` và "dịch" lời gọi từ Target sang lời gọi tương ứng trên Adaptee.

## 2. Ý nghĩa

- **Vấn đề gặp phải nếu không có Adapter:** hệ thống đang định nghĩa một interface chung (ví dụ `ILogger`, `IPaymentGateway`) để các nơi gọi phụ thuộc vào, nhưng một thư viện/API bên thứ ba lại có interface, tên method, kiểu tham số hoàn toàn khác và không thể sửa được (đóng gói sẵn, hoặc là code legacy không muốn động vào). Nếu ép Client gọi trực tiếp Adaptee, Client sẽ bị phụ thuộc vào interface "lạ" đó, phá vỡ tính đồng nhất và khó thay thế provider khác sau này.
- **Adapter giải quyết bằng cách chèn một lớp trung gian dịch interface:** Client vẫn chỉ làm việc với Target quen thuộc, Adapter chịu trách nhiệm gọi đúng method của Adaptee bên dưới và chuyển đổi tham số/kết quả qua lại.
- Cho phép **tái sử dụng code cũ hoặc thư viện ngoài** mà không cần sửa source của nó (nhiều trường hợp không có quyền sửa, ví dụ thư viện đóng gói dạng NuGet).
- Giữ cho tầng nghiệp vụ **chỉ phụ thuộc abstraction (Target)**, dễ dàng thay thế Adaptee khác (đổi third-party provider) mà không ảnh hưởng code Client, chỉ cần viết Adapter mới.

## 3. Code mẫu

### Ví dụ 1 — Target interface và Adaptee (thư viện ngoài có interface khác)

    // Target - interface mà hệ thống đang dùng
    public interface ISmsSender
    {
        void Send(string phoneNumber, string message);
    }

    // Adaptee - thư viện SMS của bên thứ 3, không thể sửa, interface khác hoàn toàn
    public class ThirdPartySmsClient
    {
        public void DeliverMessage(string content, string toNumber, bool isUrgent)
        {
            Console.WriteLine($"[ThirdParty] Gui '{content}' den {toNumber} (urgent={isUrgent})");
        }
    }

### Ví dụ 2 — Adapter dịch giữa Target và Adaptee

    public class ThirdPartySmsAdapter : ISmsSender
    {
        private readonly ThirdPartySmsClient _client;

        public ThirdPartySmsAdapter(ThirdPartySmsClient client)
        {
            _client = client;
        }

        public void Send(string phoneNumber, string message)
        {
            // Dịch lời gọi Send(phone, message) sang DeliverMessage(content, toNumber, isUrgent)
            _client.DeliverMessage(message, phoneNumber, isUrgent: false);
        }
    }

### Ví dụ 3 — Client chỉ phụ thuộc Target, không biết Adaptee tồn tại

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

    // Đăng ký DI: sau này đổi sang nhà cung cấp SMS khác chỉ cần viết Adapter mới,
    // OrderNotificationService không phải sửa gì.
    ISmsSender smsSender = new ThirdPartySmsAdapter(new ThirdPartySmsClient());
    var service = new OrderNotificationService(smsSender);
    service.NotifyOrderCreated("0900000000", "DH0001");
