namespace Exercises.Mediator;

public interface IChatMediator
{
    void Register(ChatUser user);
    void SendMessage(string message, ChatUser sender);
}

public abstract class ChatUser
{
    protected readonly IChatMediator Mediator;
    public string Name { get; }
    public List<string> ReceivedLog { get; } = new List<string>();

    protected ChatUser(IChatMediator mediator, string name)
    {
        Mediator = mediator;
        Name = name;
        mediator.Register(this);
    }

    public void Send(string message)
    {
        Mediator.SendMessage(message, this);
    }

    public abstract void Receive(string message, string senderName);
}

public class ConsoleChatUser : ChatUser
{
    public ConsoleChatUser(IChatMediator mediator, string name) : base(mediator, name)
    {
    }

    // TODO: them vao ReceivedLog chuoi "{senderName}: {message}"
    public override void Receive(string message, string senderName)
    {
        throw new NotImplementedException();
    }
}

public class ChatRoom : IChatMediator
{
    private readonly List<ChatUser> _users = new List<ChatUser>();

    // TODO: them user vao _users
    public void Register(ChatUser user)
    {
        throw new NotImplementedException();
    }

    // TODO: goi Receive(message, sender.Name) tren tat ca user trong _users,
    // NGOAI TRU chinh sender (khong tu gui cho minh)
    public void SendMessage(string message, ChatUser sender)
    {
        throw new NotImplementedException();
    }
}
