# Builder Pattern

## 1. Khái niệm cơ bản

**Builder** là một Creational Design Pattern, dùng để xây dựng một đối tượng phức tạp từng bước, thay vì truyền quá nhiều tham số vào một constructor.

Pattern gồm 4 thành phần:

- **Product:** đối tượng phức tạp cần được xây dựng.
- **Builder:** định nghĩa các bước để cấu hình Product.
- **ConcreteBuilder:** thực hiện các bước xây dựng và trả về Product hoàn chỉnh qua `Build()`.
- **Director (tùy chọn):** định nghĩa sẵn thứ tự/cách gọi Builder để tạo một số cấu hình thường dùng. Với Fluent Builder, Client thường gọi Builder trực tiếp nên không nhất thiết cần Director.

## 2. Khi nào nên dùng

Dùng khi:

✅ Object cần khởi tạo có nhiều thuộc tính, phần lớn là optional

✅ Muốn tránh constructor nhận quá nhiều tham số (telescoping constructor)

✅ Cần đảm bảo object luôn ở trạng thái hợp lệ, không "dở dang" giữa chừng

✅ Muốn tạo ra nhiều đại diện (representation) khác nhau từ cùng một quy trình dựng

## 3. Code examples

### Ví dụ 1 — Fluent Builder dựng HttpRequest

**Bài toán:** Một `HttpRequest` có nhiều phần cấu hình khác nhau — `Url`, `Method`, `Headers`, `Body` — trong đó phần lớn là optional và số lượng header có thể thay đổi tùy request. Nếu dùng một constructor duy nhất nhận hết các tham số này (kể cả collection header), constructor sẽ rất dài, khó đọc, và không linh hoạt khi có request chỉ cần một vài trường. Nếu thay bằng các setter riêng lẻ để gán từng phần sau khi khởi tạo, object có thể bị sử dụng khi còn thiếu `Url` — trường bắt buộc — mà không có bước nào kiểm tra tính hợp lệ trước khi dùng.

**Ý nghĩa của Builder trong ví dụ này:** `HttpRequestBuilder` cung cấp các method `WithUrl()`, `WithMethod()`, `AddHeader()`, `WithBody()`, mỗi method chỉ gán một phần của `HttpRequest` rồi trả về chính `this` để cho phép gọi chuỗi (method chaining) theo thứ tự tùy ý. Việc validate được dồn vào một điểm duy nhất là `Build()`: chỉ khi `Url` đã được set, `HttpRequest` hoàn chỉnh mới được trả về, nhờ đó Client không bao giờ nhận về một `HttpRequest` ở trạng thái dở dang.

**Cách implementation (C#):**

```csharp
// Product
public class HttpRequest
{
    public string Url { get; set; }
    public string Method { get; set; } = "GET";
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    public string Body { get; set; }
}

// Builder (ConcreteBuilder)
public class HttpRequestBuilder
{
    private readonly HttpRequest _request = new HttpRequest();

    public HttpRequestBuilder WithUrl(string url)
    {
        _request.Url = url;
        return this;
    }

    public HttpRequestBuilder WithMethod(string method)
    {
        _request.Method = method;
        return this;
    }

    public HttpRequestBuilder AddHeader(string key, string value)
    {
        _request.Headers[key] = value;
        return this;
    }

    public HttpRequestBuilder WithBody(string body)
    {
        _request.Body = body;
        return this;
    }

    public HttpRequest Build()
    {
        if (string.IsNullOrEmpty(_request.Url))
        {
            throw new InvalidOperationException("Url is required");
        }

        return _request;
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        HttpRequest request = new HttpRequestBuilder()
            .WithUrl("https://api.example.com/products")
            .WithMethod("POST")
            .AddHeader("Authorization", "Bearer token123")
            .AddHeader("Content-Type", "application/json")
            .WithBody("{\"name\":\"Product A\"}")
            .Build();

        Console.WriteLine($"{request.Method} {request.Url}");
        foreach (var header in request.Headers)
        {
            Console.WriteLine($"  {header.Key}: {header.Value}");
        }
        Console.WriteLine($"Body: {request.Body}");
    }
}
// POST https://api.example.com/products
//   Authorization: Bearer token123
//   Content-Type: application/json
// Body: {"name":"Product A"}
```

### Ví dụ 2 — Fluent Builder tạo hồ sơ nhân viên

**Bài toán:** Trong hệ thống quản lý nhân sự, một `Employee` có nhiều thông tin như họ tên, email, số điện thoại, phòng ban, chức vụ, mức lương và địa chỉ. Trong đó `Name` và `Email` là bắt buộc, còn nhiều thông tin khác là optional và có thể được bổ sung tùy từng nhân viên. Nếu truyền tất cả thông tin qua constructor, constructor sẽ có quá nhiều tham số, khó đọc và dễ truyền nhầm vị trí. Nếu tạo object trước rồi gán từng property bằng setter, `Employee` có thể bị sử dụng khi chưa có đủ các thông tin bắt buộc.

**Ý nghĩa của Builder trong ví dụ này:** `EmployeeBuilder` cho phép xây dựng `Employee` từng bước thông qua các method như `WithName()`, `WithEmail()`, `WithPhone()`, `InDepartment()` và `WithSalary()`. Client chỉ cần cấu hình những thông tin thực sự có. Các trường bắt buộc được kiểm tra tập trung tại `Build()`, nhờ đó chỉ trả về `Employee` khi object đã ở trạng thái hợp lệ. Khi sau này `Employee` có thêm các thuộc tính optional mới, có thể bổ sung method tương ứng vào Builder mà không cần tạo thêm nhiều constructor khác nhau.

**Cách implementation (C#):**

```csharp
// Product
public class Employee
{
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal? Salary { get; set; }
}

// Builder (ConcreteBuilder)
public class EmployeeBuilder
{
    private readonly Employee _employee = new Employee();

    public EmployeeBuilder WithName(string name)
    {
        _employee.Name = name;
        return this;
    }

    public EmployeeBuilder WithEmail(string email)
    {
        _employee.Email = email;
        return this;
    }

    public EmployeeBuilder WithSalary(decimal salary, decimal coefficient)
    {
        _employee.Salary = salary + slary * coefficient;
        return this;
    }

    public Employee Build()
    {
        if (string.IsNullOrEmpty(_employee.Name) ||
            string.IsNullOrEmpty(_employee.Email))
        {
            throw new InvalidOperationException(
                "Name and Email are required");
        }

        return _employee;
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        Employee employee = new EmployeeBuilder()
            .WithName("Nguyen Van An")
            .WithEmail("an@company.com")
            .WithSalary(20000000, 0.3)
            .Build();

        Console.WriteLine(
            $"{employee.Name} - {employee.Salary:N0}");
    }
}

// Nguyen Van An - 26,000,000
```

### Ví dụ 3 — Builder kèm Director dựng sẵn công thức Pizza

**Bài toán:** Một cửa hàng pizza cho phép khách tùy chỉnh `Pizza` theo size, topping và có thêm phô mai hay không, nhưng phần lớn đơn hàng lại rơi vào một vài "công thức" cố định được đặt thường xuyên (ví dụ Margherita, Pepperoni Supreme). Nếu mỗi lần lên đơn một công thức quen thuộc, Client đều phải tự gọi lại đúng thứ tự các bước `WithSize()`, `AddTopping()`, `WithExtraCheese()`... thì logic "công thức nào gồm những bước nào" bị lặp lại và phân tán ở khắp nơi gọi, dễ gây sai sót hoặc không nhất quán giữa các lần dựng cùng một công thức.

**Ý nghĩa của Builder trong ví dụ này:** `PizzaBuilder` vẫn đóng vai trò Builder thông thường, cung cấp các bước `WithSize()`, `AddTopping()`, `WithExtraCheese()` để dựng `Pizza` theo bất kỳ cấu hình nào. Thành phần mới là `PizzaMenuDirector`, biết sẵn đúng thứ tự gọi các bước đó để cho ra từng công thức cố định — `MakeMargherita()` và `MakePepperoniSupreme()`. Nhờ tách biệt "biết cách dựng những gì" (Director) khỏi "biết cách dựng như thế nào" (Builder), Client chỉ cần gọi `director.MakeMargherita(new PizzaBuilder())` là có ngay một `Pizza` đúng công thức, không cần lặp lại từng bước thủ công, trong khi vẫn có thể dùng trực tiếp `PizzaBuilder` khi cần một cấu hình tùy chỉnh khác.

**Cách implementation (C#):**

```csharp
// Product
public class Pizza
{
    public string Size { get; set; }
    public List<string> Toppings { get; } = new List<string>();
    public bool ExtraCheese { get; set; }

    public string Describe()
    {
        var toppings = Toppings.Count > 0 ? string.Join(", ", Toppings) : "khong topping";
        var cheese = ExtraCheese ? " + extra cheese" : "";
        return $"Pizza {Size}: {toppings}{cheese}";
    }
}

// Builder (ConcreteBuilder)
public class PizzaBuilder
{
    private readonly Pizza _pizza = new Pizza();

    public PizzaBuilder WithSize(string size)
    {
        _pizza.Size = size;
        return this;
    }

    public PizzaBuilder AddTopping(string topping)
    {
        _pizza.Toppings.Add(topping);
        return this;
    }

    public PizzaBuilder WithExtraCheese()
    {
        _pizza.ExtraCheese = true;
        return this;
    }

    public Pizza Build() => _pizza;
}

// Director - biết sẵn thứ tự gọi Builder để ra các công thức pizza thường dùng
public class PizzaMenuDirector
{
    public Pizza MakeMargherita(PizzaBuilder builder)
    {
        return builder
            .WithSize("M")
            .AddTopping("Tomato")
            .AddTopping("Mozzarella")
            .Build();
    }

    public Pizza MakePepperoniSupreme(PizzaBuilder builder)
    {
        return builder
            .WithSize("L")
            .AddTopping("Pepperoni")
            .AddTopping("Mushroom")
            .WithExtraCheese()
            .Build();
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var director = new PizzaMenuDirector();

        Pizza margherita = director.MakeMargherita(new PizzaBuilder());
        Pizza pepperoniSupreme = director.MakePepperoniSupreme(new PizzaBuilder());

        Console.WriteLine(margherita.Describe());
        Console.WriteLine(pepperoniSupreme.Describe());
        // Pizza M: Tomato, Mozzarella
        // Pizza L: Pepperoni, Mushroom + extra cheese
    }
}
```
