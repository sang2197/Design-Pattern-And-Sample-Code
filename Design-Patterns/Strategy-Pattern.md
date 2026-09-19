# Strategy Pattern

## 1. Khái niệm cơ bản

**Strategy** là một Behavioral Design Pattern, dùng để đóng gói mỗi **giải thuật/cách xử lý** thành một class riêng nhưng cùng hiện thực một interface, nhờ đó có thể thay đổi cách xử lý tại runtime mà không cần sửa code của class đang sử dụng Strategy.

Pattern gồm 3 thành phần:

- **Strategy (interface):** khai báo method chung mà mọi giải thuật phải hiện thực (ví dụ `Calculate()`).
- **ConcreteStrategy:** hiện thực một giải thuật/cách xử lý cụ thể.
- **Context:** giữ một Strategy và gọi nó qua interface, không cần biết ConcreteStrategy cụ thể bên trong.

## 2. Khi nào nên dùng

Dùng khi:

✅ Có nhiều giải thuật/cách xử lý khác nhau cho cùng một bài toán

✅ Muốn tránh khối if/switch lớn liệt kê từng trường hợp xử lý

✅ Cần đổi giải thuật đang áp dụng ngay tại runtime

✅ Muốn thêm cách xử lý mới mà không phải sửa logic của Context hoặc các Strategy hiện có.

## 3. Code examples

### Ví dụ 1 — Chiến lược giảm giá đơn hàng, đổi Strategy ngay tại runtime

**Bài toán:** Một đơn hàng có thể cần áp dụng nhiều loại giảm giá khác nhau (không giảm, giảm theo phần trăm, giảm số tiền cố định), và loại giảm giá áp dụng cho một đơn hàng cụ thể có thể thay đổi giữa chừng — ví dụ phát hiện khách hàng là VIP, hoặc khách nhập voucher — trong khi đối tượng đơn hàng (`OrderContext`) đã được tạo từ trước đó. Nếu viết một method `CalculateTotal()` chứa `if/switch` liệt kê từng loại giảm giá, mỗi lần cần đổi cách tính cho một đơn hàng đang tồn tại đều phải sửa lại logic bên trong method đó, và việc thêm loại giảm giá mới (ví dụ giảm giá theo bậc) buộc phải sửa vào đúng chỗ code dùng chung này.

**Ý nghĩa của Strategy trong ví dụ này:** `OrderContext` chỉ giữ một tham chiếu `IDiscountStrategy` và ủy quyền toàn bộ phép tính cho `Apply()`, không có `if/switch` nào theo loại giảm giá bên trong `Context`. Ba lớp `NoDiscount`, `PercentageDiscount`, `FixedAmountDiscount` mỗi lớp đóng gói đúng một công thức tính giảm giá. Nhờ `OrderContext` cung cấp `SetDiscountStrategy()`, có thể đổi `IDiscountStrategy` đang dùng ngay tại runtime mà không cần tạo lại `OrderContext`, và khi cần thêm loại giảm giá mới chỉ cần thêm một `ConcreteStrategy` mới mà không đụng vào `OrderContext` đang có.

**Cách implementation (C#):**

```csharp
// Strategy
public interface IDiscountStrategy
{
    decimal Apply(decimal orderTotal);
}

// ConcreteStrategy
public class NoDiscount : IDiscountStrategy
{
    public decimal Apply(decimal orderTotal) => orderTotal;
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percent;

    public PercentageDiscount(decimal percent)
    {
        _percent = percent;
    }

    public decimal Apply(decimal orderTotal) => orderTotal - (orderTotal * _percent / 100);
}

public class FixedAmountDiscount : IDiscountStrategy
{
    private readonly decimal _amount;

    public FixedAmountDiscount(decimal amount)
    {
        _amount = amount;
    }

    public decimal Apply(decimal orderTotal) => Math.Max(0, orderTotal - _amount);
}

// Context
public class OrderContext
{
    private IDiscountStrategy _discountStrategy;

    public OrderContext(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    // Cho phép đổi Strategy ngay tại runtime
    public void SetDiscountStrategy(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public decimal CalculateTotal(decimal orderTotal)
    {
        return _discountStrategy.Apply(orderTotal);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var context = new OrderContext(new NoDiscount());
        Console.WriteLine(context.CalculateTotal(500000)); // 500000

        // Cùng một context, chỉ đổi Strategy bên trong - không tạo lại object
        context.SetDiscountStrategy(new PercentageDiscount(20));
        Console.WriteLine(context.CalculateTotal(500000)); // 400000

        context.SetDiscountStrategy(new FixedAmountDiscount(50000));
        Console.WriteLine(context.CalculateTotal(500000)); // 450000
    }
}
```

### Ví dụ 2 — Chọn chiến lược tính phí vận chuyển theo điều kiện đơn hàng

**Bài toán:** Cách tính phí vận chuyển của một hệ thống bán hàng khác nhau tùy loại đơn hàng: giao tiêu chuẩn tính theo cân nặng, giao hỏa tốc tính phí cao hơn, còn đơn hàng đủ điều kiện thì được miễn phí vận chuyển. Nếu gộp toàn bộ các công thức này vào một hàm `CalculateShippingFee()` duy nhất bằng `if/switch`, hàm đó sẽ ngày càng phình to khi doanh nghiệp bổ sung thêm hình thức giao hàng mới (giao trong ngày, giao quốc tế...), và rất dễ tính sai phí nếu sửa nhầm nhánh điều kiện của loại vận chuyển khác.

**Ý nghĩa của Strategy trong ví dụ này:** Mỗi công thức tính phí được tách thành một `ConcreteStrategy` riêng — `StandardShipping`, `ExpressShipping`, `FreeShipping` — cùng hiện thực `IShippingFeeStrategy`. `ShippingContext` chỉ gọi `_strategy.Calculate(...)` mà không biết và không cần biết công thức cụ thể bên trong từng loại. Việc chọn `IShippingFeeStrategy` nào dựa trên điều kiện nghiệp vụ (ví dụ đơn hàng đủ lớn thì dùng `FreeShipping`) được thực hiện ngay tại nơi gọi, tách biệt hoàn toàn khỏi logic tính phí bên trong `ShippingContext`; muốn thêm hình thức giao hàng mới chỉ cần thêm một `ConcreteStrategy` mới.

**Cách implementation (C#):**

```csharp
// Strategy
public interface IShippingFeeStrategy
{
    decimal Calculate(decimal orderTotal, decimal weightKg);
}

// ConcreteStrategy
public class StandardShipping : IShippingFeeStrategy
{
    public decimal Calculate(decimal orderTotal, decimal weightKg) => 15000 + weightKg * 3000;
}

public class ExpressShipping : IShippingFeeStrategy
{
    public decimal Calculate(decimal orderTotal, decimal weightKg) => 30000 + weightKg * 5000;
}

public class FreeShipping : IShippingFeeStrategy
{
    public decimal Calculate(decimal orderTotal, decimal weightKg) => 0;
}

// Context
public class ShippingContext
{
    private readonly IShippingFeeStrategy _strategy;

    public ShippingContext(IShippingFeeStrategy strategy)
    {
        _strategy = strategy;
    }

    public decimal CalculateFee(decimal orderTotal, decimal weightKg)
    {
        return _strategy.Calculate(orderTotal, weightKg);
    }
}
```

**Cách sử dụng (C#):**

```csharp
// Chạy thử - chọn Strategy theo điều kiện nghiệp vụ (đơn hàng đủ lớn thì free ship)
public class Program
{
    public static void Main()
    {
        decimal orderTotal = 800000;
        decimal weightKg = 2;

        IShippingFeeStrategy strategy = orderTotal >= 500000
            ? new FreeShipping()
            : new StandardShipping();

        var shippingContext = new ShippingContext(strategy);
        Console.WriteLine($"Phi van chuyen: {shippingContext.CalculateFee(orderTotal, weightKg):N0}"); // 0

        var expressContext = new ShippingContext(new ExpressShipping());
        Console.WriteLine($"Phi van chuyen hoa toc: {expressContext.CalculateFee(orderTotal, weightKg):N0}"); // 40000
    }
}
```

### Ví dụ 3 — Chiến lược lưu trữ file theo môi trường (Local / Cloud)

**Bài toán:** Một hệ thống cần lưu các file do người dùng upload. Khi phát triển trên máy local, file có thể được lưu trực tiếp xuống ổ đĩa để đơn giản và tiết kiệm chi phí. Khi triển khai production trên cloud, hệ thống có thể cần chuyển sang Amazon S3 hoặc Azure Blob Storage. Mục tiêu vẫn là lưu file, nhưng cách kết nối và lưu trữ của từng provider hoàn toàn khác nhau. Nếu `FileService` trực tiếp chứa `if/switch` để xử lý Local, S3, Azure..., class này sẽ ngày càng phụ thuộc vào nhiều storage provider và phải sửa mỗi khi hệ thống bổ sung hoặc thay đổi phương thức lưu trữ.

**Ý nghĩa của Strategy trong ví dụ này:** `IFileStorageStrategy` định nghĩa chung hành vi `Save()`, còn `LocalStorageStrategy`, `S3StorageStrategy` và `AzureBlobStorageStrategy` đóng gói riêng từng cách lưu file. `FileService` chỉ làm việc thông qua `IFileStorageStrategy`, không cần biết file thực tế được lưu xuống ổ đĩa local hay cloud provider nào. Strategy có thể được lựa chọn theo môi trường hoặc configuration khi khởi tạo `FileService`. Khi cần thêm một phương thức lưu trữ mới, chỉ cần thêm một `ConcreteStrategy` mới mà không phải sửa logic của `FileService`.

**Cách implementation (C#):**

```csharp
// Strategy
public interface IFileStorageStrategy
{
    string Save(string fileName);
}

// ConcreteStrategy
public class LocalStorageStrategy : IFileStorageStrategy
{
    public string Save(string fileName)
        => $"Saved {fileName} to Local Storage";
}

public class S3StorageStrategy : IFileStorageStrategy
{
    public string Save(string fileName)
        => $"Saved {fileName} to Amazon S3";
}

// Context
public class FileService
{
    private IFileStorageStrategy _storageStrategy;

    public FileService(IFileStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    public void SetStorageStrategy(IFileStorageStrategy storageStrategy)
    {
        _storageStrategy = storageStrategy;
    }

    public string SaveFile(string fileName)
    {
        return _storageStrategy.Save(fileName);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var fileService = new FileService(
            new LocalStorageStrategy());

        Console.WriteLine(
            fileService.SaveFile("report.pdf"));
        // Saved report.pdf to Local Storage

        // Chuyển sang S3 mà không thay đổi FileService
        fileService.SetStorageStrategy(
            new S3StorageStrategy());

        Console.WriteLine(
            fileService.SaveFile("report.pdf"));
        // Saved report.pdf to Amazon S3
    }
}
```