# Design Pattern And Sample Code

Repo tổng hợp báo cáo nghiên cứu về các design pattern, thực hiện theo yêu cầu của mentor. Mỗi pattern được trình bày trong một file `.md` riêng, gồm khái niệm, ý nghĩa và ví dụ minh họa.

## Danh sách báo cáo

| Pattern | File | Mô tả ngắn |
|---|---|---|
| CQRS | [CQRS.md](CQRS.md) | Tách biệt luồng xử lý Command (ghi) và Query (đọc) thành hai model độc lập. |
| Mediator | [Mediator.md](Mediator.md) | Giảm phụ thuộc giữa các đối tượng bằng cách giao tiếp qua một trung gian (mediator), ví dụ thư viện MediatR trong .NET. |
| Cache-Aside | [Cache-Aside-Pattern.md](Cache-Aside-Pattern.md) | Application tự quản lý đọc/ghi giữa cache và database chính, đọc cache trước khi query DB. |
| Command | [Command-Pattern.md](Command-Pattern.md) | Đóng gói một request thành đối tượng độc lập (Command), tách rời Invoker và Receiver. |
