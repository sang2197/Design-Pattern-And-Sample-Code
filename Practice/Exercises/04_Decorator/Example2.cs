namespace Exercises.Decorator.Example2;

// Decorator - Vi du 2: Dinh dang thong bao (them tien to, viet hoa) ma khong sua lop goc
// Xem lai: Decorator-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface INotifier (Component) { string Send(string message); }
// - class BasicNotifier : INotifier (ConcreteComponent) -> Send() tra ve nguyen message
// - abstract class NotifierDecorator : INotifier (Decorator)
//     protected readonly INotifier Inner; constructor nhan INotifier inner
//     virtual Send(message) => Inner.Send(message)
// - class PrefixDecorator : NotifierDecorator (ConcreteDecorator)
//     constructor nhan (INotifier inner, string prefix)
//     Send(message) -> "{prefix} {Inner.Send(message)}"
// - class UpperCaseDecorator : NotifierDecorator (ConcreteDecorator)
//     Send(message) -> Inner.Send(message).ToUpper()
