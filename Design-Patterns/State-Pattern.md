# State Pattern

## 1. Khái niệm cơ bản

**State** là một Behavioral Design Pattern, cho phép một object **thay đổi hành vi khi trạng thái nội bộ của nó thay đổi** — nhìn như thể object đã đổi sang một class khác. Thay vì dùng `if/switch` liệt kê hành vi theo từng trạng thái ngay trong một class, mỗi trạng thái được tách thành một class riêng.

Pattern gồm 3 thành phần:

- **State (interface):** khai báo các method hành vi phụ thuộc vào trạng thái hiện tại.
- **ConcreteState:** hiện thực `State` ứng với một trạng thái cụ thể, chứa logic của trạng thái đó và có thể tự quyết định chuyển `Context` sang `ConcreteState` khác.
- **Context:** giữ tham chiếu tới `State` hiện tại và ủy quyền hành vi cho nó; expose method để Client tương tác mà không cần biết đang ở trạng thái nào.

## 2. Khi nào nên dùng

Dùng khi:

✅ Hành vi của object phụ thuộc vào trạng thái hiện tại và phải đổi tại runtime

✅ Muốn tránh khối if/switch lớn kiểm tra trạng thái lặp lại ở nhiều method

✅ Việc chuyển trạng thái có quy tắc rõ ràng, muốn tách biệt logic của từng trạng thái

✅ Muốn tuân thủ Open/Closed Principle (thêm trạng thái mới không sửa code cũ)

## 3. Code examples

### Ví dụ 1 — Vòng đời đơn hàng (New → Paid → Shipped → Delivered)

**Bài toán:** Một đơn hàng đi qua các trạng thái New, Paid, Shipped, Delivered theo đúng thứ tự, và hành vi khi gọi `Pay()`/`Ship()`/`Deliver()` phải khác nhau tùy trạng thái hiện tại (ví dụ không thể `Ship()` khi đơn chưa `Pay()`). Nếu viết cả 3 method này bằng if/switch kiểm tra một biến trạng thái (enum) dùng chung, logic hợp lệ/không hợp lệ của từng trạng thái bị trộn lẫn trong nhiều method, dễ bỏ sót quy tắc khi hệ thống có thêm trạng thái mới.

**Ý nghĩa của State trong ví dụ này:** Mỗi trạng thái được tách thành một `ConcreteState` riêng (`NewOrderState`, `PaidOrderState`, `ShippedOrderState`, `DeliveredOrderState`), tự quyết định hành động nào hợp lệ và tự chuyển `OrderContext` sang state kế tiếp khi hợp lệ. `OrderContext` chỉ ủy quyền `Pay()`/`Ship()`/`Deliver()` cho state hiện tại, không chứa bất kỳ điều kiện nào theo trạng thái.

**Cách implementation (C#):**

```csharp
// State
public interface IOrderState
{
    string Name { get; }
    void Pay(OrderContext context);
    void Ship(OrderContext context);
    void Deliver(OrderContext context);
}

// ConcreteState
public class NewOrderState : IOrderState
{
    public string Name => "New";

    public void Pay(OrderContext context)
    {
        Console.WriteLine("[Order] Thanh toan thanh cong, chuyen sang Paid");
        context.SetState(new PaidOrderState());
    }

    public void Ship(OrderContext context) => Console.WriteLine("[Order] Khong the giao hang khi chua thanh toan");

    public void Deliver(OrderContext context) => Console.WriteLine("[Order] Khong the hoan tat khi chua thanh toan");
}

public class PaidOrderState : IOrderState
{
    public string Name => "Paid";

    public void Pay(OrderContext context) => Console.WriteLine("[Order] Don hang da duoc thanh toan roi");

    public void Ship(OrderContext context)
    {
        Console.WriteLine("[Order] Da giao cho don vi van chuyen, chuyen sang Shipped");
        context.SetState(new ShippedOrderState());
    }

    public void Deliver(OrderContext context) => Console.WriteLine("[Order] Chua the hoan tat khi chua giao hang");
}

public class ShippedOrderState : IOrderState
{
    public string Name => "Shipped";

    public void Pay(OrderContext context) => Console.WriteLine("[Order] Don hang da thanh toan va dang giao");

    public void Ship(OrderContext context) => Console.WriteLine("[Order] Don hang dang tren duong giao, khong the giao lai");

    public void Deliver(OrderContext context)
    {
        Console.WriteLine("[Order] Da giao hang thanh cong, chuyen sang Delivered");
        context.SetState(new DeliveredOrderState());
    }
}

public class DeliveredOrderState : IOrderState
{
    public string Name => "Delivered";

    public void Pay(OrderContext context) => Console.WriteLine("[Order] Don hang da hoan tat");

    public void Ship(OrderContext context) => Console.WriteLine("[Order] Don hang da hoan tat");

    public void Deliver(OrderContext context) => Console.WriteLine("[Order] Don hang da duoc giao truoc do roi");
}

// Context
public class OrderContext
{
    private IOrderState _state = new NewOrderState();

    public string CurrentState => _state.Name;

    public void SetState(IOrderState state) => _state = state;

    public void Pay() => _state.Pay(this);
    public void Ship() => _state.Ship(this);
    public void Deliver() => _state.Deliver(this);
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var order = new OrderContext();
        Console.WriteLine(order.CurrentState); // New

        order.Ship();     // [Order] Khong the giao hang khi chua thanh toan
        order.Pay();      // [Order] Thanh toan thanh cong, chuyen sang Paid
        order.Ship();     // [Order] Da giao cho don vi van chuyen, chuyen sang Shipped
        order.Deliver();  // [Order] Da giao hang thanh cong, chuyen sang Delivered

        Console.WriteLine(order.CurrentState); // Delivered
    }
}
```

### Ví dụ 2 — Đèn giao thông tự chuyển màu (Đỏ → Xanh → Vàng → Đỏ)

**Bài toán:** Đèn giao thông cần chuyển tuần tự Đỏ → Xanh → Vàng → Đỏ mỗi khi có tín hiệu chuyển tiếp (`Next()`). Nếu dùng một biến enum lưu màu hiện tại rồi if/switch trong `Next()` để quyết định màu kế tiếp, method này ngày càng phình to khi cần thêm quy tắc mới (ví dụ đèn nhấp nháy vàng lúc bảo trì), và rất dễ gõ nhầm thứ tự chuyển màu.

**Ý nghĩa của State trong ví dụ này:** Mỗi màu đèn là một `ConcreteState` (`RedState`, `GreenState`, `YellowState`), tự biết chính xác màu kế tiếp và tự gọi `context.SetState(...)` để chuyển đổi. `TrafficLightContext` chỉ ủy quyền `Next()` cho state hiện tại, hoàn toàn không có logic quyết định thứ tự chuyển màu.

**Cách implementation (C#):**

```csharp
// State
public interface ITrafficLightState
{
    string Color { get; }
    void Next(TrafficLightContext context);
}

// ConcreteState
public class RedState : ITrafficLightState
{
    public string Color => "Red";

    public void Next(TrafficLightContext context)
    {
        Console.WriteLine("Do -> Xanh");
        context.SetState(new GreenState());
    }
}

public class GreenState : ITrafficLightState
{
    public string Color => "Green";

    public void Next(TrafficLightContext context)
    {
        Console.WriteLine("Xanh -> Vang");
        context.SetState(new YellowState());
    }
}

public class YellowState : ITrafficLightState
{
    public string Color => "Yellow";

    public void Next(TrafficLightContext context)
    {
        Console.WriteLine("Vang -> Do");
        context.SetState(new RedState());
    }
}

// Context
public class TrafficLightContext
{
    private ITrafficLightState _state = new RedState();

    public string CurrentColor => _state.Color;

    public void SetState(ITrafficLightState state) => _state = state;

    public void Next() => _state.Next(this);
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var light = new TrafficLightContext();
        Console.WriteLine(light.CurrentColor); // Red

        light.Next();
        Console.WriteLine(light.CurrentColor); // Green

        light.Next();
        Console.WriteLine(light.CurrentColor); // Yellow

        light.Next();
        Console.WriteLine(light.CurrentColor); // Red
    }
}
```

### Ví dụ 3 — Trình phát nhạc (Stopped / Playing / Paused)

**Bài toán:** Trình phát nhạc có 3 trạng thái Stopped, Playing, Paused; cùng một nút `Play()` nhưng hành vi khác nhau tùy trạng thái hiện tại — đang Stopped thì `Play()` bắt đầu phát, đang Paused thì `Play()` tiếp tục phát, đang Playing thì `Play()` không làm gì. Nếu nhồi hết các trường hợp này vào if/switch bên trong từng method `Play()`/`Pause()`/`Stop()`, số nhánh điều kiện tăng nhanh và rất dễ xử lý sai một tổ hợp trạng thái/hành động.

**Ý nghĩa của State trong ví dụ này:** Mỗi trạng thái (`StoppedState`, `PlayingState`, `PausedState`) tự quyết định phải làm gì và chuyển sang `ConcreteState` nào khi `Play()`/`Pause()`/`Stop()` được gọi. `MediaPlayerContext` chỉ ủy quyền cho state hiện tại, không chứa bất kỳ điều kiện nào theo trạng thái.

**Cách implementation (C#):**

```csharp
// State
public interface IPlayerState
{
    void Play(MediaPlayerContext context);
    void Pause(MediaPlayerContext context);
    void Stop(MediaPlayerContext context);
}

// ConcreteState
public class StoppedState : IPlayerState
{
    public void Play(MediaPlayerContext context)
    {
        Console.WriteLine("[Player] Bat dau phat");
        context.SetState(new PlayingState());
    }

    public void Pause(MediaPlayerContext context) => Console.WriteLine("[Player] Dang dung, khong the tam dung");

    public void Stop(MediaPlayerContext context) => Console.WriteLine("[Player] Da dung san roi");
}

public class PlayingState : IPlayerState
{
    public void Play(MediaPlayerContext context) => Console.WriteLine("[Player] Dang phat roi");

    public void Pause(MediaPlayerContext context)
    {
        Console.WriteLine("[Player] Tam dung");
        context.SetState(new PausedState());
    }

    public void Stop(MediaPlayerContext context)
    {
        Console.WriteLine("[Player] Dung phat");
        context.SetState(new StoppedState());
    }
}

public class PausedState : IPlayerState
{
    public void Play(MediaPlayerContext context)
    {
        Console.WriteLine("[Player] Tiep tuc phat");
        context.SetState(new PlayingState());
    }

    public void Pause(MediaPlayerContext context) => Console.WriteLine("[Player] Da tam dung roi");

    public void Stop(MediaPlayerContext context)
    {
        Console.WriteLine("[Player] Dung phat");
        context.SetState(new StoppedState());
    }
}

// Context
public class MediaPlayerContext
{
    private IPlayerState _state = new StoppedState();

    public void SetState(IPlayerState state) => _state = state;

    public void Play() => _state.Play(this);
    public void Pause() => _state.Pause(this);
    public void Stop() => _state.Stop(this);
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var player = new MediaPlayerContext();

        player.Pause(); // [Player] Dang dung, khong the tam dung
        player.Play();  // [Player] Bat dau phat
        player.Play();  // [Player] Dang phat roi
        player.Pause(); // [Player] Tam dung
        player.Play();  // [Player] Tiep tuc phat
        player.Stop();  // [Player] Dung phat
    }
}
```
