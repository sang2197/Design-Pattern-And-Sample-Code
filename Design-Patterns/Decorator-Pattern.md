# Decorator Pattern

## 1. Khái niệm cơ bản

**Decorator** là một Structural Design Pattern, cho phép **thêm hành vi/chức năng cho một object tại runtime** bằng cách bọc object đó trong một hoặc nhiều Decorator có cùng interface, thay vì sửa class gốc hoặc tạo nhiều subclass cho từng tổ hợp tính năng.

Pattern gồm 4 thành phần:

- **Component (interface):** interface chung cho object gốc và các Decorator.
- **ConcreteComponent:** hiện thực `Component`, chứa hành vi cơ bản.
- **Decorator (abstract class):** cũng hiện thực `Component`, giữ tham chiếu tới một `Component` khác bên trong (`Inner`) và mặc định ủy quyền lời gọi tới object đó.
- **ConcreteDecorator:** kế thừa `Decorator` và thêm hành vi trước hoặc sau khi gọi Component được bọc.

## 2. Khi nào nên dùng

Dùng khi:

✅ Cần thêm hành vi cho object tại runtime, không cố định lúc biên dịch

✅ Các tính năng phụ có thể kết hợp tự do với nhau (không phải một tổ hợp cố định)

✅ Muốn tránh bùng nổ số lượng subclass khi kế thừa theo từng tổ hợp tính năng

✅ Muốn tuân thủ Open/Closed Principle (thêm tính năng mới không sửa code cũ)

## 3. Code examples

### Ví dụ 1 — Gọi món cà phê, thêm topping tự do (Milk / Sugar)

**Bài toán:** Quán cà phê bán `SimpleCoffee`, nhưng khách có thể chọn thêm 0, 1 hoặc nhiều topping (sữa, đường...) cho ly cà phê, và mỗi tổ hợp topping làm thay đổi cả mô tả lẫn giá tiền. Nếu hiện thực bằng kế thừa, mỗi tổ hợp topping (chỉ sữa, chỉ đường, sữa và đường...) sẽ cần một subclass riêng như `CoffeeWithMilk`, `CoffeeWithSugar`, `CoffeeWithMilkAndSugar`, khiến số lượng class tăng theo cấp số nhân khi số topping tăng lên, và tổ hợp topping của một ly cà phê phải cố định ngay lúc biên dịch chứ không thể chọn linh hoạt tại runtime.

**Ý nghĩa của Decorator trong ví dụ này:** `CoffeeDecorator` (abstract) giữ tham chiếu `Inner` tới một `ICoffee` khác và mặc định ủy quyền `Describe()`/`Cost()` cho `Inner`. `MilkDecorator` và `SugarDecorator` chỉ cần override hai method này để cộng thêm đúng phần mô tả/giá tiền của riêng mình vào kết quả gọi `Inner`, hoàn toàn không cần biết `Inner` là `SimpleCoffee` hay đã bị bọc bởi Decorator nào khác. Nhờ vậy, Client có thể lồng các Decorator theo bất kỳ tổ hợp nào ngay tại runtime, ví dụ `new SugarDecorator(new MilkDecorator(new SimpleCoffee()))`, thay vì phải định nghĩa sẵn một subclass cho từng tổ hợp topping.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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

**Bài toán:** `BasicNotifier` chỉ gửi nguyên văn message. Tuy nhiên có lúc hệ thống cần thêm tiền tố `[INFO]` trước nội dung, có lúc cần viết hoa toàn bộ message, có lúc cần áp dụng cả hai theo một thứ tự nhất định, và các yêu cầu định dạng này có thể thay đổi độc lập với nhau tùy theo loại thông báo. Nếu sửa trực tiếp vào `BasicNotifier` hoặc tạo subclass riêng cho từng cách kết hợp định dạng (`PrefixedNotifier`, `UpperCaseNotifier`, `PrefixedUpperCaseNotifier`...), class gốc sẽ ngày càng phình to hoặc số subclass sẽ tăng nhanh mỗi khi có thêm một kiểu định dạng mới.

**Ý nghĩa của Decorator trong ví dụ này:** `NotifierDecorator` (abstract) mặc định ủy quyền `Send()` cho `Inner`, còn `PrefixDecorator` và `UpperCaseDecorator` mỗi lớp chỉ chèn thêm đúng một bước xử lý (thêm tiền tố, hoặc viết hoa) trước khi trả kết quả ra ngoài, mà không đụng vào `BasicNotifier` hay lẫn vào logic của Decorator còn lại. Việc bọc `BasicNotifier` trong `PrefixDecorator` rồi bọc tiếp trong `UpperCaseDecorator` cho phép kết hợp các bước định dạng theo bất kỳ thứ tự nào, và thứ tự bọc từ trong ra ngoài chính là thứ tự áp dụng định dạng.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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

**Bài toán:** Dữ liệu ghi ra nguồn (`FileDataSource`) đôi khi cần nén trước khi ghi, đôi khi cần mã hoá trước khi ghi, đôi khi cần cả nén lẫn mã hoá theo đúng một thứ tự cụ thể (nén trước, mã hoá sau), tùy theo cấu hình của từng luồng dữ liệu. Nếu hiện thực từng tổ hợp bước xử lý bằng kế thừa (`CompressedFileDataSource`, `EncryptedFileDataSource`, `CompressedEncryptedFileDataSource`...), số subclass sẽ tăng nhanh theo số bước xử lý và không thể thay đổi thứ tự các bước cho một luồng dữ liệu cụ thể mà không viết thêm subclass mới.

**Ý nghĩa của Decorator trong ví dụ này:** `DataSourceDecorator` (abstract) mặc định ủy quyền `Write()` cho `Inner`, còn `CompressionDecorator` và `EncryptionDecorator` mỗi lớp chỉ biến đổi kết quả của `Inner.Write()` theo đúng một quy tắc riêng (thêm nhãn nén, hoặc đảo ngược chuỗi và thêm nhãn mã hoá), không biết và không cần biết còn Decorator nào khác đang bọc quanh nó. Biểu thức `new EncryptionDecorator(new CompressionDecorator(new FileDataSource()))` bọc `CompressionDecorator` sát `FileDataSource` rồi bọc tiếp `EncryptionDecorator` ra ngoài, nên thứ tự bọc từ trong ra ngoài chính là thứ tự các bước xử lý được áp dụng (nén trước, mã hoá sau), và có thể đổi thứ tự hoặc bớt một bước chỉ bằng cách đổi cách lồng Decorator, không cần sửa hay thêm class mới.

**Cách implementation (C#):**

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
```

**Cách sử dụng (C#):**

```csharp
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
