# Design Pattern And Sample Code

Repo tổng hợp báo cáo nghiên cứu về các design pattern và các nguyên tắc SOLID, thực hiện theo yêu cầu của mentor. Mỗi pattern/nguyên tắc được trình bày trong một file `.md` riêng, theo cấu trúc thống nhất: khái niệm cơ bản, khi nào nên dùng, và các code example minh họa.

Tài liệu được chia thành 2 thư mục: [Design-Patterns/](Design-Patterns/) cho các design pattern, [SOLID/](SOLID/) cho 5 nguyên tắc SOLID.

## Danh sách báo cáo

### Design Patterns / Creational

| Pattern | File | Mô tả ngắn |
|---|---|---|
| Factory Method | [Design-Patterns/Factory-Method-Pattern.md](Design-Patterns/Factory-Method-Pattern.md) | Để lớp con quyết định khởi tạo lớp cụ thể nào, tách việc tạo đối tượng khỏi logic dùng chung. |
| Builder | [Design-Patterns/Builder-Pattern.md](Design-Patterns/Builder-Pattern.md) | Xây dựng đối tượng phức tạp từng bước, thay vì constructor nhận quá nhiều tham số. |
| Singleton | [Design-Patterns/Singleton-Pattern.md](Design-Patterns/Singleton-Pattern.md) | Đảm bảo một class chỉ có đúng một instance, cung cấp một điểm truy cập toàn cục tới nó. |

### Design Patterns / Structural

| Pattern | File | Mô tả ngắn |
|---|---|---|
| Adapter | [Design-Patterns/Adapter-Pattern.md](Design-Patterns/Adapter-Pattern.md) | Chuyển đổi interface không tương thích giữa code hiện có và code/thư viện cũ. |
| Decorator | [Design-Patterns/Decorator-Pattern.md](Design-Patterns/Decorator-Pattern.md) | Gắn thêm hành vi cho đối tượng tại runtime bằng cách bọc lớp, thay vì kế thừa nhiều tổ hợp subclass. |
| Proxy | [Design-Patterns/Proxy-Pattern.md](Design-Patterns/Proxy-Pattern.md) | Tạo đối tượng đại diện đứng trước đối tượng thật để kiểm soát việc truy cập (trì hoãn khởi tạo, kiểm tra quyền, cache). |

### Design Patterns / Behavioral

| Pattern | File | Mô tả ngắn |
|---|---|---|
| Strategy | [Design-Patterns/Strategy-Pattern.md](Design-Patterns/Strategy-Pattern.md) | Đóng gói mỗi giải thuật thành một class riêng, cho phép đổi giải thuật tại runtime. |
| Observer | [Design-Patterns/Observer-Pattern.md](Design-Patterns/Observer-Pattern.md) | Tự động thông báo cho nhiều đối tượng phụ thuộc khi trạng thái của một đối tượng thay đổi. |
| State | [Design-Patterns/State-Pattern.md](Design-Patterns/State-Pattern.md) | Cho phép object đổi hành vi khi trạng thái nội bộ thay đổi, tách mỗi trạng thái thành một class riêng. |
| Template Method | [Design-Patterns/Template-Method-Pattern.md](Design-Patterns/Template-Method-Pattern.md) | Định nghĩa sẵn bộ khung thuật toán ở lớp cha, chỉ để lớp con tùy biến đúng những bước cần thiết. |

### SOLID

| Nguyên tắc | File | Mô tả ngắn |
|---|---|---|
| Single Responsibility | [SOLID/Single-Responsibility-Principle.md](SOLID/Single-Responsibility-Principle.md) | Một class chỉ nên có đúng một lý do để thay đổi, tách các trách nhiệm không liên quan ra riêng. |
| Open/Closed | [SOLID/Open-Closed-Principle.md](SOLID/Open-Closed-Principle.md) | Mở cho việc mở rộng, đóng đối với việc sửa đổi — thêm tính năng mới không sửa code cũ. |
| Liskov Substitution | [SOLID/Liskov-Substitution-Principle.md](SOLID/Liskov-Substitution-Principle.md) | Class con phải thay thế được class cha ở bất kỳ đâu mà không làm sai lệch tính đúng đắn của chương trình. |
| Interface Segregation | [SOLID/Interface-Segregation-Principle.md](SOLID/Interface-Segregation-Principle.md) | Không buộc Client phụ thuộc vào method mà nó không dùng tới — tách interface lớn thành nhiều interface nhỏ. |
| Dependency Inversion | [SOLID/Dependency-Inversion-Principle.md](SOLID/Dependency-Inversion-Principle.md) | Module cấp cao và module cấp thấp đều nên phụ thuộc vào abstraction, không phụ thuộc trực tiếp vào nhau. |
