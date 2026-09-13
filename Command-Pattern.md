# Command Pattern

## 1. Khái niệm

**Command** là một behavioral design pattern, đóng gói request thành một đối tượng độc lập, chứa đầy đủ thông tin cần thiết để thực hiện hành động đó (tham số + logic xử lý), thay vì gọi thẳng một method với các tham số rời rạc.

Pattern gồm 4 thành phần:

- **Command (interface):** khai báo method `Execute()` (và có thể có `Undo()`).
- **ConcreteCommand:** hiện thực `Command`, giữ tham chiếu tới Receiver cùng các tham số cần thiết; `Execute()` gọi Receiver thực hiện logic.
- **Receiver:** nơi thực sự chứa logic nghiệp vụ.
- **Invoker:** giữ tham chiếu tới Command và gọi `Execute()` khi cần, không biết Command làm gì bên trong hay Receiver là ai.

## 2. Ý nghĩa

- Tách Invoker khỏi Receiver: Invoker chỉ phụ thuộc interface Command, không phụ thuộc trực tiếp lớp xử lý cụ thể → có thể gán Command khác cho cùng một Invoker tại runtime.
- Biến một hành động thành dữ liệu (first-class object): có thể lưu vào danh sách, đưa vào queue, ghi log lại, hoặc truyền qua tham số — mở đường cho các tính năng như Undo/Redo, transaction log, macro command (gộp nhiều lệnh chạy tuần tự).
- Mỗi hành động là một class riêng biệt, độc lập, dễ mở rộng thêm loại lệnh mới mà không phải sửa code Invoker (Open/Closed Principle).

## 3. Code mẫu

### Ví dụ 1 — Command, Receiver, Invoker cơ bản

    public interface ICommand
    {
        void Execute();
        void Undo();
    }

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

    public class CommandInvoker
    {
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
        }
    }

### Ví dụ 2 — Undo/Redo bằng lịch sử Command

    public class CommandInvoker
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

### Ví dụ 3 — Macro Command (gộp nhiều Command chạy tuần tự)

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

        public void Undo()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }
