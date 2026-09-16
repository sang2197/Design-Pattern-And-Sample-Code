using Exercises.Singleton.Example3;
using Xunit;

namespace Singleton.Tests;

public class Example3Tests
{
    [Fact]
    public void MultipleServices_ShareSameLoggerInstance()
    {
        int before = AppLogger.Instance.LogCount;

        var orderService = new OrderService();
        var paymentService = new PaymentService();

        orderService.CreateOrder("DH0001");
        paymentService.Pay("DH0001");

        Assert.Equal(before + 2, AppLogger.Instance.LogCount);
    }
}
