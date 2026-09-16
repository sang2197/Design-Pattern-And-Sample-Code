using Exercises.Observer.Example2;
using Xunit;

namespace Observer.Tests;

public class Example2Tests
{
    [Fact]
    public void SetPrice_NotifiesAllSubscribedInvestors()
    {
        var stock = new Stock("VNM", 80000);
        var investor1 = new Investor("Nam");
        var investor2 = new Investor("Lan");

        stock.Subscribe(investor1);
        stock.Subscribe(investor2);

        stock.SetPrice(82000);

        Assert.Equal("VNM vua doi gia: 82,000", investor1.Notifications[0]);
        Assert.Equal("VNM vua doi gia: 82,000", investor2.Notifications[0]);
    }
}
