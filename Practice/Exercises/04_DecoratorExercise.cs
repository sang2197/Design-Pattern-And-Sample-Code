namespace Exercises.Decorator;

public interface ICoffee
{
    string Describe();
    decimal Cost();
}

public class SimpleCoffee : ICoffee
{
    public string Describe() => "Coffee";
    public decimal Cost() => 20000;
}

public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee Inner;

    protected CoffeeDecorator(ICoffee inner)
    {
        Inner = inner;
    }

    public virtual string Describe() => Inner.Describe();
    public virtual decimal Cost() => Inner.Cost();
}

public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee inner) : base(inner)
    {
    }

    // TODO: Describe() -> "{Inner.Describe()} + Milk"
    public override string Describe()
    {
        throw new NotImplementedException();
    }

    // TODO: Cost() -> Inner.Cost() + 5000
    public override decimal Cost()
    {
        throw new NotImplementedException();
    }
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee inner) : base(inner)
    {
    }

    // TODO: Describe() -> "{Inner.Describe()} + Sugar"
    public override string Describe()
    {
        throw new NotImplementedException();
    }

    // TODO: Cost() -> Inner.Cost() + 2000
    public override decimal Cost()
    {
        throw new NotImplementedException();
    }
}
