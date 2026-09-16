using Exercises.Adapter.Example3;
using Xunit;

namespace Adapter.Tests;

public class Example3Tests
{
    [Fact]
    public void Pay_ConvertsAmountToCents_BeforeCallingSdk()
    {
        IPaymentGateway gateway = new ForeignPaymentAdapter(new ForeignPaymentSdk());
        Assert.Equal("[ForeignSDK] Charged 1999 USD cents", gateway.Pay(19.99m));
    }
}
