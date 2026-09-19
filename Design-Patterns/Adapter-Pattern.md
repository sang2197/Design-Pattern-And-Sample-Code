# Adapter Pattern

## 1. Khái niệm cơ bản

**Adapter** là một Structural Design Pattern, dùng để chuyển interface của một class có sẵn (`Adaptee`) thành interface mà `Client` đang mong đợi (`Target`), giúp hai thành phần có interface không tương thích vẫn làm việc được với nhau mà không cần sửa code gốc của `Adaptee`.

Pattern gồm 3 thành phần:

- **Target (interface):** interface mà Client mong muốn sử dụng.
- **Adaptee:** class đã tồn tại nhưng có interface không tương thích với Target, thường là code legacy hoặc thư viện bên thứ ba.
- **Adapter:** hiện thực `Target`, giữ tham chiếu tới `Adaptee` và chuyển lời gọi từ Target sang lời gọi phù hợp trên Adaptee.

## 2. Khi nào nên dùng

Dùng khi:

✅ Interface hiện có không tương thích với interface hệ thống đang cần

✅ Không thể (hoặc không muốn) sửa trực tiếp code của thư viện/class cũ

✅ Muốn tái sử dụng lại class cũ/thư viện bên ngoài mà không viết lại từ đầu

✅ Muốn Client chỉ phụ thuộc vào một interface chung, dễ đổi provider sau này

## 3. Code examples

### Ví dụ 1 — Gửi SMS qua thư viện bên thứ ba

**Bài toán:** Hệ thống đã chuẩn hoá việc gửi SMS thông qua interface `ISmsSender`, nhưng SMS thực tế lại được gửi qua một thư viện bên thứ ba (`ThirdPartySmsClient`) với tên method và thứ tự tham số hoàn toàn khác (`DeliverMessage(content, toNumber, isUrgent)` thay vì `Send(phoneNumber, message)`). Đây là thư viện đóng gói sẵn nên không thể sửa trực tiếp. Nếu để `OrderNotificationService` gọi thẳng vào `ThirdPartySmsClient`, class này sẽ phụ thuộc vào interface riêng của nhà cung cấp, phá vỡ chuẩn `ISmsSender` mà hệ thống đang dùng và khiến việc đổi sang nhà cung cấp SMS khác về sau trở nên khó khăn.

**Ý nghĩa của Adapter trong ví dụ này:** `ThirdPartySmsAdapter` hiện thực `ISmsSender.Send()`, bên trong giữ tham chiếu tới `ThirdPartySmsClient` và gọi `_client.DeliverMessage()` với tham số đã được sắp xếp lại đúng thứ tự mà Adaptee yêu cầu. Nhờ vậy, `OrderNotificationService` (Client) chỉ inject `ISmsSender`, hoàn toàn không biết bên dưới đang chạy `ThirdPartySmsAdapter` hay `ThirdPartySmsClient`. Sau này nếu đổi sang nhà cung cấp SMS khác, chỉ cần viết một Adapter mới hiện thực `ISmsSender` mà không phải sửa `OrderNotificationService`.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        ISmsSender smsSender = new ThirdPartySmsAdapter(new ThirdPartySmsClient());
        var service = new OrderNotificationService(smsSender);

        service.NotifyOrderCreated("0900000000", "DH0001");
        // [ThirdParty] Gui 'Don hang DH0001 da duoc tao thanh cong' den 0900000000 (urgent=False)
    }
}
```

### Ví dụ 2 — Ghi log qua thư viện XML cũ (legacy)

**Bài toán:** Hệ thống mới đã chuẩn hoá việc ghi log thông qua interface `ILogger`, nhưng module ghi log cũ (`LegacyXmlLogger`) chỉ biết ghi ra định dạng XML thông qua method `WriteXmlLog()` và không còn ai bảo trì để sửa lại interface của nó. Việc viết lại toàn bộ `LegacyXmlLogger` là không khả thi vì rủi ro cao và tốn công sức, trong khi để mọi nơi trong hệ thống gọi trực tiếp `WriteXmlLog()` sẽ khiến code phụ thuộc vào một API cũ, không đồng nhất với chuẩn `ILogger` mà phần còn lại của hệ thống đang dùng.

**Ý nghĩa của Adapter trong ví dụ này:** `XmlLoggerAdapter` hiện thực `ILogger.Log()` và bên trong chuyển tiếp thẳng sang `_legacyLogger.WriteXmlLog()`. Đây là trường hợp đơn giản của Adapter khi tham số không cần biến đổi nhiều, chỉ cần đổi tên method cho khớp với `Target`. Nhờ có `XmlLoggerAdapter`, mọi nơi cần ghi log chỉ cần phụ thuộc `ILogger.Log()`, không quan tâm bên dưới đang ghi ra XML hay định dạng nào khác, và có thể thay `LegacyXmlLogger` bằng một hệ thống log mới sau này chỉ bằng cách viết Adapter khác.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        ILogger logger = new XmlLoggerAdapter(new LegacyXmlLogger());
        logger.Log("Ung dung da khoi dong");
        // [LegacyXmlLogger] <log>Ung dung da khoi dong</log>
    }
}
```

### Ví dụ 3 — Thanh toán qua SDK nước ngoài (khác đơn vị tính)

**Bài toán:** Hệ thống thanh toán nội bộ dùng `IPaymentGateway.Pay(decimal amount)` với số tiền tính theo đơn vị tiền tệ thông thường, nhưng SDK cổng thanh toán nước ngoài (`ForeignPaymentSdk`) lại yêu cầu gọi `Charge(int amountInCents, string currency)`, nhận số tiền dưới dạng số nguyên tính theo **cent** kèm mã tiền tệ. Hai bên không chỉ khác tên method mà còn khác cả kiểu dữ liệu và đơn vị tính, nên client không thể gọi thẳng SDK này mà không có bước quy đổi, và cũng không thể sửa SDK vì đây là thư viện của bên thứ ba.

**Ý nghĩa của Adapter trong ví dụ này:** `ForeignPaymentAdapter` hiện thực `IPaymentGateway.Pay()`, tự quy đổi `amount` (decimal, đơn vị tiền tệ) sang `amountInCents` (int, đơn vị cent) rồi mới gọi `_sdk.Charge()` với mã tiền tệ cố định. Nhờ đó, Client chỉ cần gọi `gateway.Pay(19.99m)` như với bất kỳ `IPaymentGateway` nào khác, không cần biết việc quy đổi sang cent xảy ra ở đâu. Ví dụ này minh hoạ rằng Adapter không chỉ đổi tên method như hai ví dụ trước, mà còn có thể đổi cả kiểu dữ liệu/đơn vị đo, và nếu sau này đổi sang một nhà cung cấp thanh toán khác, chỉ cần viết Adapter mới hiện thực `IPaymentGateway` mà không phải sửa code gọi thanh toán hiện có.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IPaymentGateway gateway = new ForeignPaymentAdapter(new ForeignPaymentSdk());
        Console.WriteLine(gateway.Pay(19.99m));
        // [ForeignSDK] Charged 1999 USD cents
    }
}
```
