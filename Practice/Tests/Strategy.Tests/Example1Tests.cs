using Exercises.Strategy.Example1;
using Xunit;

namespace Strategy.Tests;

public class Example1Tests
{
    [Fact]
    public void OrderContext_SetDiscountStrategy_ChangesResultAtRuntime()
    {
        var context = new OrderContext(new NoDiscount());
        Assert.Equal(500000, context.CalculateTotal(500000));

        context.SetDiscountStrategy(new PercentageDiscount(20));
        Assert.Equal(400000, context.CalculateTotal(500000));

        context.SetDiscountStrategy(new FixedAmountDiscount(50000));
        Assert.Equal(450000, context.CalculateTotal(500000));
    }

    [Fact]
    public void FixedAmountDiscount_NeverGoesBelowZero()
    {
        IDiscountStrategy strategy = new FixedAmountDiscount(50000);
        Assert.Equal(0, strategy.Apply(30000));
    }
}
