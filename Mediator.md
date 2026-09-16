# Mediator Pattern

## 1. Khái niệm cơ bản

**Mediator** là một Behavioral Design Pattern, định nghĩa một đối tượng trung gian (**Mediator**) đứng giữa để điều phối cách các đối tượng khác (**Colleague**) giao tiếp với nhau, thay vì để các Colleague tham chiếu và gọi trực tiếp lẫn nhau.

Pattern gồm 4 thành phần:

- **Mediator (interface):** khai báo phương thức để các Colleague giao tiếp thông qua nó.
- **ConcreteMediator:** hiện thực `Mediator`, biết toàn bộ Colleague liên quan và chứa logic điều phối giữa chúng.
- **Colleague (abstract/interface):** giữ tham chiếu tới `Mediator`, chỉ giao tiếp qua Mediator, không tham chiếu trực tiếp tới Colleague khác.
- **ConcreteColleague:** hiện thực cụ thể của Colleague, gửi và nhận thông tin thông qua Mediator.

## 2. Bài toán

Khi nhiều đối tượng cần phối hợp trạng thái với nhau (ví dụ nhiều thành phần UI trên cùng một form, hoặc nhiều người dùng trong một phòng chat), nếu để chúng tham chiếu trực tiếp lẫn nhau thì mỗi đối tượng phải biết và gọi tới từng đối tượng còn lại:

- Số lượng liên kết tăng theo kiểu **N-N** khi số đối tượng tăng lên, tạo thành một lưới phụ thuộc chằng chịt, khó theo dõi và khó sửa.
- Mỗi Colleague **gánh thêm trách nhiệm điều phối** (biết khi nào cần gọi ai, theo thứ tự nào) ngoài trách nhiệm chính của bản thân nó.
- Khó **tái sử dụng riêng lẻ** một Colleague ở ngữ cảnh khác, vì nó đang phụ thuộc cứng vào các Colleague cụ thể xung quanh.

## 3. Ý nghĩa của Mediator

- **Mediator giải quyết bằng cách gom toàn bộ logic điều phối vào một đối tượng trung tâm:** mỗi Colleague chỉ cần biết `Mediator`, không cần biết có bao nhiêu Colleague khác hay chúng là ai. Số lượng liên kết giảm từ N-N (giữa các Colleague với nhau) xuống còn N-1 (mỗi Colleague chỉ liên kết với Mediator).
- Tuân thủ **Single Responsibility Principle**: logic phối hợp giữa nhiều đối tượng được tách khỏi từng Colleague, đặt tập trung trong Mediator; bản thân Colleague chỉ lo phần việc của riêng nó.
- Dễ **tái sử dụng từng Colleague** ở ngữ cảnh khác, vì Colleague không còn phụ thuộc cứng vào các Colleague cụ thể khác — chỉ cần cắm vào một Mediator phù hợp.
- **Lưu ý học thuật:** một số thư viện (ví dụ MediatR trong .NET) dùng tên gọi "Mediator" nhưng thực chất đóng vai trò **request/response dispatcher** — định tuyến một Request tới đúng Handler xử lý nó, gần với ý tưởng CQRS hơn là đúng bản chất Mediator của GoF (điều phối giao tiếp qua lại giữa nhiều đối tượng liên quan). Các ví dụ dưới đây bám sát định nghĩa gốc của pattern, không phụ thuộc bất kỳ framework nào.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: có Mediator, ConcreteMediator, Colleague, ConcreteColleague, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Phòng chat (nhiều User giao tiếp qua ChatRoom)

**Khi nào dùng:** nhiều `ChatUser` cần gửi tin nhắn cho nhau trong cùng một phòng, số lượng người tham gia thay đổi liên tục — không thể để mỗi User giữ tham chiếu tới tất cả User còn lại.

**Cách sử dụng:** mỗi `ChatUser` chỉ gọi `Send()` trên chính nó; việc chuyển tin nhắn tới ai là việc của `ChatRoom`, User hoàn toàn không biết ai đang có mặt trong phòng.

**Cách hiện thực:** `ChatUser` (Colleague) tự đăng ký vào `Mediator` ngay khi khởi tạo; `ChatRoom` (ConcreteMediator) giữ danh sách toàn bộ User và chịu trách nhiệm phát tin nhắn tới từng người, trừ chính người gửi.

```csharp
// Mediator
public interface IChatMediator
{
    void Register(ChatUser user);
    void SendMessage(string message, ChatUser sender);
}

// Colleague
public abstract class ChatUser
{
    protected readonly IChatMediator Mediator;
    public string Name { get; }

    protected ChatUser(IChatMediator mediator, string name)
    {
        Mediator = mediator;
        Name = name;
        mediator.Register(this);
    }

    public void Send(string message)
    {
        Mediator.SendMessage(message, this);
    }

    public abstract void Receive(string message, string senderName);
}

// ConcreteColleague
public class ConsoleChatUser : ChatUser
{
    public ConsoleChatUser(IChatMediator mediator, string name) : base(mediator, name)
    {
    }

    public override void Receive(string message, string senderName)
    {
        Console.WriteLine($"[{Name}] nhan tu {senderName}: {message}");
    }
}

// ConcreteMediator
public class ChatRoom : IChatMediator
{
    private readonly List<ChatUser> _users = new List<ChatUser>();

    public void Register(ChatUser user)
    {
        _users.Add(user);
    }

    public void SendMessage(string message, ChatUser sender)
    {
        // ChatRoom la noi duy nhat biet toan bo User - tung User khong biet lan nhau
        foreach (var user in _users)
        {
            if (user != sender)
            {
                user.Receive(message, sender.Name);
            }
        }
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var chatRoom = new ChatRoom();

        var alice = new ConsoleChatUser(chatRoom, "Alice");
        var bob = new ConsoleChatUser(chatRoom, "Bob");
        var carol = new ConsoleChatUser(chatRoom, "Carol");

        alice.Send("Chao moi nguoi!");
        bob.Send("Chao Alice!");
    }
}
// [Bob] nhan tu Alice: Chao moi nguoi!
// [Carol] nhan tu Alice: Chao moi nguoi!
// [Alice] nhan tu Bob: Chao Alice!
// [Carol] nhan tu Bob: Chao Alice!
```

### Ví dụ 2 — Điều phối các control trên form đăng ký (CheckBox / TextBox / Button)

**Khi nào dùng:** trạng thái `Enabled` của nút Submit phụ thuộc vào trạng thái của cả CheckBox lẫn TextBox — đây là ví dụ UI kinh điển nhất của Mediator (sách GoF).

**Cách sử dụng:** người dùng thao tác trên `CheckBox`/`TextBox` như bình thường (`Toggle()`, `SetText()`); nút Submit tự cập nhật theo mà không có đoạn code nào gọi trực tiếp từ CheckBox/TextBox sang Button.

**Cách hiện thực:** mỗi control chỉ gọi `Mediator.Notify(this, ...)` khi trạng thái của chính nó đổi; `RegisterDialog` (ConcreteMediator) là nơi duy nhất biết điều kiện phối hợp giữa các control và quyết định bật/tắt nút Submit.

```csharp
// Mediator
public interface IDialogMediator
{
    void Notify(object sender, string eventName);
}

// Colleague
public abstract class UiControl
{
    protected readonly IDialogMediator Mediator;

    protected UiControl(IDialogMediator mediator)
    {
        Mediator = mediator;
    }
}

// ConcreteColleague
public class CheckBox : UiControl
{
    public bool Checked { get; private set; }

    public CheckBox(IDialogMediator mediator) : base(mediator)
    {
    }

    public void Toggle()
    {
        Checked = !Checked;
        Mediator.Notify(this, "CheckedChanged");
    }
}

public class TextBox : UiControl
{
    public string Text { get; private set; } = "";

    public TextBox(IDialogMediator mediator) : base(mediator)
    {
    }

    public void SetText(string text)
    {
        Text = text;
        Mediator.Notify(this, "TextChanged");
    }
}

public class SubmitButton : UiControl
{
    public bool Enabled { get; private set; }

    public SubmitButton(IDialogMediator mediator) : base(mediator)
    {
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        Console.WriteLine($"[SubmitButton] Enabled = {Enabled}");
    }
}

// ConcreteMediator
public class RegisterDialog : IDialogMediator
{
    public CheckBox AgreeCheckBox { get; }
    public TextBox EmailTextBox { get; }
    private readonly SubmitButton _submitButton;

    public RegisterDialog()
    {
        AgreeCheckBox = new CheckBox(this);
        EmailTextBox = new TextBox(this);
        _submitButton = new SubmitButton(this);
    }

    public void Notify(object sender, string eventName)
    {
        // CheckBox va TextBox khong biet Button ton tai - chi Mediator biet dieu kien phoi hop
        bool canSubmit = AgreeCheckBox.Checked && !string.IsNullOrWhiteSpace(EmailTextBox.Text);
        _submitButton.SetEnabled(canSubmit);
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var dialog = new RegisterDialog();

        dialog.EmailTextBox.SetText("a@example.com"); // chua tick dong y
        dialog.AgreeCheckBox.Toggle();                 // da du dieu kien
    }
}
// [SubmitButton] Enabled = False
// [SubmitButton] Enabled = True
```

### Ví dụ 3 — Đài kiểm soát không lưu điều phối máy bay hạ cánh

**Khi nào dùng:** nhiều máy bay cùng muốn hạ cánh, nhưng chỉ một chiếc được dùng đường băng tại một thời điểm — các máy bay không thể (và không nên) tự thương lượng trực tiếp với nhau.

**Cách sử dụng:** mỗi `Airplane` chỉ gọi `RequestLanding()`/`Land()` trên chính nó, hoàn toàn không biết còn máy bay nào khác đang tồn tại.

**Cách hiện thực:** `ControlTower` (ConcreteMediator) giữ trạng thái "đường băng đang bận hay trống" và quyết định cho phép/từ chối từng yêu cầu `RequestLanding()` — máy bay chỉ nhận kết quả `true`/`false`, không biết lý do chi tiết bên trong.

```csharp
// Mediator
public interface IControlTower
{
    bool RequestLanding(Airplane airplane);
    void NotifyLanded(Airplane airplane);
}

// Colleague
public abstract class Airplane
{
    protected readonly IControlTower Tower;
    public string FlightCode { get; }

    protected Airplane(IControlTower tower, string flightCode)
    {
        Tower = tower;
        FlightCode = flightCode;
    }

    public void RequestLanding()
    {
        bool allowed = Tower.RequestLanding(this);
        Console.WriteLine(allowed
            ? $"[{FlightCode}] Duoc phep ha canh"
            : $"[{FlightCode}] Phai cho, duong bang dang ban");
    }

    public void Land()
    {
        Console.WriteLine($"[{FlightCode}] Da ha canh");
        Tower.NotifyLanded(this);
    }
}

// ConcreteColleague
public class PassengerPlane : Airplane
{
    public PassengerPlane(IControlTower tower, string flightCode) : base(tower, flightCode)
    {
    }
}

// ConcreteMediator
public class ControlTower : IControlTower
{
    private Airplane _runwayOccupiedBy;

    public bool RequestLanding(Airplane airplane)
    {
        // Cac may bay khong biet lan nhau - chi Tower biet duong bang co dang ban hay khong
        if (_runwayOccupiedBy != null)
        {
            return false;
        }

        _runwayOccupiedBy = airplane;
        return true;
    }

    public void NotifyLanded(Airplane airplane)
    {
        if (_runwayOccupiedBy == airplane)
        {
            _runwayOccupiedBy = null;
        }
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        var tower = new ControlTower();

        var vn123 = new PassengerPlane(tower, "VN123");
        var vn456 = new PassengerPlane(tower, "VN456");

        vn123.RequestLanding();
        vn456.RequestLanding();

        vn123.Land();
        vn456.RequestLanding();
    }
}
// [VN123] Duoc phep ha canh
// [VN456] Phai cho, duong bang dang ban
// [VN123] Da ha canh
// [VN456] Duoc phep ha canh
```
