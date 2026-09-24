# SOLID — Tổng hợp siêu ngắn gọn

| Chữ | Nguyên lý | Một câu |
|---|---|---|
| **S** | Single Responsibility | Một class chỉ có **một lý do để thay đổi** |
| **O** | Open/Closed | **Mở để mở rộng, đóng để sửa đổi** — thêm tính năng bằng code mới, không sửa code cũ |
| **L** | Liskov Substitution | Class con **thay thế được** class cha mà không làm sai hành vi |
| **I** | Interface Segregation | Nhiều interface nhỏ, đúng nhu cầu — không ép class implement method thừa |
| **D** | Dependency Inversion | Logic nghiệp vụ phụ thuộc **abstraction**, không phụ thuộc implementation cụ thể |

---

## S — Single Responsibility

**1. Hóa đơn:** tách tính tiền và in ấn.

```csharp
public class InvoiceCalculator { public decimal Total(decimal price, int qty) => price * qty; }
public class InvoicePrinter    { public void Print(decimal total) => Console.WriteLine($"Tong: {total:N0}"); }
```

**2. Đăng ký user:** tách lưu user và gửi mail chào mừng.

```csharp
public class UserService  { public void Register(string name) => Console.WriteLine($"Da luu {name}"); }
public class WelcomeMailer { public void Send(string name) => Console.WriteLine($"Mail chao mung {name}"); }
```

**3. Báo cáo:** tách tạo nội dung và ghi file.

```csharp
public class ReportBuilder { public string Build(string month) => $"Bao cao {month}"; }
public class ReportExporter { public void Save(string content) => Console.WriteLine($"Da ghi: {content}"); }
```

---

## O — Open/Closed

**1. Giảm giá:** thêm loại giảm giá mới = thêm class, không sửa `if/else`.

```csharp
public interface IDiscount { decimal Apply(decimal price); }
public class VipDiscount : IDiscount { public decimal Apply(decimal p) => p * 0.8m; }
public class NoDiscount  : IDiscount { public decimal Apply(decimal p) => p; }
```

**2. Diện tích hình:** thêm hình mới không đụng vào chỗ tính tổng.

```csharp
public interface IShape { double Area(); }
public class Circle : IShape { public double R; public double Area() => Math.PI * R * R; }
public class Square : IShape { public double S; public double Area() => S * S; }
// Tong = shapes.Sum(s => s.Area());
```

**3. Gửi thông báo:** thêm kênh mới = thêm class.

```csharp
public interface INotifier { void Send(string msg); }
public class EmailNotifier : INotifier { public void Send(string msg) => Console.WriteLine($"Email: {msg}"); }
public class SmsNotifier   : INotifier { public void Send(string msg) => Console.WriteLine($"SMS: {msg}"); }
```

---

## L — Liskov Substitution

**1. Hình chữ nhật/vuông:** `Square : Rectangle` phá hành vi set Width/Height → cho cả hai cùng implement `IShape`.

```csharp
public interface IShape { double Area(); }
public class Rectangle : IShape { public double W, H; public double Area() => W * H; }
public class Square    : IShape { public double S;    public double Area() => S * S; }
```

**2. Chim:** `Penguin : Bird` mà `Fly()` ném exception → chỉ chim biết bay mới implement `IFlyable`.

```csharp
public class Bird { public void Eat() { } }
public interface IFlyable { void Fly(); }
public class Sparrow : Bird, IFlyable { public void Fly() => Console.WriteLine("Bay"); }
public class Penguin : Bird { }
```

**3. Hoàn tiền:** không ép mọi phương thức thanh toán có `Refund()` (COD không hoàn được).

```csharp
public interface IPayment { void Pay(decimal amount); }
public interface IRefundable { void Refund(decimal amount); }
public class CardPayment : IPayment, IRefundable { public void Pay(decimal a) { } public void Refund(decimal a) { } }
public class CodPayment  : IPayment { public void Pay(decimal a) { } }
```

---

## I — Interface Segregation

**1. Máy văn phòng:** máy in đơn giản không phải implement `Scan()`.

```csharp
public interface IPrinter { void Print(string doc); }
public interface IScanner { void Scan(); }
public class BasicPrinter : IPrinter { public void Print(string doc) { } }
public class AllInOne : IPrinter, IScanner { public void Print(string doc) { } public void Scan() { } }
```

**2. Nhân viên/robot:** robot không cần `Eat()`.

```csharp
public interface IWorkable { void Work(); }
public interface IEatable  { void Eat(); }
public class Human : IWorkable, IEatable { public void Work() { } public void Eat() { } }
public class Robot : IWorkable { public void Work() { } }
```

**3. Repository:** tách đọc và ghi, service chỉ đọc thì chỉ nhận `IReadRepository`.

```csharp
public interface IReadRepository<T>  { T GetById(int id); }
public interface IWriteRepository<T> { void Save(T item); }
public class ReportViewer { public ReportViewer(IReadRepository<string> repo) { } }
```

---

## D — Dependency Inversion

**1. Đặt hàng:** `OrderService` phụ thuộc `IOrderRepository`, không `new SqlOrderRepository()`.

```csharp
public interface IOrderRepository { void Save(string id); }
public class SqlOrderRepository : IOrderRepository { public void Save(string id) => Console.WriteLine($"[SQL] {id}"); }
public class OrderService
{
    private readonly IOrderRepository _repo;
    public OrderService(IOrderRepository repo) => _repo = repo;
    public void Place(string id) => _repo.Save(id);
}
// new OrderService(new SqlOrderRepository()).Place("DH01");
```

**2. Đặt lại mật khẩu:** inject kênh gửi, đổi Email ↔ SMS không sửa service.

```csharp
public interface IChannel { void Send(string to, string msg); }
public class EmailChannel : IChannel { public void Send(string to, string msg) => Console.WriteLine($"[Email->{to}] {msg}"); }
public class PasswordResetService
{
    private readonly IChannel _channel;
    public PasswordResetService(IChannel channel) => _channel = channel;
    public void SendCode(string to, string code) => _channel.Send(to, $"Ma: {code}");
}
```

**3. Ghi file báo cáo:** inject `IFileWriter`, khi test dùng bản in-memory.

```csharp
public interface IFileWriter { void Write(string path, string content); }
public class InMemoryFileWriter : IFileWriter
{
    public Dictionary<string, string> Files = new();
    public void Write(string path, string content) => Files[path] = content;
}
public class ReportService
{
    private readonly IFileWriter _writer;
    public ReportService(IFileWriter writer) => _writer = writer;
    public void Generate(string month) => _writer.Write($"report-{month}.txt", $"Bao cao {month}");
}
```
