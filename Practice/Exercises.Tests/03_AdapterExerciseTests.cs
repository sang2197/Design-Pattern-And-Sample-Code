using Exercises.Adapter;
using Xunit;

namespace Exercises.Tests;

public class AdapterExerciseTests
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
}
