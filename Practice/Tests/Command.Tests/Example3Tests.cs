using Exercises.Command.Example3;
using Xunit;

namespace Command.Tests;

public class Example3Tests
{
    [Fact]
    public void MacroCommand_ExecutesAllInnerCommandsInOrder()
    {
        var tv = new Television();
        var ac = new AirConditioner();

        ICommand watchMovieMode = new MacroCommand(new List<ICommand>
        {
            new TurnOnTvCommand(tv),
            new TurnOnAcCommand(ac)
        });

        watchMovieMode.Execute();

        Assert.True(tv.IsOn);
        Assert.True(ac.IsOn);
    }
}
