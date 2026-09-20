namespace Exercises.Observer.Example2;

// Observer - Vi du 2: Gia co phieu thay doi, nhieu nha dau tu theo doi cung luc
// Xem lai: Observer-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IStockObserver (Observer) { void OnPriceChanged(string symbol, decimal price); }
// - class Stock (Subject)
//     private readonly List<IStockObserver> _observers = new List<IStockObserver>();
//     string Symbol { get; }; private decimal _price;
//     constructor nhan (string symbol, decimal initialPrice)
//     Subscribe(observer) -> them vao _observers
//     SetPrice(newPrice) -> gan _price roi goi OnPriceChanged(Symbol, _price) tren tat ca observer
// - class Investor : IStockObserver (ConcreteObserver)
//     string Name { get; }; List<string> Notifications { get; } = new List<string>();
//     constructor nhan string name
//     OnPriceChanged() -> them vao Notifications chuoi "{symbol} vua doi gia: {price:N0}"

// Observer
public interface IStockObserver
{
    void Change(string symbol, decimal price);
}

// Concrete
public class Investor : IStockObserver
{
    public string Name;
    public Investor(string name)
    {
        Name = name;
    }

    public void Change(string symbol, decimal price)
    {
        Console.WriteLine($"[{Name}] {symbol} Da cap nhat gia: {price}");
    }
}

// Subject
public class StockSubject
{
    private string _symbol;
    private decimal _price;
    private readonly List<IStockObserver> _observers = new List<IStockObserver>();

    public StockSubject(string symbol, decimal price)
    {
        _symbol = symbol;
        _price = price;
    }

    public void Subscribe(IStockObserver observer)
    {
        _observers.Add(observer);
    }

    public void SetPrice(decimal newPrice)
    {
        _price = newPrice;
        foreach(var observer in _observers)
        {
            observer.Change(_symbol, _price);
        }
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var stock = new StockSubject("VNM", 80000);

        stock.Subscribe(new Investor("Nam"));
        stock.Subscribe(new Investor("Lan"));

        stock.SetPrice(82000);
    }
}