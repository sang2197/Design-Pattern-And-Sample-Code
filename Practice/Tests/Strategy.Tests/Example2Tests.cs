using Exercises.Strategy.Example2;
using Xunit;

namespace Strategy.Tests;

public class Example2Tests
{
    [Fact]
    public void FreeShipping_AlwaysReturnsZero()
    {
        var context = new ShippingContext(new FreeShipping());
        Assert.Equal(0, context.CalculateFee(800000, 2));
    }

    [Fact]
    public void ExpressShipping_CalculatesBaseFeePlusWeight()
    {
        var context = new ShippingContext(new ExpressShipping());
        Assert.Equal(40000, context.CalculateFee(800000, 2));
    }

    [Fact]
    public void StandardShipping_CalculatesBaseFeePlusWeight()
    {
        var context = new ShippingContext(new StandardShipping());
        Assert.Equal(21000, context.CalculateFee(100000, 2));
    }
}
