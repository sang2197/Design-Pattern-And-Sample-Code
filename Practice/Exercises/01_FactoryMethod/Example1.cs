namespace Exercises.FactoryMethod.Example1;

// Factory Method - Vi du 1: Xu ly thanh toan theo phuong thuc (VNPay / Momo)
// Xem lai: Factory-Method-Pattern.md - Vi du 1
//
// Viet lai TU DAU (khong xem code mau) cac thanh phan sau, dung EXACT ten va chu ky nhu duoi day
// de test bien dich va chay duoc:
//
// - interface IPaymentMethod { string Pay(decimal amount); }
// - class VnPayPayment : IPaymentMethod
//     Pay() tra ve "Thanh toan {amount:N0} qua VNPay"
// - class MomoPayment : IPaymentMethod
//     Pay() tra ve "Thanh toan {amount:N0} qua Momo"
// - abstract class CheckoutProcessor
//     protected abstract IPaymentMethod CreatePaymentMethod();
//     public string Checkout(decimal amount) -> goi CreatePaymentMethod() roi tra ve "[Checkout] {ket qua Pay()}"
// - class VnPayCheckoutProcessor : CheckoutProcessor -> CreatePaymentMethod() tra ve VnPayPayment
// - class MomoCheckoutProcessor : CheckoutProcessor -> CreatePaymentMethod() tra ve MomoPayment

// Product
public interface IPaymentMethod
{
    string Pay(decimal amount);
}

// Concrete Product
public class VnPayPaymentMethod : IPaymentMethod
{
    public string Pay(decimal amount) => $"Da thanh toan {amount:N0} qua VNPay";
}

public class MomoPaymentMethod : IPaymentMethod
{
    public string Pay(decimal amount) => $"Da thanh toan {amount:N0} qua MoMo";
}

// Creator
public abstract class CheckoutProcessor
{
    protected abstract IPaymentMethod CreatePayment();
    public string Checkout(decimal amount)
    {
        var payment = CreatePayment();
        return payment.Pay(amount);
    }
}

// Concrete Creator
public class VnPayCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePayment() => new VnPayPaymentMethod();
}

public class MomoCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePayment() => new MomoPaymentMethod();
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var vnPay = new VnPayCheckoutProcessor();
        var momo = new MomoCheckoutProcessor();

        Console.WriteLine(vnPay.Checkout(100000));
        Console.WriteLine(momo.Checkout(200000));
    }
}