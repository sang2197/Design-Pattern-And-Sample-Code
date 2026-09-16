using Exercises.Decorator.Example1;
using Xunit;

namespace Decorator.Tests;

public class Example1Tests
{
    [Fact]
    public void CombiningDecorators_StacksInWrapOrder()
    {
        ICoffee coffee = new SugarDecorator(new MilkDecorator(new SimpleCoffee()));

        Assert.Equal("Coffee + Milk + Sugar", coffee.Describe());
        Assert.Equal(27000, coffee.Cost());
    }

    [Fact]
    public void SingleDecorator_AddsOnlyItsOwnPart()
    {
        ICoffee coffee = new MilkDecorator(new SimpleCoffee());

        Assert.Equal("Coffee + Milk", coffee.Describe());
        Assert.Equal(25000, coffee.Cost());
    }
}
