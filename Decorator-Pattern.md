# Decorator Pattern

## 1. Khái niệm cơ bản

**Decorator** là một Structural Design Pattern, cho phép **thêm hành vi/chức năng cho một object tại runtime** bằng cách bọc object đó trong một hoặc nhiều Decorator có cùng interface, thay vì sửa class gốc hoặc tạo nhiều subclass cho từng tổ hợp tính năng.

Pattern gồm 4 thành phần:

- **Component (interface):** interface chung cho object gốc và các Decorator.
- **ConcreteComponent:** hiện thực Component, chứa hành vi cơ bản.
- **Decorator:** cũng hiện thực `Component`, giữ tham chiếu tới một `Component` khác bên trong và chuyển tiếp lời gọi tới object đó.
- **ConcreteDecorator:** kế thừa Decorator và thêm hành vi trước hoặc sau khi gọi Component được bọc.

## 2. Bài toán

Muốn thêm các tính năng phụ có thể **kết hợp tự do** với nhau (logging, caching, nén, mã hóa, thêm topping...), cách làm bằng kế thừa sẽ phải tạo một subclass cho **từng tổ hợp tính năng** (`CoffeeWithMilk`, `CoffeeWithSugar`, `CoffeeWithMilkAndSugar`...):

- Số lượng class tăng theo **cấp số nhân** (combinatorial explosion) khi số tính năng tăng lên.
- Tổ hợp tính năng phải **cố định lúc biên dịch** — không thể thêm/bớt tính năng cho một object cụ thể tại runtime.

## 3. Ý nghĩa của Decorator

- **Decorator giải quyết bằng cách bọc nhiều lớp quanh Component gốc:** mỗi Decorator phụ trách đúng một chức năng và có thể kết hợp linh hoạt với các Decorator khác tại runtime, ví dụ `new SugarDecorator(new MilkDecorator(new SimpleCoffee()))`.
- Hỗ trợ **Open/Closed Principle**: thêm chức năng mới bằng cách tạo thêm `ConcreteDecorator`, không cần sửa Component gốc hoặc các Decorator hiện có.
- Component gốc và Decorator dùng chung interface nên Client sử dụng object đã được bọc **giống như object gốc**, không cần biết bên trong có bao nhiêu lớp Decorator.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: nêu rõ khi nào nên dùng, có đủ Component/ConcreteComponent/Decorator/ConcreteDecorator, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Gọi món cà phê, thêm topping tự do (Milk / Sugar)

**Khi nào dùng:** khách có thể chọn thêm 0, 1 hoặc nhiều topping cho ly cà phê, mỗi cách kết hợp làm thay đổi cả mô tả lẫn giá tiền.

**Cách sử dụng:** Client lồng Decorator quanh `SimpleCoffee` theo bất kỳ tổ hợp nào cần, ví dụ `new SugarDecorator(new MilkDecorator(new SimpleCoffee()))`.

**Cách hiện thực:** `CoffeeDecorator` (abstract) giữ tham chiếu `Inner` và mặc định ủy quyền; mỗi `ConcreteDecorator` override `Describe()`/`Cost()` để cộng thêm phần của mình vào kết quả gọi `Inner`.

```csharp
// Component
public interface ICoffee
{
    string Describe();
    decimal Cost();
}

// ConcreteComponent
public class SimpleCoffee : ICoffee
{
    public string Describe() => "Coffee";
    public decimal Cost() => 20000;
}

// Decorator
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee Inner;

    protected CoffeeDecorator(ICoffee inner)
    {
        Inner = inner;
    }

    public virtual string Describe() => Inner.Describe();

    public virtual decimal Cost() => Inner.Cost();
}

// ConcreteDecorator
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee inner) : base(inner)
    {
    }

    public override string Describe() => $"{Inner.Describe()} + Milk";

    public override decimal Cost() => Inner.Cost() + 5000;
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee inner) : base(inner)
    {
    }

    public override string Describe() => $"{Inner.Describe()} + Sugar";

    public override decimal Cost() => Inner.Cost() + 2000;
}

// Chạy thử - cách sử dụng: lồng Decorator quanh Component gốc
public class Program
{
    public static void Main()
    {
        ICoffee order = new SugarDecorator(new MilkDecorator(new SimpleCoffee()));
        Console.WriteLine(order.Describe());
        Console.WriteLine(order.Cost());

        ICoffee anotherOrder = new MilkDecorator(new SimpleCoffee());
        Console.WriteLine(anotherOrder.Describe());
        Console.WriteLine(anotherOrder.Cost());
    }
}
// Coffee + Milk + Sugar
// 27000
// Coffee + Milk
// 25000
```

### Ví dụ 2 — Định dạng thông báo (thêm tiền tố, viết hoa) mà không sửa lớp gốc

**Khi nào dùng:** `BasicNotifier` chỉ gửi nguyên văn message, nhưng có lúc cần thêm tiền tố `[INFO]`, có lúc cần viết hoa toàn bộ, có lúc cần cả hai — và những yêu cầu định dạng này có thể thay đổi độc lập với nhau, không nên sửa thẳng vào `BasicNotifier`.

**Cách sử dụng:** bọc `BasicNotifier` trong `PrefixDecorator`, rồi bọc tiếp trong `UpperCaseDecorator` — thứ tự bọc quyết định thứ tự áp dụng định dạng.

**Cách hiện thực:** tương tự Ví dụ 1, `NotifierDecorator` (abstract) mặc định ủy quyền `Send()` cho `Inner`; mỗi `ConcreteDecorator` chỉ chèn thêm đúng một bước xử lý trước khi trả kết quả ra ngoài.

```csharp
// Component
public interface INotifier
{
    string Send(string message);
}

// ConcreteComponent
public class BasicNotifier : INotifier
{
    public string Send(string message) => message;
}

// Decorator
public abstract class NotifierDecorator : INotifier
{
    protected readonly INotifier Inner;

    protected NotifierDecorator(INotifier inner)
    {
        Inner = inner;
    }

    public virtual string Send(string message) => Inner.Send(message);
}

// ConcreteDecorator
public class PrefixDecorator : NotifierDecorator
{
    private readonly string _prefix;

    public PrefixDecorator(INotifier inner, string prefix) : base(inner)
    {
        _prefix = prefix;
    }

    public override string Send(string message) => $"{_prefix} {Inner.Send(message)}";
}

public class UpperCaseDecorator : NotifierDecorator
{
    public UpperCaseDecorator(INotifier inner) : base(inner)
    {
    }

    public override string Send(string message) => Inner.Send(message).ToUpper();
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        INotifier notifier = new UpperCaseDecorator(new PrefixDecorator(new BasicNotifier(), "[INFO]"));
        Console.WriteLine(notifier.Send("don hang da duoc xac nhan"));
    }
}
// [INFO] DON HANG DA DUOC XAC NHAN
```

### Ví dụ 3 — Ghép pipeline xử lý dữ liệu (nén rồi mã hoá)

**Khi nào dùng:** dữ liệu ghi ra nguồn (`FileDataSource`) đôi khi cần nén trước, đôi khi cần mã hoá trước, đôi khi cần cả nén lẫn mã hoá theo đúng thứ tự. Ghép các bước xử lý này bằng kế thừa sẽ không linh hoạt bằng việc bọc (wrap) từng bước lại với nhau.

**Cách sử dụng:** `new EncryptionDecorator(new CompressionDecorator(new FileDataSource()))` — thứ tự bọc từ trong ra ngoài chính là thứ tự các bước xử lý được áp dụng.

**Cách hiện thực:** `CompressionDecorator`/`EncryptionDecorator` mỗi lớp chỉ biến đổi kết quả của `Inner.Write()` theo đúng một quy tắc riêng, không biết và không cần biết các Decorator khác đang bọc quanh nó.

```csharp
// Component
public interface IDataSource
{
    string Write(string data);
}

// ConcreteComponent
public class FileDataSource : IDataSource
{
    public string Write(string data) => data;
}

// Decorator
public abstract class DataSourceDecorator : IDataSource
{
    protected readonly IDataSource Inner;

    protected DataSourceDecorator(IDataSource inner)
    {
        Inner = inner;
    }

    public virtual string Write(string data) => Inner.Write(data);
}

// ConcreteDecorator
public class CompressionDecorator : DataSourceDecorator
{
    public CompressionDecorator(IDataSource inner) : base(inner)
    {
    }

    public override string Write(string data) => $"[compressed]{Inner.Write(data)}";
}

public class EncryptionDecorator : DataSourceDecorator
{
    public EncryptionDecorator(IDataSource inner) : base(inner)
    {
    }

    public override string Write(string data)
    {
        var written = Inner.Write(data);
        var reversed = new string(written.Reverse().ToArray());
        return $"[encrypted]{reversed}";
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        // Thu tu bien: nen truoc (CompressionDecorator gan Component nhat), ma hoa sau
        IDataSource source = new EncryptionDecorator(new CompressionDecorator(new FileDataSource()));
        Console.WriteLine(source.Write("hello"));
    }
}
// [encrypted]olleh]desserpmoc[
```