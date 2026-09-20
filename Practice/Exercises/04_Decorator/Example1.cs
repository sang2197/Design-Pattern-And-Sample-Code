namespace Exercises.Decorator.Example1;

// Decorator - Vi du 1: Goi mon ca phe, them topping tu do (Milk / Sugar)
// Xem lai: Decorator-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ICoffee (Component) { string Describe(); decimal Cost(); }
// - class SimpleCoffee : ICoffee (ConcreteComponent) -> Describe() "Coffee", Cost() 20000
// - abstract class CoffeeDecorator : ICoffee (Decorator)
//     protected readonly ICoffee Inner; constructor nhan ICoffee inner, gan vao Inner
//     virtual Describe() => Inner.Describe(); virtual Cost() => Inner.Cost();
// - class MilkDecorator : CoffeeDecorator (ConcreteDecorator)
//     Describe() -> "{Inner.Describe()} + Milk"; Cost() -> Inner.Cost() + 5000
// - class SugarDecorator : CoffeeDecorator (ConcreteDecorator)
//     Describe() -> "{Inner.Describe()} + Sugar"; Cost() -> Inner.Cost() + 2000


// Component
public interface ICoffee
{
    string Describe();
    decimal Cost();
}

// Concrete Component
public class SimpleCoffee : ICoffee
{
    public string Describe() => "Coffee";
    public decimal Cost() => 20000;
}

// Decorator
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee Inner;
    public CoffeeDecorator(ICoffee inner)
    {
        Inner = inner;
    }

    public virtual string Describe() => Inner.Describe();

    public virtual decimal Cost() => Inner.Cost();
}

// Concrete Decorator
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee inner) : base(inner)
    {
    }

    public override string Describe() => $"{Inner.Describe()} + Milk";
    public override decimal Cost() => Inner.Cost() + 8000;
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee inner) : base(inner)
    {
    }

    public override string Describe() => $"{Inner.Describe()} + Sugar";
    public override decimal Cost() => Inner.Cost() + 5000;
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        ICoffee coffee = new SugarDecorator(new MilkDecorator(new SimpleCoffee()));
        Console.WriteLine(coffee.Describe());
        Console.WriteLine(coffee.Cost());
    }
}