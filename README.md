# Design Pattern And Sample Code

Repo tổng hợp báo cáo nghiên cứu về các design pattern, thực hiện theo yêu cầu của mentor. Mỗi pattern được trình bày trong một file `.md` riêng, gồm khái niệm, ý nghĩa và ví dụ minh họa.

## Danh sách báo cáo

### Creational

| Pattern | File | Mô tả ngắn |
|---|---|---|
| Factory Method | [Factory-Method-Pattern.md](Factory-Method-Pattern.md) | Để lớp con quyết định khởi tạo lớp cụ thể nào, tách việc tạo đối tượng khỏi logic dùng chung. |
| Builder | [Builder-Pattern.md](Builder-Pattern.md) | Xây dựng đối tượng phức tạp từng bước, thay vì constructor nhận quá nhiều tham số. |

### Structural

| Pattern | File | Mô tả ngắn |
|---|---|---|
| Adapter | [Adapter-Pattern.md](Adapter-Pattern.md) | Chuyển đổi interface không tương thích giữa code hiện có và code/thư viện cũ. |
| Decorator | [Decorator-Pattern.md](Decorator-Pattern.md) | Gắn thêm hành vi cho đối tượng tại runtime bằng cách bọc lớp, thay vì kế thừa nhiều tổ hợp subclass. |

### Behavioral

| Pattern | File | Mô tả ngắn |
|---|---|---|
| Command | [Command-Pattern.md](Command-Pattern.md) | Đóng gói một request thành đối tượng độc lập (Command), tách rời Invoker và Receiver. |
| Strategy | [Strategy-Pattern.md](Strategy-Pattern.md) | Đóng gói mỗi giải thuật thành một class riêng, cho phép đổi giải thuật tại runtime. |
| Observer | [Observer-Pattern.md](Observer-Pattern.md) | Tự động thông báo cho nhiều đối tượng phụ thuộc khi trạng thái của một đối tượng thay đổi. |
| Mediator | [Mediator.md](Mediator.md) | Giảm phụ thuộc giữa các đối tượng bằng cách giao tiếp qua một trung gian (mediator), ví dụ thư viện MediatR trong .NET. |

### Architectural

| Pattern | File | Mô tả ngắn |
|---|---|---|
| CQRS | [CQRS.md](CQRS.md) | Tách biệt luồng xử lý Command (ghi) và Query (đọc) thành hai model độc lập. |
| Cache-Aside | [Cache-Aside-Pattern.md](Cache-Aside-Pattern.md) | Application tự quản lý đọc/ghi giữa cache và database chính, đọc cache trước khi query DB. |
