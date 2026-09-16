namespace Exercises.Command;

public interface ICommand
{
    void Execute();
}

public class Light
{
    public bool IsOn { get; private set; }

    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;
}

public class TurnOnLightCommand : ICommand
{
    private readonly Light _light;

    public TurnOnLightCommand(Light light)
    {
        _light = light;
    }

    // TODO: goi _light.TurnOn()
    public void Execute()
    {
        throw new NotImplementedException();
    }
}

public class TurnOffLightCommand : ICommand
{
    private readonly Light _light;

    public TurnOffLightCommand(Light light)
    {
        _light = light;
    }

    // TODO: goi _light.TurnOff()
    public void Execute()
    {
        throw new NotImplementedException();
    }
}

public class RemoteControl
{
    private ICommand? _command;

    // TODO: luu command vao _command
    public void SetCommand(ICommand command)
    {
        throw new NotImplementedException();
    }

    // TODO: goi Execute() tren _command dang giu (neu co)
    public void PressButton()
    {
        throw new NotImplementedException();
    }
}
