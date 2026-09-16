using Exercises.Adapter.Example1;
using Xunit;

namespace Adapter.Tests;

public class Example1Tests
{
    [Fact]
    public void Send_DelegatesToDeliverMessage_WithSwappedArguments()
    {
        var client = new ThirdPartySmsClient();
        ISmsSender sender = new ThirdPartySmsAdapter(client);

        sender.Send("0900000000", "Xin chao");

        Assert.Single(client.SentLog);
        Assert.Equal("Xin chao|0900000000|False", client.SentLog[0]);
    }

    [Fact]
    public void OrderNotificationService_NotifyOrderCreated_SendsFormattedMessage()
    {
        var client = new ThirdPartySmsClient();
        var service = new OrderNotificationService(new ThirdPartySmsAdapter(client));

        service.NotifyOrderCreated("0900000000", "DH0001");

        Assert.Equal("Don hang DH0001 da duoc tao thanh cong|0900000000|False", client.SentLog[0]);
    }
}
