namespace Exercises.Strategy.Example1;

// Strategy - Vi du 1: Chien luoc giam gia don hang, doi Strategy ngay tai runtime
// Xem lai: Strategy-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IDiscountStrategy (Strategy) { decimal Apply(decimal orderTotal); }
// - class NoDiscount : IDiscountStrategy -> Apply() tra ve nguyen orderTotal
// - class PercentageDiscount : IDiscountStrategy
//     constructor nhan decimal percent
//     Apply(orderTotal) -> orderTotal - (orderTotal * _percent / 100)
// - class FixedAmountDiscount : IDiscountStrategy
//     constructor nhan decimal amount
//     Apply(orderTotal) -> Math.Max(0, orderTotal - _amount)
// - class OrderContext (Context)
//     constructor nhan IDiscountStrategy discountStrategy
//     SetDiscountStrategy(IDiscountStrategy) -> gan lai _discountStrategy (cho phep doi Strategy tai runtime)
//     CalculateTotal(orderTotal) -> uy quyen cho _discountStrategy.Apply(orderTotal)

// Strategy
public interface IDiscount
{
    decimal Apply(decimal orderTotal);
}

// Concrete Strategy
public class NoDiscount : IDiscount
{
    public decimal Apply(decimal orderTotal) => orderTotal;
}

public class PercentageDiscount : IDiscount
{
    private readonly decimal _percent;
    public PercentageDiscount(decimal percent)
    {
        _percent = percent;
    }

    public decimal Apply(decimal orderTotal) => orderTotal * (1 - _percent / 100);
}

public class FixAmountDiscount : IDiscount
{
    private readonly decimal _amount;
    public FixAmountDiscount(decimal amount)
    {
        _amount = amount;
    }

    public decimal Apply(decimal orderTotal) => orderTotal - _amount;
}

// Context
public class OrderContext
{
    private IDiscount _discount;
    public OrderContext(IDiscount discount)
    {
        _discount = discount;
    }

    public decimal Calculate(decimal orderTotal)
    {
        return _discount.Apply(orderTotal);
    }
}

// Cách dùng
public class Program
{
    public void Main()
    {
        var noDiscount = new OrderContext(new NoDiscount());
        Console.WriteLine(noDiscount.Calculate(500000));

        var percentageDiscount = new OrderContext(new PercentageDiscount(20));
        Console.WriteLine(percentageDiscount.Calculate(500000));

        var fixAmountDiscount = new OrderContext(new FixAmountDiscount(80000));
        Console.WriteLine(fixAmountDiscount.Calculate(500000));
    }
}