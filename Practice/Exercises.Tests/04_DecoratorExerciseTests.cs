using Exercises.Decorator;
using Xunit;

namespace Exercises.Tests;

public class DecoratorExerciseTests
{
    [Fact]
    public void MilkDecorator_AddsMilkDescriptionAndCost()
    {
        ICoffee coffee = new MilkDecorator(new SimpleCoffee());

        Assert.Equal("Coffee + Milk", coffee.Describe());
        Assert.Equal(25000, coffee.Cost());
    }

    [Fact]
    public void SugarDecorator_AddsSugarDescriptionAndCost()
    {
        ICoffee coffee = new SugarDecorator(new SimpleCoffee());

        Assert.Equal("Coffee + Sugar", coffee.Describe());
        Assert.Equal(22000, coffee.Cost());
    }

    [Fact]
    public void CombiningDecorators_StacksInWrapOrder()
    {
        ICoffee coffee = new SugarDecorator(new MilkDecorator(new SimpleCoffee()));

        Assert.Equal("Coffee + Milk + Sugar", coffee.Describe());
        Assert.Equal(27000, coffee.Cost());
    }
}
