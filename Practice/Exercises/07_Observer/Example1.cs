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

//Observer
public interface IOrderObserver
{
    void Update(string orderId, string newStatus);
}

//Concrete Observer
public class EmailNotifier : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        Console.WriteLine($"[Email] don hang {orderId} chuyen sang trang thai: {newStatus}");
    }
}

public class InventoryUpdate : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        if(newStatus == "Cancelled")
        {
            Console.WriteLine($"[Inventory] Hoan tra ton kho cho don hang {orderId}");
        }
    }
}

// Subject
public class OrderSubject
{
    private readonly List<IOrderObserver> _observer = new List<IOrderObserver>();

    public void Subscribe(IOrderObserver observer)
    {
        _observer.Add(observer);
    } 

    public void UnSubscribe(IOrderObserver observer)
    {
        _observer.Remove(observer);
    }

    public void ChangerStatus(string orderId, string newStatus)
    {
        foreach(var observer in _observer)
        {
            observer.Update(orderId, newStatus);
        }
    }
}

// Cách sử dụng
public class Program
{
    public static void Main()
    {
        OrderSubject orderSubject = new OrderSubject();

        orderSubject.Subscribe(new EmailNotifier());
        orderSubject.Subscribe(new InventoryUpdate());

        orderSubject.ChangerStatus("HD0001", "Cancelled");
    }
}