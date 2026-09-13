# Decorator Pattern

## 1. Khái niệm

**Decorator** là một structural design pattern, cho phép **gắn thêm hành vi/trách nhiệm mới vào một đối tượng cụ thể tại runtime**, bằng cách bọc (wrap) đối tượng đó bên trong một hoặc nhiều lớp Decorator cùng hiện thực chung interface, thay vì sửa class gốc hoặc tạo subclass cho từng tổ hợp tính năng.

Pattern gồm 4 thành phần:

- **Component (interface):** interface chung cho cả đối tượng gốc và các Decorator.
- **ConcreteComponent:** hiện thực gốc, chứa hành vi cơ bản.
- **Decorator (abstract):** cũng hiện thực `Component`, bên trong giữ tham chiếu tới một `Component` khác (có thể là ConcreteComponent hoặc Decorator khác) và mặc định ủy quyền (delegate) lại lời gọi.
- **ConcreteDecorator:** hiện thực cụ thể của Decorator, thêm hành vi trước/sau khi gọi tới Component được bọc bên trong.

## 2. Ý nghĩa

- **Vấn đề gặp phải nếu không có Decorator:** muốn thêm các tính năng phụ (ghi log, cache, nén, mã hoá, tính thêm phí...) có thể kết hợp tự do với nhau, cách làm bằng kế thừa sẽ phải tạo một subclass cho **từng tổ hợp tính năng** (`CoffeeWithMilk`, `CoffeeWithSugar`, `CoffeeWithMilkAndSugar`...) — số lượng class tăng theo cấp số nhân (combinatorial explosion), và tổ hợp phải cố định lúc biên dịch, không đổi được lúc runtime.
- **Decorator giải quyết bằng cách "bọc" nhiều lớp lên trên một Component gốc:** mỗi Decorator chỉ thêm đúng một hành vi, có thể **kết hợp tự do và linh hoạt tại runtime** bằng cách lồng các Decorator vào nhau (`new SugarDecorator(new MilkDecorator(new SimpleCoffee()))`), không cần tạo class riêng cho từng tổ hợp.
- Tuân thủ **Open/Closed Principle**: thêm tính năng mới chỉ cần thêm 1 ConcreteDecorator, không sửa Component gốc hay các Decorator khác đang có.
- Component gốc và Decorator dùng chung interface, nên Client sử dụng đối tượng đã bọc **giống hệt như dùng đối tượng gốc**, không cần biết đang có bao nhiêu lớp Decorator bên trong.

## 3. Code mẫu

### Ví dụ 1 — Component và ConcreteComponent

    public interface ICoffee
    {
        string Describe();
        decimal Cost();
    }

    public class SimpleCoffee : ICoffee
    {
        public string Describe() => "Coffee";

        public decimal Cost() => 20000;
    }

### Ví dụ 2 — Decorator abstract và các ConcreteDecorator

    public abstract class CoffeeDecorator : ICoffee
    {
        protected readonly ICoffee Inner;

        protected CoffeeDecorator(ICoffee inner)
        {
            Inner = inner;
        }

        public virtual string Describe() => Inner.Describe();

        public virtual decimal Cost() => Inner.Cost();
    }

    public class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee inner) : base(inner)
        {
        }

        public override string Describe() => $"{Inner.Describe()} + Milk";

        public override decimal Cost() => Inner.Cost() + 5000;
    }

    public class SugarDecorator : CoffeeDecorator
    {
        public SugarDecorator(ICoffee inner) : base(inner)
        {
        }

        public override string Describe() => $"{Inner.Describe()} + Sugar";

        public override decimal Cost() => Inner.Cost() + 2000;
    }

### Ví dụ 3 — Kết hợp nhiều Decorator tại runtime

    // Client chỉ làm việc với ICoffee, không biết bên trong đang bọc bao nhiêu lớp
    ICoffee order = new SugarDecorator(new MilkDecorator(new SimpleCoffee()));

    Console.WriteLine(order.Describe()); // Coffee + Milk + Sugar
    Console.WriteLine(order.Cost());     // 27000

    // Tổ hợp khác nhau chỉ cần lồng Decorator khác nhau, không cần class mới
    ICoffee anotherOrder = new MilkDecorator(new SimpleCoffee());
    Console.WriteLine(anotherOrder.Describe()); // Coffee + Milk
    Console.WriteLine(anotherOrder.Cost());     // 25000
