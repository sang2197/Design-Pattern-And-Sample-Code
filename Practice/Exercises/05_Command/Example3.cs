namespace Exercises.Command.Example3;

// Command - Vi du 3: Macro Command - bat nhieu thiet bi cung luc
// Xem lai: Command-Pattern.md - Vi du 3
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ICommand (Command) { void Execute(); }
// - class Television (Receiver) -> bool IsOn { get; private set; }; TurnOn() gan IsOn=true
// - class AirConditioner (Receiver) -> bool IsOn { get; private set; }; TurnOn() gan IsOn=true
// - class TurnOnTvCommand : ICommand (ConcreteCommand) -> constructor nhan Television, Execute() goi _tv.TurnOn()
// - class TurnOnAcCommand : ICommand (ConcreteCommand) -> constructor nhan AirConditioner, Execute() goi _ac.TurnOn()
// - class MacroCommand : ICommand (ConcreteCommand - gop nhieu Command)
//     constructor nhan List<ICommand> commands
//     Execute() -> lap qua tung command trong danh sach va goi Execute() cua no
