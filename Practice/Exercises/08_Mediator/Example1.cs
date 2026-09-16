namespace Exercises.Mediator.Example1;

// Mediator - Vi du 1: Phong chat (nhieu User giao tiep qua ChatRoom)
// Xem lai: Mediator.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IChatMediator (Mediator) { void Register(ChatUser user); void SendMessage(string message, ChatUser sender); }
// - abstract class ChatUser (Colleague)
//     protected readonly IChatMediator Mediator; string Name { get; }; List<string> ReceivedLog { get; } = new List<string>();
//     constructor nhan (IChatMediator mediator, string name) -> gan Mediator, Name, roi goi mediator.Register(this)
//     Send(message) -> goi Mediator.SendMessage(message, this)
//     abstract Receive(string message, string senderName)
// - class ConsoleChatUser : ChatUser (ConcreteColleague)
//     Receive() -> them vao ReceivedLog chuoi "{senderName}: {message}"
// - class ChatRoom : IChatMediator (ConcreteMediator)
//     private readonly List<ChatUser> _users = new List<ChatUser>();
//     Register(user) -> them vao _users
//     SendMessage(message, sender) -> goi Receive(message, sender.Name) tren tat ca user trong _users
//       NGOAI TRU chinh sender (khong tu gui cho minh)
