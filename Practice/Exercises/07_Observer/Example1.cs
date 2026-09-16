namespace Exercises.Observer.Example1;

// Observer - Vi du 1: Don hang doi trang thai, nhieu noi can phan ung theo
// Xem lai: Observer-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IOrderObserver (Observer) { void Update(string orderId, string newStatus); }
// - class OrderSubject (Subject)
//     private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();
//     Subscribe(observer) -> them vao _observers
//     Unsubscribe(observer) -> bo khoi _observers
//     ChangeStatus(orderId, newStatus) -> goi Update(orderId, newStatus) tren tat ca observer trong _observers
// - class EmailNotifier : IOrderObserver (ConcreteObserver)
//     List<string> SentEmails { get; } = new List<string>();
//     Update() -> them vao SentEmails chuoi "Don hang {orderId} chuyen sang trang thai: {newStatus}"
// - class InventoryUpdater : IOrderObserver (ConcreteObserver)
//     List<string> RestockedOrders { get; } = new List<string>();
//     Update() -> neu newStatus == "Cancelled" thi them orderId vao RestockedOrders
