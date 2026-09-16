using Exercises.Singleton.Example2;
using Xunit;

namespace Singleton.Tests;

public class Example2Tests
{
    [Fact]
    public void Increment_AccumulatesAcrossAllCallers()
    {
        int before = RequestCounter.Instance.GetCount();

        RequestCounter.Instance.Increment();
        RequestCounter.Instance.Increment();
        RequestCounter.Instance.Increment();

        Assert.Equal(before + 3, RequestCounter.Instance.GetCount());
    }
}
