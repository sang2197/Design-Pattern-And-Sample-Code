using Exercises.Command;
using Xunit;

namespace Exercises.Tests;

public class CommandExerciseTests
{
    [Fact]
    public void RemoteControl_WithTurnOnCommand_TurnsLightOn()
    {
        var light = new Light();
        var remote = new RemoteControl();

        remote.SetCommand(new TurnOnLightCommand(light));
        remote.PressButton();

        Assert.True(light.IsOn);
    }

    [Fact]
    public void RemoteControl_WithTurnOffCommand_TurnsLightOff()
    {
        var light = new Light();
        light.TurnOn();
        var remote = new RemoteControl();

        remote.SetCommand(new TurnOffLightCommand(light));
        remote.PressButton();

        Assert.False(light.IsOn);
    }

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
