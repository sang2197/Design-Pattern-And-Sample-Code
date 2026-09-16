using Exercises.Mediator.Example2;
using Xunit;

namespace Mediator.Tests;

public class Example2Tests
{
    [Fact]
    public void SubmitButton_EnablesOnlyWhenBothConditionsMet()
    {
        var dialog = new RegisterDialog();

        dialog.EmailTextBox.SetText("a@example.com");
        Assert.False(dialog.SubmitButton.Enabled);

        dialog.AgreeCheckBox.Toggle();
        Assert.True(dialog.SubmitButton.Enabled);
    }
}
