# Open/Closed Principle (OCP)

## 1. Khái niệm cơ bản

**Open/Closed Principle** phát biểu: **"Class/module nên mở cho việc mở rộng (open for extension), nhưng đóng đối với việc sửa đổi (closed for modification)."** Khi cần thêm hành vi hoặc tính năng mới, nên **thêm code mới** (class mới, implementation mới) thay vì sửa lại code đang hoạt động ổn định — nhờ đó giảm nguy cơ phá vỡ các tính năng đã có khi mở rộng hệ thống.

Trong C#, OCP thường đạt được bằng cách lập trình theo **abstraction** (interface/abstract class) kết hợp **polymorphism**, thay vì dùng `if/switch` liệt kê từng trường hợp cụ thể — đây cũng chính là cơ chế nền tảng đứng sau các pattern như Factory Method, Strategy, Decorator...

## 2. Khi nào nên dùng

Dùng khi:

✅ Hệ thống thường xuyên cần bổ sung thêm loại/biến thể mới (thêm phương thức thanh toán, thêm loại giảm giá...)

✅ Muốn tránh sửa lại code đang chạy ổn định mỗi khi có yêu cầu mới

✅ Logic rẽ nhánh if/switch theo loại đang phình to và lặp lại ở nhiều nơi

✅ Muốn giảm rủi ro hồi quy (regression) khi mở rộng tính năng

## 3. Code examples

### Ví dụ 1 — Tính phí vận chuyển, mở rộng thêm loại vận chuyển mới

**Bài toán:** Method tính phí vận chuyển ban đầu dùng `switch` theo tên loại vận chuyển (Standard, Express...). Mỗi khi công ty logistics bổ sung một loại vận chuyển mới (ví dụ giao trong ngày), phải sửa thẳng vào method tính phí đang được nhiều nơi khác sử dụng, dễ ảnh hưởng tới các loại vận chuyển đã hoạt động ổn định trước đó.

**Ý nghĩa của OCP trong ví dụ này:** Định nghĩa abstraction `IShippingCalculator`, mỗi loại vận chuyển là một implementation riêng. `ShippingFeeService` chỉ tìm đúng `IShippingCalculator` cần dùng trong danh sách đã đăng ký, không còn `switch` theo loại. Thêm loại vận chuyển mới (`SameDayShippingCalculator`) chỉ cần thêm 1 class mới — "đóng" đối với `ShippingFeeService` đang có, "mở" cho việc bổ sung loại vận chuyển.

**Cách implementation (C#):**

```csharp
public interface IShippingCalculator
{
    string Type { get; }
    decimal Calculate(decimal weightKg);
}

public class StandardShippingCalculator : IShippingCalculator
{
    public string Type => "Standard";
    public decimal Calculate(decimal weightKg) => 15000 + weightKg * 3000;
}

public class ExpressShippingCalculator : IShippingCalculator
{
    public string Type => "Express";
    public decimal Calculate(decimal weightKg) => 30000 + weightKg * 5000;
}

// Them loai van chuyen moi - chi them class, khong sua ShippingFeeService
public class SameDayShippingCalculator : IShippingCalculator
{
    public string Type => "SameDay";
    public decimal Calculate(decimal weightKg) => 50000 + weightKg * 8000;
}

public class ShippingFeeService
{
    private readonly List<IShippingCalculator> _calculators;

    public ShippingFeeService(List<IShippingCalculator> calculators)
    {
        _calculators = calculators;
    }

    // "Dong" - khong can sua khi them loai van chuyen moi
    public decimal CalculateFee(string type, decimal weightKg)
    {
        var calculator = _calculators.First(c => c.Type == type);
        return calculator.Calculate(weightKg);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var service = new ShippingFeeService(new List<IShippingCalculator>
        {
            new StandardShippingCalculator(),
            new ExpressShippingCalculator(),
            new SameDayShippingCalculator()
        });

        Console.WriteLine(service.CalculateFee("Standard", 2));
        Console.WriteLine(service.CalculateFee("SameDay", 2));
    }
}
// 21000
// 66000
```

### Ví dụ 2 — Tính tổng diện tích nhiều loại hình, mở rộng thêm hình mới

**Bài toán:** Method tính tổng diện tích ban đầu dùng `if/else` kiểm tra kiểu cụ thể (`is Circle`, `is Rectangle`...) rồi tính diện tích từng hình theo công thức riêng. Mỗi khi thêm một loại hình mới (tam giác...), phải sửa thêm một nhánh `if` vào đúng method này.

**Ý nghĩa của OCP trong ví dụ này:** Định nghĩa abstraction `IShape` với method `Area()`; `AreaCalculator.TotalArea()` chỉ lặp qua danh sách `IShape` và cộng dồn `Area()`, không cần biết cụ thể là hình gì. Thêm hình mới (`Triangle`) chỉ cần thêm 1 class hiện thực `IShape`, `AreaCalculator` không cần sửa.

**Cách implementation (C#):**

```csharp
public interface IShape
{
    double Area();
}

public class Circle : IShape
{
    private readonly double _radius;
    public Circle(double radius) => _radius = radius;
    public double Area() => Math.PI * _radius * _radius;
}

public class Rectangle : IShape
{
    private readonly double _width;
    private readonly double _height;

    public Rectangle(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public double Area() => _width * _height;
}

// Them hinh moi - chi them class, khong sua AreaCalculator
public class Triangle : IShape
{
    private readonly double _base;
    private readonly double _height;

    public Triangle(double baseLength, double height)
    {
        _base = baseLength;
        _height = height;
    }

    public double Area() => 0.5 * _base * _height;
}

public class AreaCalculator
{
    // "Dong" - khong can sua khi them loai hinh moi
    public double TotalArea(List<IShape> shapes) => shapes.Sum(s => s.Area());
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var shapes = new List<IShape>
        {
            new Circle(2),
            new Rectangle(3, 4),
            new Triangle(5, 6)
        };

        Console.WriteLine(new AreaCalculator().TotalArea(shapes));
    }
}
// 39.56637061435917
```

### Ví dụ 3 — Áp dụng khuyến mãi, mở rộng thêm chương trình mới

**Bài toán:** Method áp dụng khuyến mãi ban đầu `switch` theo mã khuyến mãi để tính mức giảm giá. Mỗi chương trình khuyến mãi mới ra mắt lại phải sửa thêm một `case` vào method này, trong khi các khuyến mãi cũ đang chạy không nên bị động vào mỗi lần sửa.

**Ý nghĩa của OCP trong ví dụ này:** Định nghĩa abstraction `IPromotion` với method `Apply()`; `PromotionService` chỉ tìm đúng `IPromotion` theo mã trong danh sách đã đăng ký, không `switch` theo mã khuyến mãi. Thêm khuyến mãi mới (`Black11Promotion`) chỉ cần thêm 1 class mới.

**Cách implementation (C#):**

```csharp
public interface IPromotion
{
    string Code { get; }
    decimal Apply(decimal total);
}

public class SummerSalePromotion : IPromotion
{
    public string Code => "SUMMER10";
    public decimal Apply(decimal total) => total * 0.9m;
}

public class NewCustomerPromotion : IPromotion
{
    public string Code => "NEWCUS";
    public decimal Apply(decimal total) => Math.Max(0, total - 50000);
}

// Them khuyen mai moi - chi them class, khong sua PromotionService
public class Black11Promotion : IPromotion
{
    public string Code => "BLACK11";
    public decimal Apply(decimal total) => total * 0.5m;
}

public class PromotionService
{
    private readonly List<IPromotion> _promotions;

    public PromotionService(List<IPromotion> promotions)
    {
        _promotions = promotions;
    }

    // "Dong" - khong can sua khi them khuyen mai moi
    public decimal ApplyPromotion(string promoCode, decimal total)
    {
        var promotion = _promotions.FirstOrDefault(p => p.Code == promoCode);
        return promotion?.Apply(total) ?? total;
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var service = new PromotionService(new List<IPromotion>
        {
            new SummerSalePromotion(),
            new NewCustomerPromotion(),
            new Black11Promotion()
        });

        Console.WriteLine(service.ApplyPromotion("SUMMER10", 500000));
        Console.WriteLine(service.ApplyPromotion("BLACK11", 500000));
    }
}
// 450000.0
// 250000.0
```
