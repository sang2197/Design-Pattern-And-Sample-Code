# Single Responsibility Principle (SRP)

## 1. Khái niệm cơ bản

**Single Responsibility Principle** là nguyên tắc đầu tiên trong SOLID: **"Một class chỉ nên có đúng một lý do để thay đổi"** — tức là một class chỉ nên đảm nhiệm đúng **một trách nhiệm (responsibility)** duy nhất. Nếu một class gộp nhiều trách nhiệm không liên quan, thay đổi ở trách nhiệm này có nguy cơ ảnh hưởng tới các trách nhiệm khác đang nằm chung trong class đó.

Cách nhận biết vi phạm: tự hỏi "class này thay đổi vì những lý do gì?" — nếu câu trả lời có từ 2 lý do trở lên (ví dụ: đổi vì quy tắc nghiệp vụ thay đổi, VÀ đổi vì định dạng báo cáo thay đổi, VÀ đổi vì cách lưu trữ thay đổi), class đó đang vi phạm SRP.

## 2. Khi nào nên dùng

Dùng khi:

✅ Một class đang đồng thời xử lý nhiều mối quan tâm không liên quan (nghiệp vụ, lưu trữ, định dạng, gửi thông báo...)

✅ Thay đổi một tính năng lại vô tình ảnh hưởng tới tính năng khác trong cùng class

✅ Muốn từng phần logic có thể test độc lập, không phải mock quá nhiều phụ thuộc không liên quan

✅ Muốn nhiều người có thể sửa các phần khác nhau của hệ thống song song mà ít xung đột

## 3. Code examples

### Ví dụ 1 — Tách tính lương, in phiếu lương và lưu trữ nhân viên

**Bài toán:** Class `Employee` ban đầu gộp cả 3 trách nhiệm: tính lương (logic nghiệp vụ), in phiếu lương (định dạng hiển thị), và lưu xuống database (lưu trữ). Ba lý do thay đổi hoàn toàn khác nhau — quy tắc tính lương đổi, mẫu phiếu lương đổi, hoặc đổi cách lưu trữ từ SQL sang NoSQL — đều buộc phải sửa cùng một class `Employee`, dễ gây xung đột khi nhiều người cùng sửa và khó test riêng từng phần.

**Ý nghĩa của SRP trong ví dụ này:** Tách `Employee` (chỉ giữ dữ liệu và tính lương), `PaySlipPrinter` (chỉ lo định dạng in phiếu lương), và `EmployeeRepository` (chỉ lo lưu trữ) thành 3 class riêng biệt — mỗi class chỉ có đúng một lý do để thay đổi.

**Cách implementation (C#):**

```csharp
public class Employee
{
    public string Name { get; }
    public decimal BaseSalary { get; }

    public Employee(string name, decimal baseSalary)
    {
        Name = name;
        BaseSalary = baseSalary;
    }

    // Chi lo dung 1 viec: tinh luong
    public decimal CalculatePay(int workedDays) => BaseSalary / 26 * workedDays;
}

public class PaySlipPrinter
{
    // Chi lo dung 1 viec: dinh dang phieu luong
    public string Print(Employee employee, decimal pay) => $"Phieu luong - {employee.Name}: {pay:N0} VND";
}

public class EmployeeRepository
{
    // Chi lo dung 1 viec: luu tru
    public void Save(Employee employee) => Console.WriteLine($"[DB] Da luu nhan vien {employee.Name}");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var employee = new Employee("Nam", 15_000_000);
        var payAmount = employee.CalculatePay(22);

        Console.WriteLine(new PaySlipPrinter().Print(employee, payAmount));

        new EmployeeRepository().Save(employee);
    }
}
// Phieu luong - Nam: 12,692,308 VND
// [DB] Da luu nhan vien Nam
```

### Ví dụ 2 — Tách tính tổng hóa đơn, xuất PDF và lưu trữ

**Bài toán:** Class `Invoice` ban đầu vừa tính tổng tiền hóa đơn (cộng dồn từng dòng), vừa tự xuất ra PDF, vừa tự lưu xuống database. Khi cần đổi mẫu PDF hoặc đổi nơi lưu trữ, đều phải sửa vào đúng class `Invoice` — nơi cũng đang chứa logic tính tiền quan trọng, dễ gây lỗi ngoài ý muốn khi sửa những phần không liên quan tới nhau.

**Ý nghĩa của SRP trong ví dụ này:** Tách `Invoice` (chỉ giữ dữ liệu và tính tổng tiền), `InvoicePdfExporter` (chỉ lo xuất PDF), và `InvoiceRepository` (chỉ lo lưu trữ) — sửa cách xuất PDF hay cách lưu trữ không còn động chạm tới logic tính tiền của `Invoice`.

**Cách implementation (C#):**

```csharp
public class Invoice
{
    public List<(string Item, decimal Price)> Lines { get; } = new List<(string, decimal)>();

    public void AddLine(string item, decimal price) => Lines.Add((item, price));

    // Chi lo dung 1 viec: tinh tong tien
    public decimal CalculateTotal() => Lines.Sum(l => l.Price);
}

public class InvoicePdfExporter
{
    // Chi lo dung 1 viec: xuat PDF
    public string Export(Invoice invoice) => $"[PDF] Hoa don gom {invoice.Lines.Count} dong, tong {invoice.CalculateTotal():N0} VND";
}

public class InvoiceRepository
{
    // Chi lo dung 1 viec: luu tru
    public void Save(Invoice invoice) => Console.WriteLine($"[DB] Da luu hoa don, tong {invoice.CalculateTotal():N0} VND");
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var invoice = new Invoice();
        invoice.AddLine("San pham A", 200000);
        invoice.AddLine("San pham B", 150000);

        Console.WriteLine(new InvoicePdfExporter().Export(invoice));
        new InvoiceRepository().Save(invoice);
    }
}
// [PDF] Hoa don gom 2 dong, tong 350,000 VND
// [DB] Da luu hoa don, tong 350,000 VND
```

### Ví dụ 3 — Tách quy trình đăng ký người dùng thành các bước độc lập

**Bài toán:** Một method `Register()` duy nhất ban đầu tự làm hết: kiểm tra định dạng email/mật khẩu, tự băm mật khẩu, tự lưu user xuống database, tự gửi email chào mừng. Bốn lý do thay đổi khác nhau — quy tắc validate đổi, thuật toán băm đổi, cách lưu trữ đổi, hoặc mẫu email đổi — đều dồn vào cùng một chỗ, khiến method này vừa dài vừa khó test riêng từng bước.

**Ý nghĩa của SRP trong ví dụ này:** Tách thành `UserValidator`, `PasswordHasher`, `UserRepository`, `WelcomeEmailSender`, mỗi class chỉ lo đúng một việc; `UserRegistrationService` chỉ còn đóng vai trò điều phối, gọi lần lượt các class trên theo đúng thứ tự mà không tự chứa logic chi tiết của bất kỳ bước nào.

**Cách implementation (C#):**

```csharp
public class UserValidator
{
    // Chi lo dung 1 viec: kiem tra dau vao hop le
    public bool IsValid(string email, string password) => email.Contains("@") && password.Length >= 6;
}

public class PasswordHasher
{
    // Chi lo dung 1 viec: bam mat khau
    public string Hash(string password) => $"hashed:{password}";
}

public class UserRepository
{
    // Chi lo dung 1 viec: luu tru
    public void Save(string email, string hashedPassword) => Console.WriteLine($"[DB] Da luu user {email}");
}

public class WelcomeEmailSender
{
    // Chi lo dung 1 viec: gui email
    public void Send(string email) => Console.WriteLine($"[Email] Da gui email chao mung toi {email}");
}

// Dieu phoi cac buoc - khong tu chua logic chi tiet cua tung buoc
public class UserRegistrationService
{
    private readonly UserValidator _validator = new UserValidator();
    private readonly PasswordHasher _hasher = new PasswordHasher();
    private readonly UserRepository _repository = new UserRepository();
    private readonly WelcomeEmailSender _emailSender = new WelcomeEmailSender();

    public void Register(string email, string password)
    {
        if (!_validator.IsValid(email, password))
        {
            Console.WriteLine("[Register] Du lieu khong hop le");
            return;
        }

        var hashedPassword = _hasher.Hash(password);
        _repository.Save(email, hashedPassword);
        _emailSender.Send(email);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var service = new UserRegistrationService();
        service.Register("user@example.com", "123456");
        service.Register("bad-email", "123");
    }
}
// [DB] Da luu user user@example.com
// [Email] Da gui email chao mung toi user@example.com
// [Register] Du lieu khong hop le
```
