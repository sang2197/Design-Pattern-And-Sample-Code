namespace Exercises.Command.Example1;

// Command - Vi du 1: Dieu khien tu xa bat/tat den
// Xem lai: Command-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ICommand (Command) { void Execute(); }
// - class Light (Receiver) -> bool IsOn { get; private set; }; TurnOn() gan IsOn=true; TurnOff() gan IsOn=false
// - class TurnOnLightCommand : ICommand (ConcreteCommand) -> constructor nhan Light, Execute() goi _light.TurnOn()
// - class TurnOffLightCommand : ICommand (ConcreteCommand) -> constructor nhan Light, Execute() goi _light.TurnOff()
// - class RemoteControl (Invoker)
//     private ICommand? _command;
//     SetCommand(ICommand command) -> gan _command
//     PressButton() -> goi _command?.Execute()
