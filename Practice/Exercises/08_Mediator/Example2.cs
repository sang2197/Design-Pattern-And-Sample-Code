namespace Exercises.Mediator.Example2;

// Mediator - Vi du 2: Dieu phoi cac control tren form dang ky (CheckBox / TextBox / Button)
// Xem lai: Mediator.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IDialogMediator (Mediator) { void Notify(object sender, string eventName); }
// - abstract class UiControl (Colleague)
//     protected readonly IDialogMediator Mediator; constructor nhan IDialogMediator mediator
// - class CheckBox : UiControl (ConcreteColleague)
//     bool Checked { get; private set; }
//     Toggle() -> dao nguoc Checked roi goi Mediator.Notify(this, "CheckedChanged")
// - class TextBox : UiControl (ConcreteColleague)
//     string Text { get; private set; } = ""
//     SetText(text) -> gan Text roi goi Mediator.Notify(this, "TextChanged")
// - class SubmitButton : UiControl (ConcreteColleague)
//     bool Enabled { get; private set; }
//     SetEnabled(bool enabled) -> gan Enabled
// - class RegisterDialog : IDialogMediator (ConcreteMediator)
//     CheckBox AgreeCheckBox { get; }; TextBox EmailTextBox { get; }; SubmitButton SubmitButton { get; }
//     constructor -> khoi tao ca 3 control, truyen "this" lam mediator cho tung control
//     Notify(sender, eventName) -> tinh canSubmit = AgreeCheckBox.Checked && EmailTextBox.Text khong rong/trang,
//       roi goi SubmitButton.SetEnabled(canSubmit)
