# Strategy Pattern

## 1. Khái niệm

**Strategy** là một Behavioral Design Pattern, dùng để đóng gói mỗi **giải thuật/cách xử lý** thành một class riêng nhưng cùng hiện thực một interface, Nhờ đó có thể thay đổi cách xử lý tại runtime mà không cần sửa code của class đang sử dụng Strategy.

Pattern gồm 3 thành phần:

- **Strategy (interface):** khai báo method chung mà mọi giải thuật phải hiện thực (ví dụ `Calculate()`).
- **ConcreteStrategy:** hiện thực một giải thuật/cách xử lý cụ thể.
- **Context:** giữ một Strategy và gọi nó qua interface, không cần biết ConcreteStrategy cụ thể bên trong.

## 2. Ý nghĩa

- **Không dùng Strategy:** khi có nhiều cách xử lý cho cùng một bài toán, code thường xuất hiện nhiều if/switch trong cùng một method. Mỗi khi thêm cách xử lý mới phải sửa method cũ, làm code khó mở rộng và khó test riêng từng giải thuật.
- **Strategy giải quyết bằng cách tách mỗi giải thuật ra một class riêng:** `Context` chỉ gọi `_strategy.Execute(...)`, còn Strategy cụ thể được truyền từ bên ngoài qua constructor, DI hoặc thay đổi tại runtime.
- Tuân thủ **Open/Closed Principle**: thêm giải thuật mới chỉ cần thêm 1 ConcreteStrategy, không sửa `Context` đang có.
- Có thể **đổi giải thuật ngay tại runtime** (ví dụ đổi chiến lược tính giá theo loại khách hàng) bằng cách gán lại Strategy khác cho Context, không cần sửa code, không cần deploy lại logic cũ.
- Dễ **unit test** từng giải thuật độc lập, vì mỗi ConcreteStrategy là một class riêng biệt, không phụ thuộc các giải thuật khác.

## 3. Code mẫu

### Ví dụ 1 — Strategy interface và ConcreteStrategy

```csharp
public interface IDiscountStrategy
{
    decimal Apply(decimal orderTotal);
}

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
```

### Ví dụ 2 — Context sử dụng Strategy

```csharp
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

### Ví dụ 3 — Chọn Strategy theo điều kiện nghiệp vụ tại nơi gọi

```csharp
public class CheckoutService
{
    public decimal Checkout(decimal orderTotal, string customerType)
    {
        IDiscountStrategy strategy = customerType switch
        {
            "vip" => new PercentageDiscount(20),
            "member" => new FixedAmountDiscount(50000),
            _ => new NoDiscount()
        };

        var context = new OrderContext(strategy);
        return context.CalculateTotal(orderTotal);
    }
}
```
