using System.ComponentModel.Design;

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

// Observer
public interface IWeatherObserver
{
    void Update(double temperature, double humidity);
}

// Concrete Observer
public class CurrentConditionsDisplay : IWeatherObserver
{
    public void Update(double temperature, double humidity)
    {
        Console.WriteLine($"[CurrentConditons] Nhiet do: {temperature}C, Do am: {humidity}%");
    }
}

public class StaticsDisplay: IWeatherObserver
{
    public List<double> _temperatures = new List<double>();

    public void Update(double temperature, double humidity)
    {
        _temperatures.Add(temperature);
        var _avg = _temperatures.Average();
        Console.WriteLine($"[Statics] Nhiet do trung binh: {_avg:0.0}C");
    }
}

// Subject
public class WeatherStation
{
    public List<IWeatherObserver> _observers = new List<IWeatherObserver>();

    public void Subscribe(IWeatherObserver observer)
    {
        _observers.Add(observer);
    }

    public void SetMeasurement(double temperature, double humility)
    {
        foreach(var observer in _observers)
        {
            observer.Update(temperature, humility);
        }
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var weatherStation = new WeatherStation();

        weatherStation.Subscribe(new CurrentConditionsDisplay());
        weatherStation.Subscribe(new StaticsDisplay());

        weatherStation.SetMeasurement(25, 60);
        weatherStation.SetMeasurement(27, 70);
    }
}