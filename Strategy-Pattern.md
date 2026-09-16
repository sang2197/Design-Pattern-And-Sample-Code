# Strategy Pattern

## 1. Khái niệm cơ bản

**Strategy** là một Behavioral Design Pattern, dùng để đóng gói mỗi **giải thuật/cách xử lý** thành một class riêng nhưng cùng hiện thực một interface, nhờ đó có thể thay đổi cách xử lý tại runtime mà không cần sửa code của class đang sử dụng Strategy.

Pattern gồm 3 thành phần:

- **Strategy (interface):** khai báo method chung mà mọi giải thuật phải hiện thực (ví dụ `Calculate()`).
- **ConcreteStrategy:** hiện thực một giải thuật/cách xử lý cụ thể.
- **Context:** giữ một Strategy và gọi nó qua interface, không cần biết ConcreteStrategy cụ thể bên trong.

## 2. Bài toán

Khi có nhiều cách xử lý khác nhau cho cùng một bài toán (nhiều loại khuyến mãi, nhiều loại phí vận chuyển, nhiều kiểu sắp xếp...), cách làm thường gặp là viết một method lớn chứa `if/switch` liệt kê toàn bộ trường hợp:

- Mỗi khi thêm một cách xử lý mới, phải **sửa lại chính method đó**, vi phạm Open/Closed Principle.
- Các giải thuật khác nhau bị **trộn lẫn trong cùng một class**, khiến khó đọc và khó test riêng từng giải thuật.
- Không thể **đổi giải thuật đang áp dụng cho một object đã tồn tại** tại runtime mà không sửa code.

## 3. Ý nghĩa của Strategy

- **Strategy giải quyết bằng cách tách mỗi giải thuật ra một class riêng:** `Context` chỉ gọi `_strategy.Apply(...)`, còn Strategy cụ thể được truyền từ bên ngoài qua constructor, DI hoặc thay đổi tại runtime.
- Tuân thủ **Open/Closed Principle**: thêm giải thuật mới chỉ cần thêm 1 ConcreteStrategy, không sửa `Context` đang có.
- Có thể **đổi giải thuật ngay tại runtime** (ví dụ đổi chiến lược tính giá theo loại khách hàng) bằng cách gán lại Strategy khác cho Context, không cần sửa code, không cần deploy lại logic cũ.
- Dễ **unit test** từng giải thuật độc lập, vì mỗi ConcreteStrategy là một class riêng biệt, không phụ thuộc các giải thuật khác.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: có Strategy, ConcreteStrategy, Context, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Chiến lược giảm giá đơn hàng, đổi Strategy ngay tại runtime

**Khi nào dùng:** loại giảm giá áp dụng cho một đơn hàng có thể thay đổi giữa chừng (phát hiện khách VIP, áp voucher...) trong khi `OrderContext` đã được tạo từ trước.

**Cách sử dụng:** truyền `IDiscountStrategy` vào `OrderContext` lúc khởi tạo, và có thể gọi `SetDiscountStrategy()` bất kỳ lúc nào để đổi cách tính mà không cần tạo lại `OrderContext`.

**Cách hiện thực:** `OrderContext` chỉ giữ một tham chiếu `IDiscountStrategy` và ủy quyền toàn bộ phép tính cho `Apply()`; không có `if/switch` nào theo loại giảm giá bên trong `Context`.

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

// Chạy thử
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

**Khi nào dùng:** cách tính phí vận chuyển khác nhau tùy loại đơn hàng (free ship khi đủ điều kiện, giao hỏa tốc tính phí riêng...), và có thể bổ sung thêm loại phí mới trong tương lai.

**Cách sử dụng:** chọn đúng `IShippingFeeStrategy` dựa trên điều kiện nghiệp vụ (ở đây là ngay tại nơi gọi), rồi truyền vào `ShippingContext` để tính phí.

**Cách hiện thực:** mỗi `ConcreteStrategy` (`StandardShipping`, `ExpressShipping`, `FreeShipping`) tự chứa công thức tính riêng; `ShippingContext` không biết và không cần biết công thức bên trong từng loại.

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

### Ví dụ 3 — Chiến lược sắp xếp danh sách (tăng dần / giảm dần)

**Khi nào dùng:** cùng một danh sách số nhưng cần sắp xếp theo nhiều tiêu chí khác nhau tùy lựa chọn của người dùng (tăng dần, giảm dần...).

**Cách sử dụng:** gán `ISortStrategy` phù hợp cho `NumberSorter` qua constructor hoặc `SetStrategy()`, rồi gọi `Sort()` như nhau bất kể đang dùng chiến lược nào.

**Cách hiện thực:** mỗi `ConcreteStrategy` chỉ khác nhau ở biểu thức LINQ (`OrderBy`/`OrderByDescending`); `NumberSorter` chỉ đơn thuần ủy quyền `Sort()` cho `_strategy` đang giữ.

```csharp
// Strategy
public interface ISortStrategy
{
    List<int> Sort(List<int> numbers);
}

// ConcreteStrategy
public class AscendingSortStrategy : ISortStrategy
{
    public List<int> Sort(List<int> numbers) => numbers.OrderBy(x => x).ToList();
}

public class DescendingSortStrategy : ISortStrategy
{
    public List<int> Sort(List<int> numbers) => numbers.OrderByDescending(x => x).ToList();
}

// Context
public class NumberSorter
{
    private ISortStrategy _strategy;

    public NumberSorter(ISortStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(ISortStrategy strategy)
    {
        _strategy = strategy;
    }

    public List<int> Sort(List<int> numbers) => _strategy.Sort(numbers);
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var numbers = new List<int> { 5, 2, 8, 1, 9 };

        var sorter = new NumberSorter(new AscendingSortStrategy());
        Console.WriteLine(string.Join(", ", sorter.Sort(numbers))); // 1, 2, 5, 8, 9

        sorter.SetStrategy(new DescendingSortStrategy());
        Console.WriteLine(string.Join(", ", sorter.Sort(numbers))); // 9, 8, 5, 2, 1
    }
}
```
