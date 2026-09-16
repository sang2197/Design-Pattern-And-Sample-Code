using Exercises.Singleton;
using Xunit;

namespace Exercises.Tests;

public class SingletonExerciseTests
{
    [Fact]
    public void Instance_AlwaysReturnsSameReference()
    {
        var first = AppSettings.Instance;
        var second = AppSettings.Instance;

        Assert.Same(first, second);
    }

    [Fact]
    public void Instance_IsOnlyConstructedOnce()
    {
        var first = AppSettings.Instance;
        var second = AppSettings.Instance;

        Assert.Equal(1, second.InitCount);
        Assert.Equal("Server=localhost;Database=MyApp;", first.ConnectionString);
    }
}
