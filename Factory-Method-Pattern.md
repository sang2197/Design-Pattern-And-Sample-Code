# Factory Method Pattern

## 1. Khái niệm

**Factory Method** là một Creational Design Pattern, định nghĩa một method để tạo đối tượng, nhưng **cho phép lớp con (subclass) quyết định lớp cụ thể nào sẽ được khởi tạo **, thay vì để logic dùng chung trực tiếp `new` các đối tượng cụ thể.

Pattern gồm 4 thành phần:

- **Product (interface/abstract class):** khai báo interface chung cho các đối tượng được tạo ra.
- **ConcreteProduct:** các implementation cụ thể của `Product`.
- **Creator (abstract class):** khai báo `FactoryMethod()` trả về `Product`. Creator thường chứa logic xử lý dùng chung và sử dụng Product thông qua abstraction mà không cần biết Product cụ thể là gì.
- **ConcreteCreator:** override/implement FactoryMethod() và quyết định ConcreteProduct cụ thể nào sẽ được tạo ra.

## 2. Ý nghĩa

- **Vấn đề khi không sử dụng Factory Method:** logic xử lý dùng chung có thể bị trộn với logic khởi tạo các đối tượng cụ thể, ví dụ new VnPayPayment(), new MomoPayment(). Khi số lượng biến thể tăng lên, Creator có thể phải chứa nhiều if/switch và thường xuyên bị sửa mỗi khi bổ sung một loại mới.
- **Factory Method tách việc tạo đối tượng khỏi logic sử dụng đối tượng:** `Creator` định nghĩa luồng xử lý chung và làm việc với abstraction `Product`. Việc quyết định `ConcreteProduct` nào được tạo được giao cho `ConcreteCreator` thông qua `FactoryMethod()`.
- Khi cần thêm một biến thể mới, có thể tạo thêm `ConcreteProduct` và `ConcreteCreator` tương ứng mà không cần sửa workflow chung trong `Creator`
- Tuân thủ **Open/Closed Principle**: phần workflow ổn định có thể được giữ nguyên trong khi hệ thống vẫn có thể mở rộng bằng các implementation mới.

## 3. Code mẫu

### Ví dụ 1 — Product và Creator

```csharp
public interface IPaymentMethod
{
    string Pay(decimal amount);
}

public class VnPayPayment : IPaymentMethod
{
    public string Pay(decimal amount) => $"Thanh toan {amount:N0} qua VNPay";
}

public class MomoPayment : IPaymentMethod
{
    public string Pay(decimal amount) => $"Thanh toan {amount:N0} qua Momo";
}

public abstract class CheckoutProcessor
{
    // Factory Method - để lớp con quyết định tạo IPaymentMethod nào
    protected abstract IPaymentMethod CreatePaymentMethod();

    // Logic nghiệp vụ dùng chung, chỉ phụ thuộc abstraction IPaymentMethod
    public string Checkout(decimal amount)
    {
        var payment = CreatePaymentMethod();
        var result = payment.Pay(amount);

        return $"[Checkout] {result}";
    }
}
```

### Ví dụ 2 — ConcreteCreator hiện thực Factory Method

```csharp
public class VnPayCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePaymentMethod()
    {
        return new VnPayPayment();
    }
}

public class MomoCheckoutProcessor : CheckoutProcessor
{
    protected override IPaymentMethod CreatePaymentMethod()
    {
        return new MomoPayment();
    }
}
```

### Ví dụ 3 — Sử dụng tại nơi gọi

```csharp
public class CheckoutController
{
    public string Handle(string method, decimal amount)
    {
        // Chọn đúng ConcreteCreator, phần logic Checkout() dùng chung không đổi
        CheckoutProcessor processor = method switch
        {
            "vnpay" => new VnPayCheckoutProcessor(),
            "momo" => new MomoCheckoutProcessor(),
            _ => throw new ArgumentException("Payment method not supported")
        };

        return processor.Checkout(amount);
    }
}
```
