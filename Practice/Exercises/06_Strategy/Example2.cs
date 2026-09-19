namespace Exercises.Strategy.Example2;

// Strategy - Vi du 2: Chon chien luoc tinh phi van chuyen theo dieu kien don hang
// Xem lai: Strategy-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IShippingFeeStrategy (Strategy) { decimal Calculate(decimal orderTotal, decimal weightKg); }
// - class StandardShipping : IShippingFeeStrategy -> Calculate() -> 15000 + weightKg * 3000
// - class ExpressShipping : IShippingFeeStrategy -> Calculate() -> 30000 + weightKg * 5000
// - class FreeShipping : IShippingFeeStrategy -> Calculate() -> 0
// - class ShippingContext (Context)
//     constructor nhan IShippingFeeStrategy strategy
//     CalculateFee(orderTotal, weightKg) -> uy quyen cho _strategy.Calculate(orderTotal, weightKg)

// Strategy
public interface IShippingFee
{
    decimal Calculate(decimal weight);
}

// Concrete Strategy
public class FreeShipping : IShippingFee
{
    public decimal Calculate(decimal weight) => 0;
}

public class StandardShipping : IShippingFee
{
    public decimal Calculate(decimal weight) => 12000 + weight * 2000;
}

public class ExpressShipping : IShippingFee
{
    public decimal Calculate(decimal weight) => 15000 + weight * 5000;
}

// Context
public class ShippingContext
{
    private IShippingFee _strategy;
    public ShippingContext(IShippingFee strategy)
    {
        _strategy = strategy;
    }

    public decimal CalculateFee(decimal weight)
    {
        return _strategy.Calculate(weight);
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var freeShipping = new ShippingContext(new FreeShipping());
        Console.WriteLine(freeShipping.CalculateFee(10));

        var standardShipping = new ShippingContext(new StandardShipping());
        Console.WriteLine(standardShipping.CalculateFee(20));

        var expressShipping = new ShippingContext(new ExpressShipping());
        Console.WriteLine(expressShipping.CalculateFee(20));
    }
}