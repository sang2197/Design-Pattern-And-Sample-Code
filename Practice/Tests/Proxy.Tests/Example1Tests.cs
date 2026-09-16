using Exercises.Proxy.Example1;
using Xunit;

namespace Proxy.Tests;

public class Example1Tests
{
    [Fact]
    public void Display_DoesNotLoadRealImage_UntilFirstCall()
    {
        int before = RealImage.LoadCount;
        IImage image = new ImageProxy("unit-test-photo.png");

        Assert.Equal(before, RealImage.LoadCount);

        image.Display();

        Assert.Equal(before + 1, RealImage.LoadCount);
    }

    [Fact]
    public void Display_ReusesSameRealImage_OnSubsequentCalls()
    {
        int before = RealImage.LoadCount;
        IImage image = new ImageProxy("unit-test-photo-2.png");

        image.Display();
        image.Display();
        image.Display();

        Assert.Equal(before + 1, RealImage.LoadCount);
    }
}
