using Exercises.Builder.Example3;
using Xunit;

namespace Builder.Tests;

public class Example3Tests
{
    [Fact]
    public void Director_MakeMargherita_BuildsExpectedPizza()
    {
        var director = new PizzaMenuDirector();
        Pizza pizza = director.MakeMargherita(new PizzaBuilder());

        Assert.Equal("Pizza M: Tomato, Mozzarella", pizza.Describe());
    }

    [Fact]
    public void Director_MakePepperoniSupreme_BuildsExpectedPizzaWithExtraCheese()
    {
        var director = new PizzaMenuDirector();
        Pizza pizza = director.MakePepperoniSupreme(new PizzaBuilder());

        Assert.Equal("Pizza L: Pepperoni, Mushroom + extra cheese", pizza.Describe());
    }
}
