using Exercises.Strategy;
using Xunit;

namespace Exercises.Tests;

public class StrategyExerciseTests
{
    [Fact]
    public void NoDiscount_ReturnsOriginalTotal()
    {
        IDiscountStrategy strategy = new NoDiscount();
        Assert.Equal(500000, strategy.Apply(500000));
    }

    [Fact]
    public void PercentageDiscount_SubtractsPercentOfTotal()
    {
        IDiscountStrategy strategy = new PercentageDiscount(20);
        Assert.Equal(400000, strategy.Apply(500000));
    }

    [Fact]
    public void FixedAmountDiscount_SubtractsFixedAmount_NeverBelowZero()
    {
        IDiscountStrategy strategy = new FixedAmountDiscount(50000);
        Assert.Equal(450000, strategy.Apply(500000));
        Assert.Equal(0, strategy.Apply(30000));
    }

    [Fact]
    public void OrderContext_SetDiscountStrategy_ChangesResultAtRuntime()
    {
        var context = new OrderContext(new NoDiscount());
        Assert.Equal(500000, context.CalculateTotal(500000));

        context.SetDiscountStrategy(new PercentageDiscount(20));
        Assert.Equal(400000, context.CalculateTotal(500000));
    }
}
