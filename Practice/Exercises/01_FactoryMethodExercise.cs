namespace Exercises.FactoryMethod;

public interface IPaymentMethod
{
    string Pay(decimal amount);
}

// TODO: hien thuc Pay() -> "Thanh toan {amount:N0} qua VNPay"
public class VnPayPayment : IPaymentMethod
{
    public string Pay(decimal amount)
    {
        throw new NotImplementedException();
    }
}

// TODO: hien thuc Pay() -> "Thanh toan {amount:N0} qua Momo"
public class MomoPayment : IPaymentMethod
{
    public string Pay(decimal amount)
    {
        throw new NotImplementedException();
    }
}

public abstract class CheckoutProcessor
{
    // Factory Method - lop con quyet dinh tao IPaymentMethod nao
    protected abstract IPaymentMethod CreatePaymentMethod();

    // Logic dung chung - KHONG duoc sua, chi phu thuoc abstraction IPaymentMethod
    public string Checkout(decimal amount)
    {
        var payment = CreatePaymentMethod();
        return $"[Checkout] {payment.Pay(amount)}";
    }
}

// TODO: override CreatePaymentMethod() de tra ve VnPayPayment
public class VnPayCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePaymentMethod()
    {
        throw new NotImplementedException();
    }
}

// TODO: override CreatePaymentMethod() de tra ve MomoPayment
public class MomoCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePaymentMethod()
    {
        throw new NotImplementedException();
    }
}
