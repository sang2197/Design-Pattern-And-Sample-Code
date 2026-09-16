using Exercises.Singleton.Example1;
using Xunit;

namespace Singleton.Tests;

public class Example1Tests
{
    [Fact]
    public void Instance_AlwaysReturnsSameReference()
    {
        Assert.Same(AppSettings.Instance, AppSettings.Instance);
    }

    [Fact]
    public void Instance_IsOnlyConstructedOnce()
    {
        var first = AppSettings.Instance;
        Assert.Equal(1, AppSettings.Instance.InitCount);
        Assert.Equal("Server=localhost;Database=MyApp;", first.ConnectionString);
    }
}
