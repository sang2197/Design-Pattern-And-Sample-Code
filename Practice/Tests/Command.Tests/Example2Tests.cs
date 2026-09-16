using Exercises.Command.Example2;
using Xunit;

namespace Command.Tests;

public class Example2Tests
{
    [Fact]
    public void Undo_RevertsLastExecutedCommand()
    {
        var document = new TextDocument();
        var history = new CommandHistory();

        history.ExecuteCommand(new InsertTextCommand(document, "Hello", 0));
        history.ExecuteCommand(new InsertTextCommand(document, " World", 5));
        Assert.Equal("Hello World", document.Content);

        history.Undo();
        Assert.Equal("Hello", document.Content);
    }
}
