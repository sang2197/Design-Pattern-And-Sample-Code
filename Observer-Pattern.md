# Observer Pattern

## 1. Khái niệm cơ bản

**Observer** là một Behavioral Design Pattern, định nghĩa cơ chế **một-nhiều (one-to-many)** giữa các đối tượng: khi trạng thái của một đối tượng (Subject) thay đổi, tất cả đối tượng phụ thuộc vào nó (Observer) đã đăng ký sẽ **tự động được thông báo và cập nhật**, mà Subject không cần biết chi tiết từng Observer là ai, xử lý như thế nào.

Pattern gồm 4 thành phần:

- **Subject (Observable):** giữ danh sách các Observer đã đăng ký, cung cấp method `Subscribe()`/`Unsubscribe()`, và gọi `Notify()` tới tất cả Observer khi trạng thái thay đổi.
- **Observer (interface):** khai báo method (ví dụ `Update()`) mà Subject sẽ gọi khi có sự kiện.
- **ConcreteSubject:** hiện thực cụ thể, chứa trạng thái nghiệp vụ và phát sự kiện khi trạng thái đổi.
- **ConcreteObserver:** hiện thực cụ thể `Observer`, chứa logic phản ứng khi nhận được thông báo.

## 2. Bài toán

Khi một sự kiện xảy ra (ví dụ đơn hàng đổi trạng thái) cần nhiều nơi khác phản ứng theo (gửi email, gửi SMS, cập nhật tồn kho, ghi log...), cách làm trực tiếp là gọi tuần tự từng service ngay trong method xử lý sự kiện đó:

- Subject khi ấy phải **biết và phụ thuộc trực tiếp** vào từng lớp xử lý cụ thể.
- Mỗi lần thêm một hành động phản ứng mới lại phải **sửa vào đúng method đó**, vi phạm Open/Closed Principle và khiến class ngày càng phình to.
- Khó **test riêng từng phần phản ứng**, vì tất cả bị gói chung trong một method.

## 3. Ý nghĩa của Observer

- **Observer giải quyết bằng cách đảo ngược chiều phụ thuộc:** Subject chỉ biết interface `Observer` chung, không biết có bao nhiêu Observer hay chúng làm gì; các Observer tự đăng ký lắng nghe vào Subject. Khi trạng thái đổi, Subject chỉ cần gọi `Notify()`, tất cả Observer đã đăng ký tự động được gọi.
- Tuân thủ **Open/Closed Principle**: thêm một hành động phản ứng mới khi sự kiện xảy ra chỉ cần thêm 1 ConcreteObserver và đăng ký nó, không sửa code Subject.
- Cho phép **thêm/bớt Observer linh hoạt tại runtime** (subscribe/unsubscribe), tách rời hoàn toàn logic phát sinh sự kiện (Subject) khỏi logic xử lý sự kiện (từng Observer) — đây cũng là nền tảng cho cơ chế event/delegate trong .NET.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: nêu rõ khi nào nên dùng, có đủ Subject/Observer/ConcreteSubject/ConcreteObserver, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Đơn hàng đổi trạng thái, nhiều nơi cần phản ứng theo

**Khi nào dùng:** khi đơn hàng bị hủy, cần đồng thời gửi email và hoàn trả tồn kho — hai hành động độc lập, có thể có thêm hành động khác trong tương lai (SMS, ghi log...) mà không muốn sửa vào nơi phát sự kiện.

**Cách sử dụng:** đăng ký các `IOrderObserver` cần thiết qua `Subscribe()`, sau đó chỉ cần gọi `orderSubject.ChangeStatus(...)` một lần duy nhất — mọi Observer đã đăng ký tự động được gọi.

**Cách hiện thực:** `OrderSubject.ChangeStatus()` duyệt qua danh sách `_observers` và gọi `Update()` trên từng phần tử; bản thân Subject không có logic riêng cho từng loại phản ứng.

```csharp
// Observer
public interface IOrderObserver
{
    void Update(string orderId, string newStatus);
}

// Subject
public class OrderSubject
{
    private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

    public void Subscribe(IOrderObserver observer)
    {
        _observers.Add(observer);
    }

    public void Unsubscribe(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    public void ChangeStatus(string orderId, string newStatus)
    {
        // Subject không biết và không quan tâm từng Observer xử lý ra sao
        foreach (var observer in _observers)
        {
            observer.Update(orderId, newStatus);
        }
    }
}

// ConcreteObserver
public class EmailNotifier : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        Console.WriteLine($"[Email] Don hang {orderId} chuyen sang trang thai: {newStatus}");
    }
}

public class InventoryUpdater : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        if (newStatus == "Cancelled")
        {
            Console.WriteLine($"[Inventory] Hoan tra ton kho cho don hang {orderId}");
        }
    }
}

// Chạy thử - cách sử dụng: đăng ký Observer rồi để Subject tự thông báo
public class Program
{
    public static void Main()
    {
        var orderSubject = new OrderSubject();

        orderSubject.Subscribe(new EmailNotifier());
        orderSubject.Subscribe(new InventoryUpdater());

        orderSubject.ChangeStatus("DH0001", "Cancelled");
    }
}
// [Email] Don hang DH0001 chuyen sang trang thai: Cancelled
// [Inventory] Hoan tra ton kho cho don hang DH0001
```

### Ví dụ 2 — Giá cổ phiếu thay đổi, nhiều nhà đầu tư theo dõi cùng lúc

**Khi nào dùng:** nhiều nhà đầu tư (Investor) muốn được báo ngay khi giá một mã cổ phiếu thay đổi. Số lượng người theo dõi thay đổi liên tục (đăng ký/hủy đăng ký), `Stock` không thể biết trước danh sách này khi viết code.

**Cách sử dụng:** mỗi `Investor` gọi `stock.Subscribe(this)` để bắt đầu theo dõi; khi giá đổi (`SetPrice()`), mọi Investor đã subscribe đều nhận được `OnPriceChanged()`.

**Cách hiện thực:** giống cấu trúc Ví dụ 1, nhưng Subject (`Stock`) lần này còn giữ thêm trạng thái nghiệp vụ (`_price`) — minh hoạ ConcreteSubject không chỉ đơn thuần là nơi phát sự kiện mà thường có dữ liệu thật đi kèm.

```csharp
// Observer
public interface IStockObserver
{
    void OnPriceChanged(string symbol, decimal price);
}

// Subject
public class Stock
{
    private readonly List<IStockObserver> _observers = new List<IStockObserver>();
    public string Symbol { get; }
    private decimal _price;

    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void Subscribe(IStockObserver observer)
    {
        _observers.Add(observer);
    }

    public void SetPrice(decimal newPrice)
    {
        _price = newPrice;
        foreach (var observer in _observers)
        {
            observer.OnPriceChanged(Symbol, _price);
        }
    }
}

// ConcreteObserver
public class Investor : IStockObserver
{
    public string Name { get; }

    public Investor(string name)
    {
        Name = name;
    }

    public void OnPriceChanged(string symbol, decimal price)
    {
        Console.WriteLine($"[{Name}] {symbol} vua doi gia: {price:N0}");
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var stock = new Stock("VNM", 80000);

        stock.Subscribe(new Investor("Nam"));
        stock.Subscribe(new Investor("Lan"));

        stock.SetPrice(82000);
    }
}
// [Nam] VNM vua doi gia: 82,000
// [Lan] VNM vua doi gia: 82,000
```

### Ví dụ 3 — Trạm thời tiết, nhiều màn hình hiển thị khác nhau

**Khi nào dùng:** ví dụ kinh điển của Observer (sách Head First Design Patterns) — một `WeatherStation` đo được nhiệt độ/độ ẩm mới, cần cập nhật đồng thời nhiều loại màn hình hiển thị khác nhau (hiển thị hiện tại, hiển thị thống kê...), mỗi loại xử lý dữ liệu theo cách riêng.

**Cách sử dụng:** đăng ký bao nhiêu loại display tuỳ ý vào `WeatherStation`; mỗi lần có số đo mới chỉ cần gọi `SetMeasurements()` một lần.

**Cách hiện thực:** `CurrentConditionsDisplay` chỉ in trực tiếp số đo mới nhất, trong khi `StatisticsDisplay` tự giữ thêm trạng thái riêng (`_temperatures`) để tính trung bình — hai Observer xử lý cùng một thông báo theo hai cách hoàn toàn khác nhau, độc lập với nhau.

```csharp
// Observer
public interface IWeatherObserver
{
    void Update(double temperature, double humidity);
}

// Subject
public class WeatherStation
{
    private readonly List<IWeatherObserver> _observers = new List<IWeatherObserver>();

    public void Subscribe(IWeatherObserver observer)
    {
        _observers.Add(observer);
    }

    public void SetMeasurements(double temperature, double humidity)
    {
        foreach (var observer in _observers)
        {
            observer.Update(temperature, humidity);
        }
    }
}

// ConcreteObserver
public class CurrentConditionsDisplay : IWeatherObserver
{
    public void Update(double temperature, double humidity)
    {
        Console.WriteLine($"[CurrentConditions] Nhiet do: {temperature}C, Do am: {humidity}%");
    }
}

public class StatisticsDisplay : IWeatherObserver
{
    private readonly List<double> _temperatures = new List<double>();

    public void Update(double temperature, double humidity)
    {
        _temperatures.Add(temperature);
        var avg = _temperatures.Average();
        Console.WriteLine($"[Statistics] Nhiet do trung binh: {avg:0.0}C");
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var station = new WeatherStation();
        station.Subscribe(new CurrentConditionsDisplay());
        station.Subscribe(new StatisticsDisplay());

        station.SetMeasurements(25, 65);
        station.SetMeasurements(27, 70);
    }
}
// [CurrentConditions] Nhiet do: 25C, Do am: 65%
// [Statistics] Nhiet do trung binh: 25.0C
// [CurrentConditions] Nhiet do: 27C, Do am: 70%
// [Statistics] Nhiet do trung binh: 26.0C
```
