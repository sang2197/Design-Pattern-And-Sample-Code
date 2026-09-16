using Exercises.Decorator.Example2;
using Xunit;

namespace Decorator.Tests;

public class Example2Tests
{
    [Fact]
    public void CombiningPrefixAndUpperCase_AppliesBothInWrapOrder()
    {
        INotifier notifier = new UpperCaseDecorator(new PrefixDecorator(new BasicNotifier(), "[INFO]"));
        Assert.Equal("[INFO] DON HANG DA DUOC XAC NHAN", notifier.Send("don hang da duoc xac nhan"));
    }
}
