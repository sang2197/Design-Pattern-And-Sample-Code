using Exercises.FactoryMethod;
using Xunit;

namespace Exercises.Tests;

public class FactoryMethodExerciseTests
{
    [Fact]
    public void VnPayPayment_Pay_ReturnsVnPayMessage()
    {
        IPaymentMethod payment = new VnPayPayment();
        Assert.Equal("Thanh toan 100,000 qua VNPay", payment.Pay(100000));
    }

    [Fact]
    public void MomoPayment_Pay_ReturnsMomoMessage()
    {
        IPaymentMethod payment = new MomoPayment();
        Assert.Equal("Thanh toan 50,000 qua Momo", payment.Pay(50000));
    }

    [Fact]
    public void VnPayCheckoutProcessor_Checkout_UsesVnPayPayment()
    {
        CheckoutProcessor processor = new VnPayCheckoutProcessor();
        Assert.Equal("[Checkout] Thanh toan 100,000 qua VNPay", processor.Checkout(100000));
    }

    [Fact]
    public void MomoCheckoutProcessor_Checkout_UsesMomoPayment()
    {
        CheckoutProcessor processor = new MomoCheckoutProcessor();
        Assert.Equal("[Checkout] Thanh toan 50,000 qua Momo", processor.Checkout(50000));
    }
}
