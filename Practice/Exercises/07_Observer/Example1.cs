using System.Security.Cryptography.X509Certificates;

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

// Observer
public interface IOrderObserver
{
    void Update(string orderId, string status);
}

// Concrete Observer
public class MailObserver : IOrderObserver
{
    public void Update(string orderId, string status)
    {
        Console.WriteLine($"Don hang {orderId} da chuyen trang thai {status}");
    }
}

public class InventoryObserver : IOrderObserver
{
    public void Update(string orderId, string status)
    {
        if(status == "Cancelled")
        {
            Console.WriteLine($"Cap nhat ton kho cho don hang {orderId}");
        }
    }
}

// Subject
public class OrderSubject
{
    private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

    public void Subscribe (IOrderObserver observer)
    {
        _observers.Add(observer);
    }

    public void ChangeStatus(string orderId, string status)
    {
        foreach(var observer in _observers)
        {
            observer.Update(orderId, status);
        }
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var order = new OrderSubject();

        order.Subscribe(new MailObserver());
        order.Subscribe(new InventoryObserver());

        order.ChangeStatus("DH0001", "Cancelled");
    }
}