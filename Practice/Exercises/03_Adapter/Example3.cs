namespace Exercises.Adapter.Example3;

// Adapter - Vi du 3: Thanh toan qua SDK nuoc ngoai (khac don vi tinh)
// Xem lai: Adapter-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IPaymentGateway (Target) { string Pay(decimal amount); }
// - class ForeignPaymentSdk (Adaptee)
//     string Charge(int amountInCents, string currency) -> tra ve "[ForeignSDK] Charged {amountInCents} {currency} cents"
// - class ForeignPaymentAdapter : IPaymentGateway (Adapter)
//     constructor nhan ForeignPaymentSdk
//     Pay(decimal amount) -> quy doi amount (vd 19.99) sang amountInCents (int, vd 1999) bang (int)(amount * 100),
//       roi goi _sdk.Charge(amountInCents, "USD")

// Target
public interface IPayment
{
    void Pay(decimal amount);
}

// Adaptee
public class ForeignPaymentSDK
{
    public void Charge(int amountInCents, string currency)
    {
        Console.WriteLine($"[ForeignSDK] Charged {amountInCents} {currency} cents");
    }
}

// Adapter
public class ForeignPaymentAdapter : IPayment
{
    private readonly ForeignPaymentSDK _foreignPayment;
    public ForeignPaymentAdapter(ForeignPaymentSDK foreignPayment)
    {
        _foreignPayment = foreignPayment;
    }

    public void Pay(decimal amount)
    {
        int amountInCents = (int)(amount * 100);
        _foreignPayment.Charge(amountInCents, "USD");
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        IPayment payment = new ForeignPaymentAdapter(new ForeignPaymentSDK());
        payment.Pay(12.99M);
    }
}