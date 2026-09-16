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
