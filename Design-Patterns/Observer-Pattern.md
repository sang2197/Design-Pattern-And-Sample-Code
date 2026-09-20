# Observer Pattern

## 1. Khái niệm cơ bản

**Observer** là một Behavioral Design Pattern, định nghĩa cơ chế **một-nhiều (one-to-many)** giữa các đối tượng: khi trạng thái của một đối tượng (Subject) thay đổi, tất cả đối tượng phụ thuộc vào nó (Observer) đã đăng ký sẽ **tự động được thông báo và cập nhật**, mà Subject không cần biết chi tiết từng Observer là ai, xử lý như thế nào.

Pattern gồm 4 thành phần:

- **Subject (Observable):** giữ danh sách các Observer đã đăng ký, cung cấp method `Subscribe()`/`Unsubscribe()`, và gọi `Notify()` tới tất cả Observer khi trạng thái thay đổi.
- **Observer (interface):** khai báo method (ví dụ `Update()`) mà Subject sẽ gọi khi có sự kiện.
- **ConcreteSubject:** hiện thực cụ thể, chứa trạng thái nghiệp vụ và phát sự kiện khi trạng thái đổi.
- **ConcreteObserver:** hiện thực cụ thể `Observer`, chứa logic phản ứng khi nhận được thông báo.

## 2. Khi nào nên dùng

Dùng khi:

✅ Một thay đổi trạng thái cần thông báo cho nhiều đối tượng khác cùng lúc

✅ Không muốn đối tượng phát sự kiện phụ thuộc trực tiếp vào từng nơi xử lý

✅ Số lượng/loại đối tượng lắng nghe có thể thay đổi linh hoạt tại runtime

✅ Muốn tuân thủ Open/Closed Principle (thêm phản ứng mới không sửa nơi phát sự kiện)

## 3. Code examples

### Ví dụ 1 — Đơn hàng đổi trạng thái, nhiều nơi cần phản ứng theo

**Bài toán:** Khi một đơn hàng bị hủy, hệ thống cần đồng thời gửi email thông báo cho khách và hoàn trả tồn kho — hai hành động độc lập nhau, và trong tương lai có thể cần thêm hành động khác như gửi SMS hoặc ghi log. Nếu gọi tuần tự từng service này ngay trong method xử lý đổi trạng thái đơn hàng, nơi phát sự kiện sẽ phải biết và phụ thuộc trực tiếp vào từng lớp xử lý cụ thể. Mỗi lần thêm một phản ứng mới lại phải sửa vào đúng method đó, vi phạm Open/Closed Principle, khiến class ngày càng phình to và khó test riêng từng phần phản ứng.

**Ý nghĩa của Observer trong ví dụ này:** `OrderSubject` đóng vai trò Subject, chỉ biết interface `IOrderObserver` chung chứ không biết có bao nhiêu Observer hay chúng xử lý ra sao. `EmailNotifier` và `InventoryUpdater` là các ConcreteObserver, tự đăng ký lắng nghe qua `Subscribe()`. Khi trạng thái đơn hàng đổi, `OrderSubject.ChangeStatus()` chỉ cần duyệt danh sách `_observers` và gọi `Update()`, không cần biết chi tiết từng phản ứng. Muốn thêm phản ứng mới (ví dụ gửi SMS), chỉ cần thêm một ConcreteObserver mới và đăng ký nó, không phải sửa `OrderSubject`.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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

**Bài toán:** Nhiều nhà đầu tư muốn được báo ngay khi giá một mã cổ phiếu thay đổi, nhưng số lượng và danh tính người theo dõi thay đổi liên tục — có người đăng ký, có người hủy đăng ký bất cứ lúc nào, nên `Stock` không thể biết trước danh sách này ngay khi viết code. Nếu để `Stock` giữ một danh sách cố định các nhà đầu tư cụ thể và tự gọi từng người, việc thêm/bớt nhà đầu tư sẽ đòi hỏi sửa trực tiếp vào lớp `Stock`, phá vỡ khả năng mở rộng linh hoạt tại runtime.

**Ý nghĩa của Observer trong ví dụ này:** `Stock` đóng vai trò ConcreteSubject, ngoài việc quản lý danh sách `IStock` còn giữ thêm trạng thái nghiệp vụ thực sự (`_price`) — minh họa Subject không chỉ đơn thuần là nơi phát sự kiện mà thường có dữ liệu đi kèm. Mỗi `Investor` là một ConcreteObserver, tự gọi `stock.Subscribe(this)` để bắt đầu theo dõi mã cổ phiếu mình quan tâm. Khi giá đổi, `SetPrice()` chỉ cần gọi `OnPriceChanged()` trên từng Observer đã đăng ký, cho phép số lượng nhà đầu tư tăng giảm tự do mà không ảnh hưởng đến logic của `Stock`.

**Cách implementation (C#):**

```csharp
// Observer
public interface IStock
{
    void OnPriceChanged(string symbol, decimal price);
}

// Subject
public class Stock
{
    private readonly List<IStock> _observers = new List<IStock>();
    public string Symbol { get; }
    private decimal _price;

    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void Subscribe(IStock observer)
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
public class Investor : IStock
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
```

**Cách sử dụng (C#):**

```csharp
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

**Bài toán:** Một `WeatherStation` liên tục đo được số liệu nhiệt độ/độ ẩm mới, và hệ thống cần cập nhật đồng thời nhiều loại màn hình hiển thị khác nhau — một màn hình chỉ hiển thị số đo hiện tại, một màn hình khác lại tính toán và hiển thị thống kê trung bình. Nếu `WeatherStation` tự gọi trực tiếp từng loại màn hình ngay trong hàm nhận số đo, nó sẽ phải biết chi tiết cách từng màn hình xử lý dữ liệu, và mỗi lần thêm một loại màn hình mới đều phải sửa lại chính hàm đó.

**Ý nghĩa của Observer trong ví dụ này:** `WeatherStation` là Subject, chỉ làm việc qua abstraction `IWeatherObserver` mà không biết có bao nhiêu loại display hay chúng hiển thị ra sao. `CurrentConditionsDisplay` và `StatisticsDisplay` là hai ConcreteObserver xử lý cùng một thông báo theo hai cách hoàn toàn khác nhau và độc lập với nhau: `CurrentConditionsDisplay` chỉ in trực tiếp số đo mới nhất, còn `StatisticsDisplay` tự giữ thêm trạng thái riêng (`_temperatures`) để tính trung bình. Mỗi lần có số đo mới, `WeatherStation` chỉ cần gọi `SetMeasurements()` một lần, và có thể đăng ký thêm bao nhiêu loại display tùy ý mà không phải sửa `WeatherStation`.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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
