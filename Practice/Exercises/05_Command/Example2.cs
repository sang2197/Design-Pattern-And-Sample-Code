namespace Exercises.Command.Example2;

// Command - Vi du 2: Undo khi soan thao van ban
// Xem lai: Command-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ICommand (Command) { void Execute(); void Undo(); }
// - class TextDocument (Receiver)
//     string Content { get; private set; } = ""
//     InsertText(string text, int position) -> Content = Content.Insert(position, text)
//     DeleteText(int position, int length) -> Content = Content.Remove(position, length)
// - class InsertTextCommand : ICommand (ConcreteCommand)
//     constructor nhan (TextDocument document, string text, int position)
//     Execute() -> goi _document.InsertText(_text, _position)
//     Undo() -> goi _document.DeleteText(_position, _text.Length)
// - class CommandHistory (Invoker)
//     private readonly Stack<ICommand> _history = new Stack<ICommand>();
//     ExecuteCommand(ICommand command) -> goi command.Execute() roi push vao _history
//     Undo() -> neu _history rong thi return; nguoc lai pop 1 command va goi Undo() cua no
