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

// Product
public interface ITransport
{
    string Deliver();
}

// Concrete Product
public class Ship : ITransport
{
    public string Deliver() => "Da van chuyen bang thuyen";
}

public class Truck : ITransport
{
    public string Deliver() => "Da van chuyen bang xe tai";
}

// Creator
public abstract class TransportProcessor
{
    protected abstract ITransport CreateTransport();

    public string Delivery()
    {
        var transport = CreateTransport();
        return transport.Deliver();
    }
}

// Concrete Creator
public class TruckTransportProcessor : TransportProcessor
{
    protected override ITransport CreateTransport() => new Truck();
}

public class ShipTransportProcessor : TransportProcessor
{
    protected override ITransport CreateTransport() => new Ship();
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var truck = new TruckTransportProcessor();
        var ship = new ShipTransportProcessor();

        Console.WriteLine(truck.Delivery());
        Console.WriteLine(ship.Delivery());
    }
}