namespace Exercises.Adapter.Example1;

// Adapter - Vi du 1: Gui SMS qua thu vien ben thu ba
// Xem lai: Adapter-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ISmsSender (Target) { void Send(string phoneNumber, string message); }
// - class ThirdPartySmsClient (Adaptee)
//     List<string> SentLog { get; } = new List<string>()
//     void DeliverMessage(string content, string toNumber, bool isUrgent)
//       -> them vao SentLog chuoi "{content}|{toNumber}|{isUrgent}"
// - class ThirdPartySmsAdapter : ISmsSender (Adapter)
//     constructor nhan ThirdPartySmsClient
//     Send(phoneNumber, message) -> goi _client.DeliverMessage(message, phoneNumber, isUrgent: false)
//       (chu y: dao thu tu tham so giua Send va DeliverMessage)
// - class OrderNotificationService (Client)
//     constructor nhan ISmsSender
//     NotifyOrderCreated(phoneNumber, orderId) -> goi _smsSender.Send(phoneNumber, "Don hang {orderId} da duoc tao thanh cong")

// Target
public interface ISmsSender
{
    void Send(string phoneNumber, string message);
}

// Adaptee
public class ThirdPartySmsClient
{
    public void SendMessage(string content, string toNumber, bool isUrgent)
    {
        Console.WriteLine($"[ThirdPartySms] Da gui {content} toi {toNumber} (Urgent: {isUrgent})");
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
        _client.SendMessage(message, phoneNumber, false);
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        ISmsSender smsSender = new ThirdPartySmsAdapter(new ThirdPartySmsClient());
        smsSender.Send("0966113344", "Hoan thanh don hang");
    }
}
