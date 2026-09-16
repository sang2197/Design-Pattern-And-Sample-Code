# Design Patterns Practice

Project luyện tập song song với các file báo cáo `.md` ở thư mục gốc. Với mỗi pattern, bạn **viết lại từ đầu cả 3 ví dụ** (không xem code mẫu), rồi chạy test để tự chấm.

## Cấu trúc

```
Practice/
  DesignPatternsPractice.sln
  Exercises/                     <- Viet code vao day
    01_FactoryMethod/Example1.cs, Example2.cs, Example3.cs
    02_Builder/Example1.cs, Example2.cs, Example3.cs
    03_Adapter/...
    04_Decorator/...
    05_Command/...
    06_Strategy/...
    07_Observer/...
    08_Mediator/...
    09_Singleton/...
    10_Proxy/...
  Tests/                          <- KHONG sua file trong nay, chi dung de tu cham
    FactoryMethod.Tests/
    Builder.Tests/
    Adapter.Tests/
    Decorator.Tests/
    Command.Tests/
    Strategy.Tests/
    Observer.Tests/
    Mediator.Tests/
    Singleton.Tests/
    Proxy.Tests/
```

Mỗi file trong `Exercises/` hiện chỉ có **comment mô tả "đề bài"** (tên interface/class/method cần viết và logic bên trong) — không có code. Bạn phải tự viết lại toàn bộ, dùng đúng tên và chữ ký như comment ghi (bắt buộc đúng tên thì test mới biên dịch được).

## Cách luyện tập

1. Mở một trong 3 file của một pattern, ví dụ `Exercises/01_FactoryMethod/Example1.cs`.
2. Đọc comment, **không mở lại file `.md`**, tự viết lại toàn bộ từ trí nhớ.
3. Sau khi làm xong cả 3 ví dụ của pattern đó, chạy:

```
cd Practice
dotnet test Tests/FactoryMethod.Tests
```

(đổi `FactoryMethod.Tests` thành đúng project của pattern đang luyện, xem tên trong `Tests/`)

4. Test pass (xanh) = làm đúng. Test fail sẽ in **Expected** (kết quả đúng) và **Actual** (kết quả code bạn viết) để biết sai ở đâu.
5. Nếu bí, mở lại đúng file `.md` tương ứng (bảng đối chiếu bên dưới) — đó chính là đáp án.

**Quan trọng:** mỗi pattern có 1 project test riêng (`Tests/<Pattern>.Tests`), luôn chạy `dotnet test` nhắm đúng project đó khi đang luyện một pattern cụ thể. Nếu chỉ mới làm xong Factory Method và các pattern khác vẫn còn để trống comment, chạy `dotnet test` (không chỉ định project, tức chạy toàn bộ solution) sẽ báo lỗi biên dịch ở những pattern chưa làm — đó là bình thường, không phải lỗi của bạn ở phần đã làm.

## Tương ứng file bài tập ↔ file báo cáo

| Pattern | File báo cáo | File bài tập |
|---|---|---|
| Factory Method | Factory-Method-Pattern.md | Exercises/01_FactoryMethod/Example1-3.cs |
| Builder | Builder-Pattern.md | Exercises/02_Builder/Example1-3.cs |
| Adapter | Adapter-Pattern.md | Exercises/03_Adapter/Example1-3.cs |
| Decorator | Decorator-Pattern.md | Exercises/04_Decorator/Example1-3.cs |
| Command | Command-Pattern.md | Exercises/05_Command/Example1-3.cs |
| Strategy | Strategy-Pattern.md | Exercises/06_Strategy/Example1-3.cs |
| Observer | Observer-Pattern.md | Exercises/07_Observer/Example1-3.cs |
| Mediator | Mediator.md | Exercises/08_Mediator/Example1-3.cs |
| Singleton | Singleton-Pattern.md | Exercises/09_Singleton/Example1-3.cs |
| Proxy | Proxy-Pattern.md | Exercises/10_Proxy/Example1-3.cs |

## Lưu ý về sai khác nhỏ so với file `.md`

Một vài chỗ trong bài tập thay `Console.WriteLine(...)` bằng việc lưu vào `List<string>` hoặc trả về `bool` thay vì `void` — để test có thể kiểm tra được kết quả (xUnit không đọc được output console). Logic pattern và luồng gọi hàm hoàn toàn giống bản gốc trong `.md`, chỉ khác cách "quan sát" kết quả. Comment trong từng file bài tập đã ghi rõ chỗ nào có thay đổi này.
