using Exercises.Mediator;
using Xunit;

namespace Exercises.Tests;

public class MediatorExerciseTests
{
    [Fact]
    public void Send_DeliversMessageToOtherRegisteredUsers_NotToSender()
    {
        var chatRoom = new ChatRoom();
        var alice = new ConsoleChatUser(chatRoom, "Alice");
        var bob = new ConsoleChatUser(chatRoom, "Bob");

        alice.Send("Chao Bob");

        Assert.Single(bob.ReceivedLog);
        Assert.Equal("Alice: Chao Bob", bob.ReceivedLog[0]);
        Assert.Empty(alice.ReceivedLog);
    }

    [Fact]
    public void Send_DeliversToAllOtherUsers_WhenMoreThanTwoRegistered()
    {
        var chatRoom = new ChatRoom();
        var alice = new ConsoleChatUser(chatRoom, "Alice");
        var bob = new ConsoleChatUser(chatRoom, "Bob");
        var carol = new ConsoleChatUser(chatRoom, "Carol");

        alice.Send("Xin chao");

        Assert.Single(bob.ReceivedLog);
        Assert.Single(carol.ReceivedLog);
    }
}
