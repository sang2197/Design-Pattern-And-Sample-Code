# Adapter Pattern

## 1. Khái niệm cơ bản

**Adapter** là một Structural Design Pattern, dùng để chuyển interface của một class có sẵn (`Adaptee`) thành interface mà `Client` đang mong đợi (`Target`), giúp hai thành phần có interface không tương thích vẫn làm việc được với nhau mà không cần sửa code gốc của `Adaptee`.

Pattern gồm 3 thành phần:

- **Target (interface):** interface mà Client mong muốn sử dụng.
- **Adaptee:** class đã tồn tại nhưng có interface không tương thích với Target, thường là code legacy hoặc thư viện bên thứ ba.
- **Adapter:** hiện thực `Target`, giữ tham chiếu tới `Adaptee` và chuyển lời gọi từ Target sang lời gọi phù hợp trên Adaptee.

## 2. Bài toán

Hệ thống đang sử dụng một interface chung như `ILogger` hoặc `IPaymentGateway`, nhưng thư viện bên thứ ba (hoặc code legacy) lại có tên method, thứ tự tham số, thậm chí đơn vị dữ liệu hoàn toàn khác. Không thể sửa trực tiếp thư viện đó (đóng gói sẵn, hoặc là code cũ không muốn động vào).

Nếu Client gọi thẳng vào thư viện/code cũ đó, Client sẽ bị **phụ thuộc vào interface riêng của provider**, phá vỡ tính đồng nhất mà hệ thống đang cố duy trì, và khó thay thế provider khác về sau.

## 3. Ý nghĩa của Adapter

- **Adapter giải quyết bằng cách tạo một lớp trung gian:** Client vẫn chỉ làm việc với `Target`, còn Adapter chịu trách nhiệm chuyển đổi method, tham số hoặc kết quả để gọi đúng `Adaptee`.
- Cho phép **tái sử dụng code cũ hoặc thư viện bên ngoài** mà không cần sửa source của chúng.
- Giúp Client **chỉ phụ thuộc vào abstraction (`Target`)**, nên khi đổi provider chỉ cần tạo Adapter mới mà không phải sửa logic nghiệp vụ.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: nêu rõ khi nào nên dùng, có đủ Target/Adaptee/Adapter, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Gửi SMS qua thư viện bên thứ ba

**Khi nào dùng:** hệ thống đã định nghĩa sẵn interface `ISmsSender`, nhưng thư viện SMS thật (`ThirdPartySmsClient`) lại có tên method và tham số hoàn toàn khác, không thể sửa vì đây là thư viện đóng gói sẵn.

**Cách sử dụng:** Client (`OrderNotificationService`) chỉ inject `ISmsSender`, không biết bên dưới là `ThirdPartySmsAdapter` hay `ThirdPartySmsClient`.

**Cách hiện thực:** `ThirdPartySmsAdapter` hiện thực `ISmsSender.Send()`, bên trong gọi `_client.DeliverMessage()` với tham số đã sắp xếp lại đúng thứ tự Adaptee yêu cầu.

```csharp
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

// Adapter
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

// Client - chỉ phụ thuộc Target, không biết Adaptee tồn tại
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

// Chạy thử - cách sử dụng: bọc Adaptee bên trong Adapter rồi truyền vào Client qua Target
public class Program
{
    public static void Main()
    {
        ISmsSender smsSender = new ThirdPartySmsAdapter(new ThirdPartySmsClient());
        var service = new OrderNotificationService(smsSender);

        service.NotifyOrderCreated("0900000000", "DH0001");
    }
}
// [ThirdParty] Gui 'Don hang DH0001 da duoc tao thanh cong' den 0900000000 (urgent=False)
```

### Ví dụ 2 — Ghi log qua thư viện XML cũ (legacy)

**Khi nào dùng:** hệ thống mới chuẩn hoá theo interface `ILogger`, nhưng module cũ (`LegacyXmlLogger`) chỉ biết ghi log dạng XML và không còn ai bảo trì để sửa lại interface của nó.

**Cách sử dụng:** mọi nơi cần ghi log chỉ phụ thuộc `ILogger.Log()`, không quan tâm bên dưới đang ghi ra XML hay định dạng nào khác.

**Cách hiện thực:** `XmlLoggerAdapter.Log()` chuyển tiếp thẳng sang `_legacyLogger.WriteXmlLog()` — trường hợp đơn giản nhất của Adapter khi tham số không cần biến đổi nhiều, chỉ đổi tên method.

```csharp
// Target
public interface ILogger
{
    void Log(string message);
}

// Adaptee - module cũ, chỉ biết ghi XML
public class LegacyXmlLogger
{
    public void WriteXmlLog(string xmlContent)
    {
        Console.WriteLine($"[LegacyXmlLogger] <log>{xmlContent}</log>");
    }
}

// Adapter
public class XmlLoggerAdapter : ILogger
{
    private readonly LegacyXmlLogger _legacyLogger;

    public XmlLoggerAdapter(LegacyXmlLogger legacyLogger)
    {
        _legacyLogger = legacyLogger;
    }

    public void Log(string message)
    {
        _legacyLogger.WriteXmlLog(message);
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        ILogger logger = new XmlLoggerAdapter(new LegacyXmlLogger());
        logger.Log("Ung dung da khoi dong");
    }
}
// [LegacyXmlLogger] <log>Ung dung da khoi dong</log>
```

### Ví dụ 3 — Thanh toán qua SDK nước ngoài (khác đơn vị tính)

**Khi nào dùng:** hệ thống dùng `IPaymentGateway.Pay(decimal amount)` tính theo đơn vị tiền tệ thông thường, nhưng SDK cổng thanh toán nước ngoài (`ForeignPaymentSdk`) lại yêu cầu số tiền tính theo **cent** (số nguyên) kèm mã tiền tệ — hai bên không chỉ khác tên method mà còn khác cả đơn vị dữ liệu.

**Cách sử dụng:** Client gọi `gateway.Pay(19.99m)` như với bất kỳ `IPaymentGateway` nào khác, không cần biết việc quy đổi sang cent xảy ra ở đâu.

**Cách hiện thực:** `ForeignPaymentAdapter.Pay()` tự quy đổi `amount` (decimal, đơn vị tiền tệ) sang `amountInCents` (int, đơn vị cent) trước khi gọi `_sdk.Charge()` — minh hoạ việc Adapter không chỉ đổi tên method mà còn có thể đổi cả kiểu/đơn vị dữ liệu.

```csharp
// Target
public interface IPaymentGateway
{
    string Pay(decimal amount);
}

// Adaptee - SDK cua nha cung cap nuoc ngoai, tinh theo cent
public class ForeignPaymentSdk
{
    public string Charge(int amountInCents, string currency)
    {
        return $"[ForeignSDK] Charged {amountInCents} {currency} cents";
    }
}

// Adapter - chuyen doi don vi tien va goi dung method cua Adaptee
public class ForeignPaymentAdapter : IPaymentGateway
{
    private readonly ForeignPaymentSdk _sdk;

    public ForeignPaymentAdapter(ForeignPaymentSdk sdk)
    {
        _sdk = sdk;
    }

    public string Pay(decimal amount)
    {
        int amountInCents = (int)(amount * 100);
        return _sdk.Charge(amountInCents, "USD");
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        IPaymentGateway gateway = new ForeignPaymentAdapter(new ForeignPaymentSdk());
        Console.WriteLine(gateway.Pay(19.99m));
    }
}
// [ForeignSDK] Charged 1999 USD cents
```

Trong cả 3 ví dụ, nếu sau này đổi sang nhà cung cấp khác (SMS, logging, payment provider), chỉ cần viết Adapter mới hiện thực đúng Target — Client (`OrderNotificationService`, code gọi `ILogger`/`IPaymentGateway`...) không cần sửa gì.