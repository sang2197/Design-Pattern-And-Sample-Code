using Exercises.Strategy.Example3;
using Xunit;

namespace Strategy.Tests;

public class Example3Tests
{
    [Fact]
    public void NumberSorter_CanSwitchStrategyAtRuntime()
    {
        var numbers = new List<int> { 5, 2, 8, 1, 9 };
        var sorter = new NumberSorter(new AscendingSortStrategy());

        Assert.Equal(new List<int> { 1, 2, 5, 8, 9 }, sorter.Sort(numbers));

        sorter.SetStrategy(new DescendingSortStrategy());
        Assert.Equal(new List<int> { 9, 8, 5, 2, 1 }, sorter.Sort(numbers));
    }
}
