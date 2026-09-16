using Exercises.Mediator.Example3;
using Xunit;

namespace Mediator.Tests;

public class Example3Tests
{
    [Fact]
    public void SecondPlane_MustWaitUntilRunwayIsFree()
    {
        var tower = new ControlTower();
        var vn123 = new PassengerPlane(tower, "VN123");
        var vn456 = new PassengerPlane(tower, "VN456");

        Assert.True(vn123.RequestLanding());
        Assert.False(vn456.RequestLanding());

        vn123.Land();
        Assert.True(vn456.RequestLanding());
    }
}
