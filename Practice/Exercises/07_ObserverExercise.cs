namespace Exercises.Observer;

public interface IOrderObserver
{
    void Update(string orderId, string newStatus);
}

public class OrderSubject
{
    private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

    // TODO: them observer vao _observers
    public void Subscribe(IOrderObserver observer)
    {
        throw new NotImplementedException();
    }

    // TODO: bo observer khoi _observers
    public void Unsubscribe(IOrderObserver observer)
    {
        throw new NotImplementedException();
    }

    // TODO: goi Update(orderId, newStatus) tren tat ca observer trong _observers
    public void ChangeStatus(string orderId, string newStatus)
    {
        throw new NotImplementedException();
    }
}

public class EmailNotifier : IOrderObserver
{
    public List<string> SentEmails { get; } = new List<string>();

    // TODO: them vao SentEmails chuoi "Don hang {orderId} chuyen sang trang thai: {newStatus}"
    public void Update(string orderId, string newStatus)
    {
        throw new NotImplementedException();
    }
}

public class InventoryUpdater : IOrderObserver
{
    public List<string> RestockedOrders { get; } = new List<string>();

    // TODO: neu newStatus == "Cancelled" thi them orderId vao RestockedOrders
    public void Update(string orderId, string newStatus)
    {
        throw new NotImplementedException();
    }
}
