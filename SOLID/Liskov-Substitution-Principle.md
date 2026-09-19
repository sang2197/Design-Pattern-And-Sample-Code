# Liskov Substitution Principle (LSP)

## 1. Khái niệm cơ bản

**Liskov Substitution Principle** phát biểu: **object của class con phải có thể thay thế object của class cha ở bất kỳ đâu class cha được dùng, mà không làm sai lệch tính đúng đắn của chương trình.** Nói cách khác, class con phải tuân thủ đúng "hợp đồng" (behavior/contract) mà class cha (hoặc interface) đã cam kết: không thu hẹp điều kiện đầu vào chặt hơn, không ném ra ngoại lệ mà class cha không có, không thay đổi kết quả theo cách vi phạm kỳ vọng của Client đang dùng qua abstraction.

Vi phạm LSP thường xuất hiện khi kế thừa được dùng chỉ vì hai class "giống nhau về mặt khái niệm", chứ không phải vì class con thực sự hiện thực đúng hành vi mà class cha cam kết.

## 2. Khi nào nên dùng

Dùng khi:

✅ Thiết kế class con kế thừa/hiện thực một class cha hoặc interface có sẵn

✅ Client đang thao tác với object thông qua abstraction (class cha/interface), không biết cụ thể là class con nào

✅ Muốn đảm bảo thay class con này bằng class con khác không làm hỏng logic phía Client

✅ Phát hiện class con phải ném exception hoặc để trống method mà class cha không có, vì không thực sự "là một" class cha

## 3. Code examples

### Ví dụ 1 — Rectangle/Square: tách abstraction thay vì ép kế thừa

**Bài toán:** Nếu để `Square` kế thừa `Rectangle` và override `Width`/`Height` sao cho luôn set cả 2 cạnh bằng nhau, thì đoạn code `Rectangle rect = new Square(); rect.SetWidth(5); rect.SetHeight(10);` — vốn kỳ vọng `Area()` sau đó bằng `50` đúng theo hợp đồng của `Rectangle` — sẽ trả về `100` một cách bất ngờ, vì `Square` đã ngầm ép buộc `Width == Height`. Về mặt hình học Square "là một" Rectangle đặc biệt, nhưng về hành vi lập trình thì `Square` không thể thay thế `Rectangle` mà không làm sai kỳ vọng của Client — đây là ví dụ kinh điển của vi phạm LSP.

**Ý nghĩa của LSP trong ví dụ này:** Thay vì ép `Square` kế thừa `Rectangle`, tách một abstraction chung `IShape` với `Area()`; `Rectangle` và `Square` đều hiện thực `IShape` độc lập, mỗi class tự đảm bảo đúng hợp đồng của riêng mình (Rectangle cho set Width/Height độc lập, Square chỉ có một cạnh duy nhất) — không còn class nào phải "giả vờ" kế thừa rồi âm thầm phá vỡ hành vi gốc.

**Cách implementation (C#):**

```csharp
public interface IShape
{
    double Area();
}

public class Rectangle : IShape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public double Area() => Width * Height;
}

public class Square : IShape
{
    public double Side { get; set; }

    public double Area() => Side * Side;
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IShape rectangle = new Rectangle { Width = 5, Height = 10 };
        Console.WriteLine(rectangle.Area()); // 50

        IShape square = new Square { Side = 5 };
        Console.WriteLine(square.Area()); // 25

        // Ca hai deu la IShape hop le, khong can biet ben trong la Rectangle hay Square
        var shapes = new List<IShape> { rectangle, square };
        foreach (var shape in shapes)
        {
            Console.WriteLine(shape.Area());
        }
    }
}
// 50
// 25
// 50
// 25
```

### Ví dụ 2 — Bird/Penguin: tách khả năng bay thành interface riêng

**Bài toán:** `Bird` (class cha) có method `Fly()`, `Sparrow` kế thừa và bay bình thường, nhưng `Penguin` cũng kế thừa `Bird` dù không biết bay — buộc phải override `Fly()` bằng cách ném `NotSupportedException` hoặc để trống. Nếu Client có đoạn code dùng chung `foreach (var b in birds) b.Fly();` trên một danh sách `Bird`, chương trình sẽ lỗi ngay khi gặp `Penguin`, dù về cú pháp `Penguin` vẫn "là một" `Bird` hợp lệ.

**Ý nghĩa của LSP trong ví dụ này:** Tách khả năng bay ra một interface riêng `IFlyable`, chỉ những loài chim thực sự bay được (`Sparrow`) mới hiện thực nó; `Bird` (abstract) chỉ giữ hành vi mà mọi loài chim đều có (`Eat()`), còn `Penguin` kế thừa `Bird` nhưng không hiện thực `IFlyable`. Client làm việc qua `IFlyable` sẽ không bao giờ nhận một object không thực sự bay được.

**Cách implementation (C#):**

```csharp
public abstract class Bird
{
    public string Name { get; }

    protected Bird(string name)
    {
        Name = name;
    }

    public virtual void Eat() => Console.WriteLine($"{Name} dang an");
}

public interface IFlyable
{
    void Fly();
}

public class Sparrow : Bird, IFlyable
{
    public Sparrow() : base("Chim se")
    {
    }

    public void Fly() => Console.WriteLine($"{Name} dang bay");
}

public class Penguin : Bird
{
    public Penguin() : base("Chim canh cut")
    {
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var birds = new List<Bird> { new Sparrow(), new Penguin() };
        foreach (var bird in birds)
        {
            bird.Eat();
        }

        // Chi loai chim thuc su bay duoc moi xuat hien o day
        var flyableBirds = birds.OfType<IFlyable>();
        foreach (var flyable in flyableBirds)
        {
            flyable.Fly();
        }
    }
}
// Chim se dang an
// Chim canh cut dang an
// Chim se dang bay
```

### Ví dụ 3 — Repository chỉ đọc: tách interface đọc/ghi

**Bài toán:** `IRepository<T>` định nghĩa cả `GetAll()` lẫn `Add()`. `ReadOnlyProductRepository` (đọc dữ liệu từ nguồn tĩnh, không cho phép ghi) buộc phải hiện thực `IRepository<T>` để dùng chung ở những nơi cần đọc dữ liệu, nên phải override `Add()` bằng cách ném `NotSupportedException`. Một đoạn code dùng chung như `void Seed(IRepository<T> repo) { repo.Add(item); }` sẽ lỗi ngay nếu vô tình được truyền vào một `ReadOnlyProductRepository`.

**Ý nghĩa của LSP trong ví dụ này:** Tách `IRepository<T>` thành hai interface nhỏ hơn: `IReadableRepository<T>` (chỉ có `GetAll()`) và `IWritableRepository<T>` (chỉ có `Add()`), rồi `IRepository<T>` kế thừa cả hai cho trường hợp cần đủ quyền đọc/ghi. `ReadOnlyProductRepository` chỉ hiện thực `IReadableRepository<T>` — không còn phải giả vờ hỗ trợ `Add()` rồi ném exception, và Client chỉ cần khai báo đúng interface mình thực sự cần.

**Cách implementation (C#):**

```csharp
public interface IReadableRepository<T>
{
    List<T> GetAll();
}

public interface IWritableRepository<T>
{
    void Add(T item);
}

public interface IRepository<T> : IReadableRepository<T>, IWritableRepository<T>
{
}

public class ReadOnlyProductRepository : IReadableRepository<string>
{
    private readonly List<string> _products = new List<string> { "Product A", "Product B" };

    public List<string> GetAll() => _products;
}

public class InMemoryProductRepository : IRepository<string>
{
    private readonly List<string> _products = new List<string>();

    public List<string> GetAll() => _products;

    public void Add(string item) => _products.Add(item);
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IReadableRepository<string> readOnly = new ReadOnlyProductRepository();
        Console.WriteLine(string.Join(", ", readOnly.GetAll()));

        IRepository<string> writable = new InMemoryProductRepository();
        writable.Add("Product C");
        Console.WriteLine(string.Join(", ", writable.GetAll()));
    }
}
// Product A, Product B
// Product C
```
