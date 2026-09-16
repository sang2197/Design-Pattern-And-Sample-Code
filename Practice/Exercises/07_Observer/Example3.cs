namespace Exercises.Observer.Example3;

// Observer - Vi du 3: Tram thoi tiet, nhieu man hinh hien thi khac nhau
// Xem lai: Observer-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau (can using System.Linq cho Average()):
//
// - interface IWeatherObserver (Observer) { void Update(double temperature, double humidity); }
// - class WeatherStation (Subject)
//     private readonly List<IWeatherObserver> _observers = new List<IWeatherObserver>();
//     Subscribe(observer) -> them vao _observers
//     SetMeasurements(temperature, humidity) -> goi Update(temperature, humidity) tren tat ca observer
// - class CurrentConditionsDisplay : IWeatherObserver (ConcreteObserver)
//     double LastTemperature { get; private set; }; double LastHumidity { get; private set; }
//     Update() -> gan LastTemperature/LastHumidity theo gia tri moi nhat
// - class StatisticsDisplay : IWeatherObserver (ConcreteObserver)
//     private readonly List<double> _temperatures = new List<double>();
//     Update() -> them temperature vao _temperatures
//     double AverageTemperature => neu rong tra ve 0, nguoc lai tra ve _temperatures.Average()
