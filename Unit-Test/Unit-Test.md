# Unit Test

## 1. Unit Test là gì?

**Unit test** là đoạn code dùng để **kiểm tra một đơn vị nhỏ nhất** của chương trình (thường là 1 method / 1 class) **một cách độc lập** — không gọi DB thật, không gọi API thật, không đọc file thật.

Ví dụ: kiểm tra hàm `Add(2, 3)` có trả về `5` không.

**Lợi ích:**
- Phát hiện bug **sớm**, sửa rẻ hơn nhiều so với khi lên production.
- **Tự tin refactor / sửa code** — chạy test là biết có làm hỏng gì không (chống **regression**).
- Test cũng là **tài liệu sống**: đọc test là biết hàm dùng thế nào, xử lý case nào.
- Ép thiết kế code tốt hơn: code khó test thường là code dính chặt (tight coupling).

## 2. Các cấp độ test — Test Pyramid

```
        /\        E2E / UI Test      — ít, chậm, tốn kém (test cả hệ thống như người dùng)
       /  \
      /----\      Integration Test   — vừa phải (test nhiều phần chạy cùng nhau: code + DB thật)
     /      \
    /--------\    Unit Test          — nhiều nhất, nhanh, rẻ (test từng hàm riêng lẻ)
```

| | Unit Test | Integration Test | E2E Test |
|---|---|---|---|
| Phạm vi | 1 hàm/class | Nhiều module + DB/API | Toàn bộ app |
| Tốc độ | Rất nhanh (ms) | Chậm hơn | Chậm nhất |
| Phụ thuộc ngoài | Giả lập (mock) | Dùng thật | Dùng thật |
| Người viết | Developer | Developer/QA | QA/Tester |

## 3. Nguyên tắc F.I.R.S.T — unit test tốt phải:

- **F**ast — chạy **nhanh**.
- **I**ndependent — các test **độc lập**, không phụ thuộc thứ tự chạy, không dùng chung dữ liệu.
- **R**epeatable — chạy **bao nhiêu lần, ở máy nào cũng cùng kết quả** (không phụ thuộc giờ hệ thống, mạng, DB).
- **S**elf-validating — tự báo **pass/fail**, không cần người nhìn log.
- **T**imely — viết **kịp thời**, cùng lúc (hoặc trước) code chính.

## 4. Cấu trúc một test: AAA (Arrange – Act – Assert)

- **Arrange:** chuẩn bị dữ liệu, object.
- **Act:** gọi hàm cần test.
- **Assert:** kiểm tra kết quả có đúng mong đợi không.

```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Divide(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException();
        return a / b;
    }
}

public class CalculatorTests
{
    [Fact]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calc = new Calculator();

        // Act
        var result = calc.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public void Divide_ByZero_ThrowsException()
    {
        var calc = new Calculator();
        Assert.Throws<DivideByZeroException>(() => calc.Divide(10, 0));
    }
}
```

**Đặt tên test:** `TênHàm_TìnhHuống_KếtQuảMongĐợi` → ví dụ `Withdraw_AmountGreaterThanBalance_ThrowsException`. Đọc tên là biết test gì.

**Mỗi test chỉ kiểm tra 1 hành vi** — test fail thì biết ngay lỗi ở đâu.

## 5. Framework test trong .NET

| | xUnit | NUnit | MSTest |
|---|---|---|---|
| Test 1 case | `[Fact]` | `[Test]` | `[TestMethod]` |
| Test nhiều bộ dữ liệu | `[Theory]` + `[InlineData]` | `[TestCase]` | `[DataTestMethod]` + `[DataRow]` |
| Setup trước mỗi test | Constructor | `[SetUp]` | `[TestInitialize]` |
| Dọn dẹp sau test | `IDisposable.Dispose` | `[TearDown]` | `[TestCleanup]` |

```csharp
// xUnit Theory: 1 test chạy với nhiều bộ dữ liệu
[Theory]
[InlineData(1, 2, 3)]
[InlineData(-1, 1, 0)]
[InlineData(0, 0, 0)]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    Assert.Equal(expected, new Calculator().Add(a, b));
}
```

Các Assert hay dùng: `Equal`, `NotEqual`, `True`, `False`, `Null`, `NotNull`, `Contains`, `Empty`, `Throws<T>`.

## 6. Nên test những case nào?

1. **Happy path** — dữ liệu hợp lệ, chạy bình thường.
2. **Boundary (biên)** — giá trị ở ranh giới: 0, -1, số lớn nhất, chuỗi rỗng, list rỗng. VD: tuổi hợp lệ 18–60 → test 17, 18, 60, 61.
3. **Invalid input / lỗi** — null, sai định dạng → có ném exception đúng không.
4. **Các nhánh if/else** — mỗi nhánh ít nhất 1 test.

**Không cần test:** code của framework/thư viện, getter/setter đơn giản, private method (test gián tiếp qua public method).

## 7. Test Double — Mock, Stub, Fake, Spy, Dummy

Khi class cần test **phụ thuộc** vào thứ khác (DB, email, API...), ta thay thứ đó bằng **đồ giả** (test double) để test chạy nhanh và độc lập.

| Loại | Là gì | Ví dụ |
|---|---|---|
| **Dummy** | Chỉ để truyền vào cho đủ tham số, không dùng tới | `null` hoặc object rỗng |
| **Stub** | Trả về **dữ liệu cố định** đã chuẩn bị sẵn | `GetUser(1)` luôn trả user "An" |
| **Fake** | Bản cài đặt **đơn giản nhưng chạy được thật** | Repository lưu vào `List` thay vì DB, InMemory DB |
| **Mock** | Giả lập + **kiểm tra có được gọi đúng không** | Kiểm tra `SendEmail` đã được gọi đúng 1 lần |
| **Spy** | Ghi lại các lần gọi để kiểm tra sau | Đếm số lần hàm log được gọi |

Dễ nhớ: **Stub → kiểm tra KẾT QUẢ (state)**, **Mock → kiểm tra HÀNH VI / TƯƠNG TÁC (behavior)**.

### Ví dụ với Moq

Muốn mock được thì class phải **phụ thuộc vào interface** và nhận qua **constructor (Dependency Injection)**.

```csharp
public interface IUserRepository { User? GetByEmail(string email); void Add(User user); }
public interface IEmailSender { void Send(string to, string subject); }

public class UserService
{
    private readonly IUserRepository _repo;
    private readonly IEmailSender _email;

    public UserService(IUserRepository repo, IEmailSender email)
    {
        _repo = repo;
        _email = email;
    }

    public bool Register(string email)
    {
        if (_repo.GetByEmail(email) != null) return false;   // email đã tồn tại
        _repo.Add(new User { Email = email });
        _email.Send(email, "Welcome");
        return true;
    }
}

public class UserServiceTests
{
    [Fact]
    public void Register_NewEmail_SavesUserAndSendsWelcomeMail()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmail("a@x.com")).Returns((User?)null);   // stub: chưa có user
        var email = new Mock<IEmailSender>();
        var service = new UserService(repo.Object, email.Object);

        // Act
        var result = service.Register("a@x.com");

        // Assert
        Assert.True(result);
        repo.Verify(r => r.Add(It.IsAny<User>()), Times.Once);          // mock: kiểm tra có gọi
        email.Verify(e => e.Send("a@x.com", "Welcome"), Times.Once);
    }

    [Fact]
    public void Register_ExistingEmail_ReturnsFalseAndDoesNotSendMail()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmail("a@x.com")).Returns(new User());   // đã có user
        var email = new Mock<IEmailSender>();
        var service = new UserService(repo.Object, email.Object);

        var result = service.Register("a@x.com");

        Assert.False(result);
        email.Verify(e => e.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
```

## 8. Viết code dễ test (Testability)

Code **khó test** khi:
- Tự `new` phụ thuộc bên trong (`new SqlConnection()`, `new SmtpClient()`) → không thay được bằng đồ giả.
- Dùng `static`, `DateTime.Now`, `Random` trực tiếp → kết quả thay đổi mỗi lần chạy.
- Hàm quá dài, làm nhiều việc.

Cách sửa: **Dependency Injection + interface** (chính là SOLID — chữ **D**).

```csharp
// Khó test: phụ thuộc giờ hệ thống
public bool IsHappyHour() => DateTime.Now.Hour is >= 17 and < 19;

// Dễ test: tiêm thời gian qua interface
public interface IClock { DateTime Now { get; } }
public class Bar
{
    private readonly IClock _clock;
    public Bar(IClock clock) => _clock = clock;
    public bool IsHappyHour() => _clock.Now.Hour is >= 17 and < 19;
}

// Trong test
var clock = new Mock<IClock>();
clock.Setup(c => c.Now).Returns(new DateTime(2026, 1, 1, 18, 0, 0));
Assert.True(new Bar(clock.Object).IsHappyHour());
```

## 9. TDD — Test Driven Development

Viết **test trước**, code sau. Vòng lặp **Red – Green – Refactor**:

1. 🔴 **Red:** viết 1 test cho tính năng chưa có → chạy → **fail**.
2. 🟢 **Green:** viết code **vừa đủ** để test pass.
3. 🔵 **Refactor:** dọn dẹp code cho sạch, test vẫn phải pass.
4. Lặp lại.

Ưu: code luôn có test, thiết kế gọn, chỉ viết đúng thứ cần. Nhược: ban đầu chậm hơn, cần luyện tập.

## 10. Code Coverage

**Code coverage** = % số dòng / nhánh code được chạy qua khi chạy test.
- Giúp tìm chỗ **chưa có test**.
- **Coverage cao ≠ test tốt**: test có thể chạy qua code mà không assert gì cả.
- Mục tiêu thực tế thường khoảng 70–80%, tập trung vào **logic nghiệp vụ quan trọng**.

## 11. Lỗi thường gặp khi viết test

- Test phụ thuộc nhau / phụ thuộc thứ tự chạy.
- Test gọi DB, mạng thật → chậm, lúc pass lúc fail (**flaky test**).
- Một test kiểm tra quá nhiều thứ.
- Không có Assert, hoặc Assert quá lỏng.
- Có logic (`if`, `for`) trong test → test cũng có thể sai.
- Test chi tiết cài đặt bên trong thay vì hành vi → refactor là test gãy.

## 12. Câu hỏi hay gặp

**H: Unit test là gì? Khác Integration test thế nào?**
Đ: Unit test kiểm tra 1 đơn vị nhỏ (hàm/class) độc lập, phụ thuộc bên ngoài được giả lập. Integration test kiểm tra nhiều thành phần chạy cùng nhau với DB/API thật.

**H: AAA là gì?**
Đ: Arrange (chuẩn bị) – Act (thực thi) – Assert (kiểm tra kết quả).

**H: Mock khác Stub?**
Đ: Stub trả về dữ liệu cố định để test chạy được (kiểm tra kết quả). Mock ngoài ra còn kiểm tra hàm có được gọi đúng cách, đúng số lần hay không (kiểm tra hành vi).

**H: TDD là gì?**
Đ: Viết test trước rồi mới viết code, theo vòng Red – Green – Refactor.

**H: Làm sao để code dễ unit test?**
Đ: Phụ thuộc vào interface, nhận qua constructor (DI), tránh static / `DateTime.Now`, hàm nhỏ làm 1 việc.

**H: Coverage 100% có nghĩa là không có bug?**
Đ: Không. Coverage chỉ cho biết code đã được chạy qua, không đảm bảo kết quả đã được kiểm tra đúng.

## 13. Tóm tắt học thuộc

- Unit test = test **1 đơn vị nhỏ, độc lập**, phụ thuộc bên ngoài thì **giả lập**.
- **Test pyramid:** nhiều Unit → vừa Integration → ít E2E.
- **F.I.R.S.T:** Fast, Independent, Repeatable, Self-validating, Timely.
- Cấu trúc **AAA:** Arrange – Act – Assert. Tên: `Method_Scenario_Expected`.
- Test: **happy path, biên, input sai, từng nhánh**.
- Test double: **Dummy, Stub, Fake, Mock, Spy**. Stub → state, Mock → behavior.
- Code dễ test nhờ **DI + interface**.
- **TDD:** Red → Green → Refactor.
- **Coverage** cao không có nghĩa là test tốt.
