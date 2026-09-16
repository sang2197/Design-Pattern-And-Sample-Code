# Design Patterns Practice

Project luyện tập song song với các file báo cáo `.md` ở thư mục gốc. Mỗi pattern có 1 file bài tập (interface/skeleton có sẵn, phần cài đặt để trống bằng `throw new NotImplementedException()`) và 1 file test tương ứng để tự chấm.

## Cấu trúc

```
Practice/
  DesignPatternsPractice.sln
  Exercises/                 <- Viết code vào đây (10 file, đánh số theo thứ tự các pattern)
    01_FactoryMethodExercise.cs
    02_BuilderExercise.cs
    03_AdapterExercise.cs
    04_DecoratorExercise.cs
    05_CommandExercise.cs
    06_StrategyExercise.cs
    07_ObserverExercise.cs
    08_MediatorExercise.cs
    09_SingletonExercise.cs
    10_ProxyExercise.cs
  Exercises.Tests/            <- KHÔNG sửa file trong này, chỉ dùng để tự chấm
```

## Cách luyện tập

1. Mở một file trong `Exercises/`, đọc comment `// TODO:` để biết cần viết gì.
2. Viết code thay cho `throw new NotImplementedException();`.
3. Chạy để kiểm tra:

```
cd Practice
dotnet test
```

4. Test pass (màu xanh) = làm đúng. Test fail sẽ in rõ **Expected** (kết quả đúng) và **Actual** (kết quả code bạn viết ra) để biết sai ở đâu.
5. Nếu bí, xem lại đúng ví dụ tương ứng trong file `.md` ở thư mục gốc (ví dụ `01_FactoryMethodExercise.cs` tương ứng Ví dụ 1 trong `Factory-Method-Pattern.md`) — đó chính là đáp án.

## Chạy test cho riêng 1 pattern

```
dotnet test --filter "FullyQualifiedName~StrategyExerciseTests"
```

(thay `StrategyExerciseTests` bằng tên class test tương ứng, xem trong `Exercises.Tests/`)

## Tương ứng file bài tập ↔ file báo cáo

| File bài tập | File báo cáo | Ví dụ minh họa |
|---|---|---|
| 01_FactoryMethodExercise.cs | Factory-Method-Pattern.md | Ví dụ 1 — Checkout VNPay/Momo |
| 02_BuilderExercise.cs | Builder-Pattern.md | Ví dụ 1 — HttpRequestBuilder |
| 03_AdapterExercise.cs | Adapter-Pattern.md | Ví dụ 1 — SMS Gateway |
| 04_DecoratorExercise.cs | Decorator-Pattern.md | Ví dụ 1 — Coffee |
| 05_CommandExercise.cs | Command-Pattern.md | Ví dụ 1 — Remote control đèn |
| 06_StrategyExercise.cs | Strategy-Pattern.md | Ví dụ 1 — Discount |
| 07_ObserverExercise.cs | Observer-Pattern.md | Ví dụ 1 — Order status |
| 08_MediatorExercise.cs | Mediator.md | Ví dụ 1 — Chat Room |
| 09_SingletonExercise.cs | Singleton-Pattern.md | Ví dụ 1 — AppSettings |
| 10_ProxyExercise.cs | Proxy-Pattern.md | Ví dụ 1 — Virtual Proxy Image |
