using Exercises.FactoryMethod.Example1;
using Xunit;

namespace FactoryMethod.Tests;

public class Example1Tests
{
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
