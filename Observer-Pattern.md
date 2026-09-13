# Observer Pattern

## 1. Khái niệm

**Observer** là một behavioral design pattern, định nghĩa cơ chế **một-nhiều (one-to-many)** giữa các đối tượng: khi trạng thái của một đối tượng (Subject) thay đổi, tất cả đối tượng phụ thuộc vào nó (Observer) đã đăng ký sẽ **tự động được thông báo và cập nhật**, mà Subject không cần biết chi tiết từng Observer là ai, xử lý như thế nào.

Pattern gồm 4 thành phần:

- **Subject (Observable):** giữ danh sách các Observer đã đăng ký, cung cấp method `Subscribe()`/`Unsubscribe()`, và gọi `Notify()` tới tất cả Observer khi trạng thái thay đổi.
- **Observer (interface):** khai báo method (ví dụ `Update()`) mà Subject sẽ gọi khi có sự kiện.
- **ConcreteSubject:** hiện thực cụ thể, chứa trạng thái nghiệp vụ và phát sự kiện khi trạng thái đổi.
- **ConcreteObserver:** hiện thực cụ thể `Observer`, chứa logic phản ứng khi nhận được thông báo.

## 2. Ý nghĩa

- **Vấn đề gặp phải nếu không có Observer:** khi một sự kiện xảy ra (ví dụ đơn hàng đổi trạng thái) cần nhiều nơi khác phản ứng theo (gửi email, gửi SMS, cập nhật tồn kho, ghi log...), cách làm trực tiếp là gọi tuần tự từng service ngay trong method xử lý sự kiện đó. Subject khi ấy phải **biết và phụ thuộc trực tiếp** vào từng lớp xử lý cụ thể; mỗi lần thêm một hành động phản ứng mới lại phải sửa vào đúng method đó, vi phạm Open/Closed Principle và khiến class ngày càng phình to, khó test riêng từng phần.
- **Observer giải quyết bằng cách đảo ngược chiều phụ thuộc:** Subject chỉ biết interface `Observer` chung, không biết có bao nhiêu Observer hay chúng làm gì; các Observer tự đăng ký lắng nghe vào Subject. Khi trạng thái đổi, Subject chỉ cần gọi `Notify()`, tất cả Observer đã đăng ký tự động được gọi.
- Tuân thủ **Open/Closed Principle**: thêm một hành động phản ứng mới khi sự kiện xảy ra chỉ cần thêm 1 ConcreteObserver và đăng ký nó, không sửa code Subject.
- Cho phép **thêm/bớt Observer linh hoạt tại runtime** (subscribe/unsubscribe), tách rời hoàn toàn logic phát sinh sự kiện (Subject) khỏi logic xử lý sự kiện (từng Observer) — đây cũng là nền tảng cho cơ chế event/delegate trong .NET.

## 3. Code mẫu

### Ví dụ 1 — Observer interface và Subject

```csharp
public interface IOrderObserver
{
    void Update(string orderId, string newStatus);
}

public class OrderSubject
{
    private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

    public void Subscribe(IOrderObserver observer)
    {
        _observers.Add(observer);
    }

    public void Unsubscribe(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    public void ChangeStatus(string orderId, string newStatus)
    {
        // Subject không biết và không quan tâm từng Observer xử lý ra sao
        foreach (var observer in _observers)
        {
            observer.Update(orderId, newStatus);
        }
    }
}
```

### Ví dụ 2 — Các ConcreteObserver

```csharp
public class EmailNotifier : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        Console.WriteLine($"[Email] Don hang {orderId} chuyen sang trang thai: {newStatus}");
    }
}

public class InventoryUpdater : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        if (newStatus == "Cancelled")
        {
            Console.WriteLine($"[Inventory] Hoan tra ton kho cho don hang {orderId}");
        }
    }
}
```

### Ví dụ 3 — Đăng ký và kích hoạt sự kiện tại nơi gọi

```csharp
var orderSubject = new OrderSubject();

orderSubject.Subscribe(new EmailNotifier());
orderSubject.Subscribe(new InventoryUpdater());

// Thêm một Observer mới (ví dụ SmsNotifier) không cần sửa OrderSubject
orderSubject.Subscribe(new SmsNotifier());

orderSubject.ChangeStatus("DH0001", "Cancelled");

public class SmsNotifier : IOrderObserver
{
    public void Update(string orderId, string newStatus)
    {
        Console.WriteLine($"[SMS] Don hang {orderId}: {newStatus}");
    }
}
```
