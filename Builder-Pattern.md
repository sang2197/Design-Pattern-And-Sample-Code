# Builder Pattern

## 1. Khái niệm cơ bản

**Builder** là một Creational Design Pattern, dùng để xây dựng một đối tượng phức tạp từng bước, thay vì truyền quá nhiều tham số vào một constructor.

Các thành phần chính:

- **Product:** đối tượng phức tạp cần được xây dựng.
- **Builder:** định nghĩa các bước để cấu hình Product.
- **ConcreteBuilder:** thực hiện các bước xây dựng và trả về Product hoàn chỉnh qua `Build()`.
- **Director (tùy chọn):** định nghĩa sẵn thứ tự/cách gọi Builder để tạo một số cấu hình thường dùng. Với Fluent Builder, Client thường gọi Builder trực tiếp nên không nhất thiết cần Director.

## 2. Bài toán

Khi một object có nhiều thuộc tính, đặc biệt nhiều thuộc tính optional hoặc quá trình khởi tạo cần validate, constructor duy nhất buộc phải nhận rất nhiều tham số:

- Constructor trở nên **dài và khó đọc**, các tham số cùng kiểu dữ liệu (`string`, `int`...) rất dễ bị truyền nhầm vị trí mà compiler không phát hiện được.
- Nếu dùng nhiều setter riêng lẻ thay cho constructor, object có thể rơi vào **trạng thái dở dang** (thiếu field bắt buộc) trong lúc code đang thiết lập từng bước.
- Mỗi khi thêm một thuộc tính cấu hình mới, phải sửa signature constructor (hoặc thêm overload mới), ảnh hưởng tới toàn bộ nơi đang gọi constructor cũ.

## 3. Ý nghĩa của Builder

- Builder tách quá trình cấu hình object thành **từng bước có tên rõ ràng**, code khởi tạo đọc gần như ngôn ngữ tự nhiên.
- Có thể **kiểm tra tính hợp lệ của Product** ngay trong `Build()`, chỉ trả về object khi đã đủ điều kiện.
- Dễ **thêm tùy chọn cấu hình mới** (thêm 1 method trên Builder) mà không làm constructor của Product ngày càng dài.
- Cùng một bộ method Builder có thể tạo ra **nhiều đại diện khác nhau** của Product, tùy theo cách gọi — tách biệt "cách dựng" khỏi "kết quả dựng ra".
- Builder không nhằm thay thế mọi constructor — với object đơn giản, constructor hoặc Object Initializer của C# thường là đủ; Builder chỉ đáng dùng khi quá trình khởi tạo thực sự phức tạp.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một tình huống khác nhau: có Product, Builder (ConcreteBuilder), và một `Main` chạy thử để in kết quả ra console. Ví dụ 3 minh họa thêm thành phần Director.

### Ví dụ 1 — Fluent Builder dựng HttpRequest

**Khi nào dùng:** một `HttpRequest` có nhiều phần cấu hình (Url, Method, Headers, Body), phần lớn là optional, và cần validate trước khi coi là hợp lệ (`Url` bắt buộc).

**Cách sử dụng:** Client gọi chuỗi các method `With...()`/`AddHeader()` theo thứ tự tuỳ ý rồi kết thúc bằng `Build()`.

**Cách hiện thực:** mỗi method trên `HttpRequestBuilder` chỉ set một phần của `_request` rồi `return this` để cho phép gọi tiếp (method chaining); `Build()` validate trước khi trả về.

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

// Chạy thử
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

### Ví dụ 2 — Fluent Builder dựng câu lệnh SQL

**Khi nào dùng:** câu lệnh SQL gồm nhiều phần (cột chọn, bảng, điều kiện `WHERE`), số lượng điều kiện có thể thay đổi tuỳ ngữ cảnh gọi.

**Cách sử dụng:** gọi `Select()`, `From()`, `Where()` (có thể gọi `Where()` nhiều lần) rồi `Build()` để lấy `SqlQuery` hoàn chỉnh.

**Cách hiện thực:** `SqlQueryBuilder` tích luỹ dữ liệu vào `_query` qua từng method chaining; `ToSql()` trên `SqlQuery` tự ráp chuỗi SQL cuối cùng từ dữ liệu đã tích luỹ.

```csharp
// Product
public class SqlQuery
{
    public string Table { get; set; }
    public List<string> Columns { get; } = new List<string>();
    public List<string> Conditions { get; } = new List<string>();

    public string ToSql()
    {
        var columns = Columns.Count > 0 ? string.Join(", ", Columns) : "*";
        var sql = $"SELECT {columns} FROM {Table}";

        if (Conditions.Count > 0)
        {
            sql += $" WHERE {string.Join(" AND ", Conditions)}";
        }

        return sql;
    }
}

// Builder (ConcreteBuilder)
public class SqlQueryBuilder
{
    private readonly SqlQuery _query = new SqlQuery();

    public SqlQueryBuilder Select(params string[] columns)
    {
        _query.Columns.AddRange(columns);
        return this;
    }

    public SqlQueryBuilder From(string table)
    {
        _query.Table = table;
        return this;
    }

    public SqlQueryBuilder Where(string condition)
    {
        _query.Conditions.Add(condition);
        return this;
    }

    public SqlQuery Build()
    {
        if (string.IsNullOrEmpty(_query.Table))
        {
            throw new InvalidOperationException("Table is required");
        }

        return _query;
    }
}

// Chạy thử
public class Program
{
    public static void Main()
    {
        SqlQuery query = new SqlQueryBuilder()
            .Select("Id", "Name", "Price")
            .From("Products")
            .Where("Price > 100000")
            .Where("IsActive = 1")
            .Build();

        Console.WriteLine(query.ToSql());
        // SELECT Id, Name, Price FROM Products WHERE Price > 100000 AND IsActive = 1
    }
}
```

### Ví dụ 3 — Builder kèm Director dựng sẵn công thức Pizza

**Khi nào dùng:** có sẵn một vài "công thức" pizza được đặt hàng thường xuyên (Margherita, Pepperoni Supreme...) — muốn dựng nhanh các cấu hình quen thuộc này mà không phải gọi lại thủ công từng bước Builder mỗi lần.

**Cách sử dụng:** Client gọi `director.MakeMargherita(new PizzaBuilder())` thay vì tự gọi `WithSize()`, `AddTopping()`... từng bước một.

**Cách hiện thực:** `PizzaMenuDirector` biết sẵn thứ tự gọi đúng trên `PizzaBuilder` để tạo ra từng công thức, tách biệt "biết cách dựng những gì" (Director) khỏi "biết cách dựng như thế nào" (Builder).

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

// Chạy thử
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
