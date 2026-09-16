namespace Exercises.FactoryMethod.Example3;

// Factory Method - Vi du 3: Gui thong bao qua nhieu kenh (Email / SMS)
// Xem lai: Factory-Method-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface INotifier { string Send(string message); }
// - class EmailNotifier : INotifier -> Send() tra ve "[Email] {message}"
// - class SmsNotifier : INotifier -> Send() tra ve "[SMS] {message}"
// - abstract class NotificationService
//     protected abstract INotifier CreateNotifier();
//     public string Notify(string message) -> goi CreateNotifier() roi tra ve ket qua Send(message)
// - class EmailNotificationService : NotificationService -> CreateNotifier() tra ve EmailNotifier
// - class SmsNotificationService : NotificationService -> CreateNotifier() tra ve SmsNotifier
