# Clean Code

## 1. Clean Code là gì?

**Clean code** là code **dễ đọc, dễ hiểu, dễ sửa** — người khác (hoặc chính mình sau 6 tháng) đọc vào hiểu ngay code làm gì, mà không cần hỏi lại tác giả.

> "Máy tính hiểu code thì ai cũng viết được. Lập trình viên giỏi viết code mà **con người** hiểu được." — Martin Fowler

**Tại sao quan trọng?**
- Thời gian **đọc code** nhiều gấp ~10 lần thời gian **viết code**.
- Code bẩn → sửa 1 chỗ hỏng 3 chỗ → càng lâu càng chậm (gọi là **technical debt** — nợ kỹ thuật).
- Làm việc nhóm: code rõ ràng thì review nhanh, onboard người mới nhanh.

Dấu hiệu code sạch: **tên rõ nghĩa, hàm ngắn, làm 1 việc, không lặp lại, không bất ngờ, có test**.

## 2. Đặt tên (Naming)

Quy tắc:
- Tên phải **nói lên ý định** (làm gì / chứa gì), không cần comment giải thích.
- **Class / biến** → danh từ (`Customer`, `invoiceTotal`). **Method** → động từ (`CalculateTotal`, `SendEmail`).
- **Boolean** → dạng câu hỏi: `isActive`, `hasPermission`, `canEdit`.
- Tránh viết tắt khó hiểu, tránh tên chung chung: `data`, `temp`, `x`, `obj`, `Manager`, `Helper`.
- Nhất quán: đã dùng `Get` thì đừng lẫn `Fetch`, `Retrieve` cho cùng một ý.
- Theo convention của ngôn ngữ (C#: `PascalCase` cho class/method/property, `camelCase` cho biến cục bộ/tham số, `_camelCase` cho field private, interface bắt đầu bằng `I`).

```csharp
// Xấu
int d;                         // d là gì?
List<int> list1;
bool flag;
void Process(int x) { }

// Tốt
int elapsedDays;
List<int> overdueInvoiceIds;
bool isEmailVerified;
void ApproveLeaveRequest(int requestId) { }
```

## 3. Hàm (Functions)

1. **Ngắn** — lý tưởng vài dòng đến ~20 dòng.
2. **Làm đúng 1 việc** (giống SRP nhưng ở mức hàm).
3. **Ít tham số** — 0–2 là tốt, 3 là nhiều, hơn nữa thì gom vào 1 object.
4. **Không có side effect bất ngờ** — hàm tên `CheckPassword` thì đừng lén khởi tạo session.
5. **Tránh tham số boolean (flag argument)** — `Render(true)` không ai hiểu `true` là gì → tách thành 2 hàm.
6. **Command–Query Separation:** hàm hoặc **làm gì đó** (thay đổi trạng thái) hoặc **trả lời gì đó** (trả về giá trị), không làm cả hai.

```csharp
// Xấu: hàm dài, làm nhiều việc, flag argument
public void Save(Order order, bool sendMail)
{
    if (order.Items.Count == 0) throw new Exception("empty");
    decimal total = 0;
    foreach (var i in order.Items) total += i.Price * i.Qty;
    order.Total = total;
    _db.Orders.Add(order);
    _db.SaveChanges();
    if (sendMail) { /* 20 dòng gửi mail */ }
}

// Tốt: mỗi hàm 1 việc, tên tự giải thích
public void PlaceOrder(Order order)
{
    ValidateOrder(order);
    order.Total = CalculateTotal(order);
    _orderRepository.Add(order);
    _notifier.SendOrderConfirmation(order);
}

private static void ValidateOrder(Order order)
{
    if (order.Items.Count == 0)
        throw new InvalidOperationException("Order must have at least one item.");
}

private static decimal CalculateTotal(Order order) => order.Items.Sum(i => i.Price * i.Qty);
```

## 4. Tránh lồng nhau sâu — Guard Clause (Early Return)

Kiểm tra điều kiện sai và **return sớm**, để phần chính của hàm không bị lồng nhiều tầng `if`.

```csharp
// Xấu: "mũi tên" (arrow code)
public decimal GetDiscount(Customer c)
{
    if (c != null)
    {
        if (c.IsActive)
        {
            if (c.Orders.Count > 10)
                return 0.1m;
        }
    }
    return 0;
}

// Tốt: guard clause
public decimal GetDiscount(Customer c)
{
    if (c == null) return 0;
    if (!c.IsActive) return 0;
    if (c.Orders.Count <= 10) return 0;
    return 0.1m;
}
```

## 5. Magic number / magic string

Số hoặc chuỗi "từ trên trời rơi xuống" → khó hiểu, sửa phải tìm khắp nơi. Đặt thành **hằng số có tên** hoặc **enum**.

```csharp
// Xấu
if (user.Role == 3 && age > 18) price *= 0.85m;

// Tốt
const int AdultAge = 18;
const decimal VipDiscountRate = 0.85m;
if (user.Role == Role.Vip && age > AdultAge) price *= VipDiscountRate;
```

## 6. Comment

- **Code tốt tự giải thích** — comment không cứu được code xấu, hãy sửa code (đặt tên lại, tách hàm).
- Comment nên nói **TẠI SAO** (lý do, quyết định nghiệp vụ), không nói **CÁI GÌ** (code đã nói rồi).
- Xóa code bị comment out — đã có git lưu lịch sử.
- Comment sai/lỗi thời còn **tệ hơn** không có comment.

```csharp
// Xấu: comment lặp lại code
i++; // tăng i lên 1

// Xấu: dùng comment thay cho tên tốt
// kiểm tra nhân viên có đủ điều kiện nhận thưởng không
if (e.Years > 5 && e.Rating >= 4) { }

// Tốt: tên tự giải thích
if (employee.IsEligibleForBonus()) { }

// Tốt: comment giải thích TẠI SAO
// Ngân hàng đối tác chỉ chấp nhận tối đa 3 request/giây, nên phải delay.
await Task.Delay(350);
```

## 7. Các nguyên tắc chung: DRY, KISS, YAGNI

| Nguyên tắc | Ý nghĩa | Ví dụ vi phạm |
|---|---|---|
| **DRY** — Don't Repeat Yourself | Một logic chỉ nên viết ở **một chỗ** | Công thức tính VAT copy-paste ở 5 file, đổi thuế phải sửa 5 nơi |
| **KISS** — Keep It Simple, Stupid | Giải pháp **đơn giản nhất** chạy được là tốt nhất | Dùng 3 design pattern chỉ để cộng 2 số |
| **YAGNI** — You Aren't Gonna Need It | **Đừng code trước** tính năng chưa ai yêu cầu | Làm sẵn hỗ trợ 5 loại DB "phòng khi sau này cần" |

Thêm:
- **SOLID** — 5 nguyên tắc thiết kế class (xem thư mục [SOLID](../SOLID/)).
- **Law of Demeter** — "chỉ nói chuyện với bạn thân": tránh chuỗi `a.GetB().GetC().GetD()`.
- **Composition over Inheritance** — ưu tiên ghép object hơn kế thừa nhiều tầng.
- **Boy Scout Rule** — "rời khỏi khu cắm trại sạch hơn lúc đến": mỗi lần sửa code, dọn dẹp thêm một chút.

```csharp
// Vi phạm Law of Demeter
var city = order.GetCustomer().GetAddress().GetCity();

// Tốt hơn
var city = order.GetShippingCity();
```

## 8. Xử lý lỗi (Error Handling)

- Dùng **exception** thay vì trả về mã lỗi (`-1`, `null`) mà người gọi dễ quên kiểm tra.
- **Không nuốt lỗi** (`catch {}` rỗng) — lỗi bị giấu, rất khó debug.
- Bắt exception **cụ thể**, không bắt `Exception` chung chung nếu không cần.
- Thông báo lỗi rõ ràng, có ngữ cảnh.
- Hạn chế trả về `null` → dùng collection rỗng, hoặc Null Object, hoặc `TryXxx`.

```csharp
// Xấu
try { SaveFile(path); } catch { }                  // nuốt lỗi
public List<Order> GetOrders() { return null; }     // người gọi phải check null

// Tốt
try { SaveFile(path); }
catch (IOException ex)
{
    _logger.LogError(ex, "Cannot save file {Path}", path);
    throw;
}
public List<Order> GetOrders() => new();            // trả về list rỗng
```

## 9. Class & cấu trúc code

- Class **nhỏ**, một trách nhiệm (SRP).
- **Đóng gói**: field để `private`, chỉ public những gì cần.
- **Tính kết dính cao (high cohesion)**: các thứ trong class liên quan chặt với nhau.
- **Phụ thuộc lỏng (low coupling)**: class ít phụ thuộc cứng vào class khác → dùng interface + Dependency Injection.
- Format **nhất quán**: thụt lề, khoảng trắng, thứ tự thành viên. Dùng formatter / `.editorconfig` cho cả team.
- Code liên quan để **gần nhau**; hàm gọi đặt trên hàm được gọi (đọc từ trên xuống như đọc báo).

## 10. Code Smell (dấu hiệu code "có mùi")

| Smell | Mô tả | Cách sửa |
|---|---|---|
| Long Method | Hàm quá dài | Tách hàm (Extract Method) |
| Large Class / God Class | Class biết và làm mọi thứ | Tách class (Extract Class) |
| Duplicated Code | Code lặp lại | Gom thành hàm/class dùng chung |
| Long Parameter List | Quá nhiều tham số | Gom vào object (Parameter Object) |
| Magic Number | Số không rõ nghĩa | Đặt hằng số có tên |
| Dead Code | Code không bao giờ chạy | Xóa đi |
| Deep Nesting | if/for lồng nhau nhiều tầng | Guard clause, tách hàm |
| Switch/if-else dài theo loại | `switch (type)` lặp ở nhiều nơi | Đa hình / Strategy pattern |
| Feature Envy | Hàm dùng dữ liệu class khác nhiều hơn class mình | Chuyển hàm sang class kia |
| Primitive Obsession | Dùng `string`, `int` cho mọi thứ (email, tiền...) | Tạo kiểu riêng (`Email`, `Money`) |

## 11. Refactoring

**Refactoring** = sửa cấu trúc bên trong code cho sạch hơn **mà không đổi hành vi** bên ngoài.

- Làm **từng bước nhỏ**, mỗi bước chạy lại test.
- **Phải có unit test** trước khi refactor, để chắc chắn không làm hỏng gì.
- Kỹ thuật hay dùng: Rename, Extract Method, Extract Class, Inline, Replace Magic Number with Constant, Replace Conditional with Polymorphism.

## 12. Ví dụ tổng hợp: trước và sau

```csharp
// TRƯỚC
public double calc(List<Item> l, int t)
{
    double r = 0;
    for (int i = 0; i < l.Count; i++)
    {
        r = r + l[i].p * l[i].q;
    }
    if (t == 1) r = r * 0.9;
    if (t == 2) r = r * 0.8;
    return r;
}

// SAU
public enum CustomerType { Regular, Silver, Gold }

public decimal CalculateOrderTotal(List<OrderItem> items, CustomerType customerType)
{
    decimal subtotal = items.Sum(item => item.Price * item.Quantity);
    return subtotal * GetDiscountMultiplier(customerType);
}

private static decimal GetDiscountMultiplier(CustomerType type) => type switch
{
    CustomerType.Silver => 0.9m,
    CustomerType.Gold => 0.8m,
    _ => 1.0m
};
```

Đã sửa: tên rõ nghĩa, bỏ magic number (dùng enum), `decimal` cho tiền (không dùng `double`), tách hàm nhỏ, dùng LINQ ngắn gọn.

## 13. Câu hỏi hay gặp

**H: Clean code là gì?**
Đ: Code dễ đọc, dễ hiểu, dễ sửa, dễ test; người khác đọc hiểu ngay ý định.

**H: Nêu các tiêu chí của một hàm tốt?**
Đ: Ngắn, làm 1 việc, tên là động từ rõ nghĩa, ít tham số, không side effect bất ngờ, không dùng flag argument.

**H: DRY, KISS, YAGNI là gì?**
Đ: DRY — không lặp code. KISS — giữ đơn giản. YAGNI — không làm thứ chưa cần.

**H: Khi nào nên viết comment?**
Đ: Khi cần giải thích **tại sao** (lý do nghiệp vụ, cảnh báo, workaround). Không comment lại điều code đã nói rõ.

**H: Refactoring là gì? Cần gì trước khi refactor?**
Đ: Cải thiện cấu trúc code mà không đổi hành vi. Cần có unit test để đảm bảo không làm hỏng chức năng.

## 14. Tóm tắt học thuộc

- Clean code = **dễ đọc, dễ hiểu, dễ sửa**; code được đọc nhiều hơn viết.
- **Tên**: nói lên ý định; class = danh từ, method = động từ, bool = `is/has/can`.
- **Hàm**: ngắn, **1 việc**, ≤ 2–3 tham số, không flag argument, không side effect.
- **Guard clause** để tránh lồng `if` sâu.
- Không **magic number** → hằng số / enum.
- **Comment** nói *tại sao*, không nói *cái gì*.
- **DRY – KISS – YAGNI – SOLID – Law of Demeter – Boy Scout Rule**.
- Lỗi: dùng **exception**, không nuốt lỗi, hạn chế trả `null`.
- **Code smell** → **refactor** từng bước nhỏ, **có test** bảo vệ.
