namespace Exercises.Adapter;

public interface ISmsSender
{
    void Send(string phoneNumber, string message);
}

public class ThirdPartySmsClient
{
    public List<string> SentLog { get; } = new List<string>();

    public void DeliverMessage(string content, string toNumber, bool isUrgent)
    {
        SentLog.Add($"{content}|{toNumber}|{isUrgent}");
    }
}

public class ThirdPartySmsAdapter : ISmsSender
{
    private readonly ThirdPartySmsClient _client;

    public ThirdPartySmsAdapter(ThirdPartySmsClient client)
    {
        _client = client;
    }

    // TODO: goi _client.DeliverMessage(...) voi tham so dung thu tu:
    // content = message, toNumber = phoneNumber, isUrgent = false
    public void Send(string phoneNumber, string message)
    {
        throw new NotImplementedException();
    }
}
