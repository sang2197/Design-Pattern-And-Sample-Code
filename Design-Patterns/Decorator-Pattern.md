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

### Ví dụ 2 — Bổ sung Logging và Caching cho Product

**Bài toán:** Một hệ thống bán hàng có `Product` chịu trách nhiệm lấy thông tin sản phẩm từ database. Sau đó hệ thống phát sinh thêm các yêu cầu như ghi log mỗi lần lấy sản phẩm và cache kết quả để giảm số lần truy vấn database. Nếu đưa trực tiếp toàn bộ logic logging và caching vào `Product`, class này sẽ phải xử lý cả business logic lẫn các cross-cutting concern không thuộc trách nhiệm chính của nó. Nếu dùng kế thừa để tạo các class như `Logging`, `CachingProduct`, `LoggingCachingProduct`..., số lượng subclass sẽ tăng nhanh khi có thêm các tính năng như retry, metrics hoặc authorization.

**Ý nghĩa của Decorator trong ví dụ này:** `ProductDecorator` hiện thực cùng interface `IProduct` và giữ một `IProduct` khác bên trong. `LoggingDecorator` chỉ bổ sung logging, còn `CachingProductDecorator` chỉ chịu trách nhiệm caching. Vì tất cả đều cùng hiện thực `IProduct`, Client có thể bọc chúng theo nhiều cách khác nhau mà không cần sửa `Product`. Ví dụ có thể dùng `Product` trực tiếp, chỉ thêm cache, chỉ thêm logging, hoặc kết hợp cả logging và caching tùy theo cấu hình của hệ thống.

**Cách implementation (C#):**

```csharp
// Component
public interface IProduct
{
    string GetProduct(int id);
}

// ConcreteComponent
public class Product : IProduct
{
    public string GetProduct(int id)
    {
        return $"Product {id} from Database";
    }
}

// Decorator
public abstract class ProductDecorator : IProduct
{
    protected readonly IProduct Inner;

    protected ProductDecorator(IProduct inner)
    {
        Inner = inner;
    }

    public virtual string GetProduct(int id)
    {
        return Inner.GetProduct(id);
    }
}

// ConcreteDecorator - Logging
public class LoggingDecorator : ProductDecorator
{
    public LoggingDecorator(IProduct inner)
        : base(inner)
    {
    }

    public override string GetProduct(int id)
    {
        Console.WriteLine($"[LOG] Getting product {id}");

        var result = Inner.GetProduct(id);

        Console.WriteLine($"[LOG] Product {id} loaded");
        return result;
    }
}

// ConcreteDecorator - Caching
public class CachingProductDecorator : ProductDecorator
{
    private readonly Dictionary<int, string> _cache = new();

    public CachingProductDecorator(IProduct inner)
        : base(inner)
    {
    }

    public override string GetProduct(int id)
    {
        if (_cache.TryGetValue(id, out var product))
        {
            return $"[CACHE] {product}";
        }

        var result = Inner.GetProduct(id);
        _cache[id] = result;

        return result;
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IProduct service =
            new LoggingDecorator(
                new CachingProductDecorator(
                    new Product()));

        Console.WriteLine(service.GetProduct(1));
        // [LOG] Getting product 1
        // [LOG] Product 1 loaded
        // Product 1 from Database

        Console.WriteLine(service.GetProduct(1));
        // [LOG] Getting product 1
        // [LOG] Product 1 loaded
        // [CACHE] Product 1 from Database
    }
}
```

### Ví dụ 3 — Pipeline xử lý dữ liệu trước khi ghi file (Nén / Mã hóa)

**Bài toán:** Một hệ thống cần ghi dữ liệu xuống file, nhưng tùy từng trường hợp dữ liệu có thể được ghi trực tiếp, nén trước khi ghi, mã hóa trước khi ghi, hoặc thực hiện cả nén và mã hóa. Nếu dùng kế thừa để tạo từng tổ hợp như `CompressedFileDataSource`, `EncryptedFileDataSource`, `CompressedEncryptedFileDataSource`..., số lượng subclass sẽ tăng nhanh khi có thêm các bước xử lý mới. Ngoài ra, thứ tự xử lý dữ liệu cũng có thể thay đổi tùy yêu cầu, nên việc định nghĩa cố định từng tổ hợp bằng subclass sẽ thiếu linh hoạt.

**Ý nghĩa của Decorator trong ví dụ này:** `DataSourceDecorator` hiện thực cùng interface `IDataSource` và giữ một `IDataSource` khác bên trong. `CompressionDecorator` chỉ chịu trách nhiệm nén dữ liệu rồi chuyển kết quả cho `Inner`, còn `EncryptionDecorator` chỉ mã hóa dữ liệu rồi tiếp tục chuyển xuống `Inner`. Cuối cùng `FileDataSource` nhận dữ liệu đã được xử lý và thực hiện ghi file. Nhờ các Decorator có cùng interface, Client có thể tự do kết hợp các bước xử lý và kiểm soát thứ tự pipeline chỉ bằng cách thay đổi cách lồng các Decorator.

**Cách implementation (C#):**

```csharp
// Component
public interface IDataSource
{
    void Write(string data);
}

// ConcreteComponent
public class FileDataSource : IDataSource
{
    public void Write(string data)
    {
        Console.WriteLine($"Write to file: {data}");
    }
}

// Decorator
public abstract class DataSourceDecorator : IDataSource
{
    protected readonly IDataSource Inner;

    protected DataSourceDecorator(IDataSource inner)
    {
        Inner = inner;
    }

    public virtual void Write(string data)
    {
        Inner.Write(data);
    }
}

// ConcreteDecorator - Compression
public class CompressionDecorator : DataSourceDecorator
{
    public CompressionDecorator(IDataSource inner)
        : base(inner)
    {
    }

    public override void Write(string data)
    {
        // Giả lập quá trình nén để tập trung vào cấu trúc Decorator
        string compressed = $"[compressed]{data}";

        Inner.Write(compressed);
    }
}

// ConcreteDecorator - Encryption
public class EncryptionDecorator : DataSourceDecorator
{
    public EncryptionDecorator(IDataSource inner)
        : base(inner)
    {
    }

    public override void Write(string data)
    {
        // Giả lập quá trình mã hóa để tập trung vào cấu trúc Decorator
        string encrypted = $"[encrypted]{data}";

        Inner.Write(encrypted);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        // Dữ liệu đi theo pipeline:
        // Compression -> Encryption -> File
        IDataSource source =
            new CompressionDecorator(
                new EncryptionDecorator(
                    new FileDataSource()));

        source.Write("hello");

        // Write to file:
        // [encrypted][compressed]hello
    }
}
```