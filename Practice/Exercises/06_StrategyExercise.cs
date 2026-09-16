namespace Exercises.Strategy;

public interface IDiscountStrategy
{
    decimal Apply(decimal orderTotal);
}

public class NoDiscount : IDiscountStrategy
{
    // TODO: khong giam gia, tra ve nguyen orderTotal
    public decimal Apply(decimal orderTotal)
    {
        throw new NotImplementedException();
    }
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percent;

    public PercentageDiscount(decimal percent)
    {
        _percent = percent;
    }

    // TODO: tra ve orderTotal - (orderTotal * _percent / 100)
    public decimal Apply(decimal orderTotal)
    {
        throw new NotImplementedException();
    }
}

public class FixedAmountDiscount : IDiscountStrategy
{
    private readonly decimal _amount;

    public FixedAmountDiscount(decimal amount)
    {
        _amount = amount;
    }

    // TODO: tra ve Math.Max(0, orderTotal - _amount) - khong duoc am
    public decimal Apply(decimal orderTotal)
    {
        throw new NotImplementedException();
    }
}

public class OrderContext
{
    private IDiscountStrategy _discountStrategy;

    public OrderContext(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    // TODO: gan lai _discountStrategy - day la buoc cho phep doi Strategy tai runtime
    public void SetDiscountStrategy(IDiscountStrategy discountStrategy)
    {
        throw new NotImplementedException();
    }

    // TODO: uy quyen cho _discountStrategy.Apply(orderTotal)
    public decimal CalculateTotal(decimal orderTotal)
    {
        throw new NotImplementedException();
    }
}
