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
    void OnPriceChanged(string symbol, decimal price);
}

// Concrete Observer
public class Investor : IStockObserver
{
    public string Name { get; set; }

    public Investor(string name)
    {
        Name = name;
    }

    public void OnPriceChanged(string symbol, decimal price)
    {
        Console.WriteLine($"[{Name}] {symbol} vua doi gia thanh: {price:N0}");
    }
}

// Subject
public class StockSubject
{
    private readonly List<IStockObserver> _observer = new List<IStockObserver>();
    public string _symbol;
    public decimal _price;

    public StockSubject(string symbol, decimal price)
    {
        _symbol = symbol;
        _price = price;
    }

    public void Subscribe(IStockObserver observer)
    {
        _observer.Add(observer);
    }

    public void SetPrice(decimal newPrice)
    {
        _price = newPrice;
        foreach(var observer in _observer)
        {
            observer.OnPriceChanged(_symbol, _price);
        }
    }
}

// Cách sử dụng
public class Program
{
    public static void Main()
    {
        StockSubject stockSubject = new StockSubject("VMN", 52000);

        stockSubject.Subscribe(new Investor("Lan"));
        stockSubject.Subscribe(new Investor("Nam"));

        stockSubject.SetPrice(60000);
    }
}