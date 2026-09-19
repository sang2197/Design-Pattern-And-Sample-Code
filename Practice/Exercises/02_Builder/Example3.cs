namespace Exercises.Builder.Example3;

// Builder - Vi du 3: Builder kem Director dung san cong thuc Pizza
// Xem lai: Builder-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - class Pizza (Product)
//     string Size { get; set; } = "", List<string> Toppings { get; } = new List<string>(), bool ExtraCheese { get; set; }
//     string Describe() -> "Pizza {Size}: {toppings, hoac 'khong topping' neu rong}{' + extra cheese' neu ExtraCheese}"
// - class PizzaBuilder
//     WithSize(string), AddTopping(string), WithExtraCheese() -> return this
//     Build() -> tra ve pizza
// - class PizzaMenuDirector
//     MakeMargherita(PizzaBuilder builder) -> Size "M", topping "Tomato", "Mozzarella"
//     MakePepperoniSupreme(PizzaBuilder builder) -> Size "L", topping "Pepperoni", "Mushroom", co ExtraCheese

// Product
public class Pizza
{
    public string Size { get; set; }
    public List<string> Toppings { get; } = new List<string>();
    public bool? ExtraCheese { get; set; }
}

// Builder
public class PizzaBuilder
{
    private readonly Pizza pizza = new Pizza();

    public PizzaBuilder WithSize(string size)
    {
        pizza.Size = size;
        return this;
    }

    public PizzaBuilder AddTopping(string topping)
    {
        pizza.Toppings.Add(topping);
        return this;
    }

    public PizzaBuilder WithExtraCheese()
    {
        pizza.ExtraCheese = true;
        return this;
    }

    public Pizza Build()
    {
        return pizza;
    }
}

// Director
public class PizzaMenuDirector
{
    public Pizza MediumPizza(PizzaBuilder builder)
    {
        return builder
            .WithSize("M")
            .AddTopping("Mozzarella")
            .Build();
    }

    public Pizza LagrePizza(PizzaBuilder builder)
    {
        return builder
            .WithSize("L")
            .AddTopping("Tomato")
            .AddTopping("Pepperoni")
            .WithExtraCheese()
            .Build();
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var pizzaMenu = new PizzaMenuDirector();

        var mediumPizza = pizzaMenu.MediumPizza(new PizzaBuilder());
        var largePizza = pizzaMenu.LagrePizza(new PizzaBuilder());
    }
}