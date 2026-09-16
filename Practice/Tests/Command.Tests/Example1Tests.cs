using Exercises.Command.Example1;
using Xunit;

namespace Command.Tests;

public class Example1Tests
{
    [Fact]
    public void RemoteControl_CanSwapCommandAtRuntime()
    {
        var light = new Light();
        var remote = new RemoteControl();

        remote.SetCommand(new TurnOnLightCommand(light));
        remote.PressButton();
        Assert.True(light.IsOn);

        remote.SetCommand(new TurnOffLightCommand(light));
        remote.PressButton();
        Assert.False(light.IsOn);
    }
}
