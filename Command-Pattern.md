# Command Pattern

## 1. Khái niệm cơ bản

**Command** là một Behavioral Design Pattern, dùng để đóng gói một yêu cầu/hành động thành một object riêng, thay vì gọi thẳng một method với các tham số rời rạc.

Pattern gồm 4 thành phần:

- **Command (interface):** khai báo method `Execute()` (và có thể thêm `Undo()`).
- **ConcreteCommand:** hiện thực `Command`, giữ tham chiếu tới Receiver và các dữ liệu cần thiết để thực hiện hành động.
- **Receiver:** object thực sự chứa logic xử lý.
- **Invoker:** nhận một `Command` và gọi `Execute()`, không cần biết bên trong Command sẽ gọi Receiver nào hay xử lý như thế nào.

## 2. Bài toán

Nơi phát lệnh (Invoker — ví dụ nút bấm, menu item) thường cần gọi thẳng tới nơi thực thi (Receiver) thông qua một lời gọi method cụ thể. Cách làm này có vài giới hạn:

- Invoker bị **gắn chặt** với đúng một Receiver và một hành động cố định, không thể đổi hành động cho cùng một Invoker tại runtime mà không sửa code.
- Lời gọi method biến mất ngay sau khi thực thi — không có cách nào **lưu lại, xếp hàng đợi, hay đảo ngược (Undo)** một hành động đã xảy ra, vì bản thân hành động đó không tồn tại như một đối tượng.

## 3. Ý nghĩa của Command

- Tách nơi phát lệnh khỏi nơi thực thi: Invoker chỉ làm việc với Command, không phụ thuộc trực tiếp vào Receiver.
- Cho phép thay đổi hành động linh hoạt: cùng một Invoker có thể nhận các Command khác nhau tại runtime.
- Vì Command là một object riêng nên có thể lưu vào danh sách, queue hoặc history, từ đó hỗ trợ các chức năng như Undo/Redo, retry, scheduling hoặc macro command.
- Hỗ trợ Open/Closed Principle: thêm hành động mới bằng cách tạo thêm ConcreteCommand, không cần sửa Invoker.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: nêu rõ khi nào nên dùng, có đủ Command/ConcreteCommand/Receiver/Invoker, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Điều khiển từ xa bật/tắt đèn

**Khi nào dùng:** `RemoteControl` (Invoker) chỉ có 1 nút bấm, nhưng cần gán được nhiều hành động khác nhau (bật đèn, tắt đèn...) cho nút đó tại runtime, mà không sửa code của `RemoteControl`.

**Cách sử dụng:** gọi `remote.SetCommand(...)` để gán Command mong muốn, sau đó `remote.PressButton()` — Invoker không cần biết Command đang gán là gì.

**Cách hiện thực:** `RemoteControl` chỉ giữ một tham chiếu `ICommand` và gọi `Execute()` qua interface; từng `ConcreteCommand` giữ tham chiếu `Light` (Receiver) và gọi đúng method tương ứng.

```csharp
// Command
public interface ICommand
{
    void Execute();
}

// Receiver
public class Light
{
    public void TurnOn()
    {
        Console.WriteLine("Light ON");
    }

    public void TurnOff()
    {
        Console.WriteLine("Light OFF");
    }
}

// ConcreteCommand
public class TurnOnLightCommand : ICommand
{
    private readonly Light _light;

    public TurnOnLightCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.TurnOn();
    }
}

public class TurnOffLightCommand : ICommand
{
    private readonly Light _light;

    public TurnOffLightCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.TurnOff();
    }
}

// Invoker
public class RemoteControl
{
    private ICommand _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    public void PressButton()
    {
        _command.Execute();
    }
}

// Chạy thử - cách sử dụng: gán Command khác nhau cho cùng 1 Invoker
public class Program
{
    public static void Main()
    {
        var light = new Light();
        var remote = new RemoteControl();

        remote.SetCommand(new TurnOnLightCommand(light));
        remote.PressButton();

        remote.SetCommand(new TurnOffLightCommand(light));
        remote.PressButton();
    }
}
// Light ON
// Light OFF
```

### Ví dụ 2 — Undo khi soạn thảo văn bản

**Khi nào dùng:** cần hỗ trợ Undo cho thao tác soạn thảo — chỉ khả thi khi mỗi thao tác (chèn text...) được đóng gói thành một object có thể tự đảo ngược (`Undo()`), thay vì chỉ là một lệnh gọi method rồi biến mất.

**Cách sử dụng:** mọi thao tác đều đi qua `history.ExecuteCommand(...)`; muốn hoàn tác thao tác gần nhất chỉ cần gọi `history.Undo()`.

**Cách hiện thực:** `CommandHistory` (Invoker) đẩy mỗi Command đã thực thi vào một `Stack`; `Undo()` lấy Command ở đỉnh Stack ra và gọi `Undo()` của chính nó — Invoker không cần biết cách đảo ngược từng loại thao tác.

```csharp
// Command
public interface ICommand
{
    void Execute();
    void Undo();
}

// Receiver
public class TextDocument
{
    public string Content { get; private set; } = "";

    public void InsertText(string text, int position)
    {
        Content = Content.Insert(position, text);
    }

    public void DeleteText(int position, int length)
    {
        Content = Content.Remove(position, length);
    }
}

// ConcreteCommand
public class InsertTextCommand : ICommand
{
    private readonly TextDocument _document;
    private readonly string _text;
    private readonly int _position;

    public InsertTextCommand(TextDocument document, string text, int position)
    {
        _document = document;
        _text = text;
        _position = position;
    }

    public void Execute()
    {
        _document.InsertText(_text, _position);
    }

    public void Undo()
    {
        _document.DeleteText(_position, _text.Length);
    }
}

// Invoker
public class CommandHistory
{
    private readonly Stack<ICommand> _history = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _history.Push(command);
    }

    public void Undo()
    {
        if (_history.Count == 0)
        {
            return;
        }

        var lastCommand = _history.Pop();
        lastCommand.Undo();
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var document = new TextDocument();
        var history = new CommandHistory();

        history.ExecuteCommand(new InsertTextCommand(document, "Hello", 0));
        history.ExecuteCommand(new InsertTextCommand(document, " World", 5));
        Console.WriteLine(document.Content);

        history.Undo();
        Console.WriteLine(document.Content);
    }
}
// Hello World
// Hello
```

### Ví dụ 3 — Macro Command: bật nhiều thiết bị cùng lúc

**Khi nào dùng:** một nút "Chế độ xem phim" cần bật TV lẫn điều hòa cùng lúc — gộp nhiều Command thành 1 Command duy nhất (Macro), Invoker vẫn chỉ gọi `Execute()` như bình thường mà không cần biết bên trong có bao nhiêu hành động.

**Cách sử dụng:** tạo `MacroCommand` từ một danh sách Command con, rồi gán/gọi nó y hệt như một Command đơn lẻ bình thường.

**Cách hiện thực:** `MacroCommand` cũng hiện thực `ICommand`, nhưng `Execute()` của nó chỉ là vòng lặp gọi `Execute()` của từng Command con theo đúng thứ tự trong danh sách.

```csharp
// Command
public interface ICommand
{
    void Execute();
}

// Receiver
public class Television
{
    public void TurnOn() => Console.WriteLine("TV ON");
}

public class AirConditioner
{
    public void TurnOn() => Console.WriteLine("AC ON");
}

// ConcreteCommand
public class TurnOnTvCommand : ICommand
{
    private readonly Television _tv;

    public TurnOnTvCommand(Television tv)
    {
        _tv = tv;
    }

    public void Execute() => _tv.TurnOn();
}

public class TurnOnAcCommand : ICommand
{
    private readonly AirConditioner _ac;

    public TurnOnAcCommand(AirConditioner ac)
    {
        _ac = ac;
    }

    public void Execute() => _ac.TurnOn();
}

// ConcreteCommand - gộp nhiều Command chạy tuần tự
public class MacroCommand : ICommand
{
    private readonly List<ICommand> _commands;

    public MacroCommand(List<ICommand> commands)
    {
        _commands = commands;
    }

    public void Execute()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var tv = new Television();
        var ac = new AirConditioner();

        ICommand watchMovieMode = new MacroCommand(new List<ICommand>
        {
            new TurnOnTvCommand(tv),
            new TurnOnAcCommand(ac)
        });

        watchMovieMode.Execute();
    }
}
// TV ON
// AC ON
```
