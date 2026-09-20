namespace Exercises.Decorator.Example2;

// Decorator - Vi du 2: Dinh dang thong bao (them tien to, viet hoa) ma khong sua lop goc
// Xem lai: Decorator-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface INotifier (Component) { string Send(string message); }
// - class BasicNotifier : INotifier (ConcreteComponent) -> Send() tra ve nguyen message
// - abstract class NotifierDecorator : INotifier (Decorator)
//     protected readonly INotifier Inner; constructor nhan INotifier inner
//     virtual Send(message) => Inner.Send(message)
// - class PrefixDecorator : NotifierDecorator (ConcreteDecorator)
//     constructor nhan (INotifier inner, string prefix)
//     Send(message) -> "{prefix} {Inner.Send(message)}"
// - class UpperCaseDecorator : NotifierDecorator (ConcreteDecorator)
//     Send(message) -> Inner.Send(message).ToUpper()

// Component
public interface IProductService
{
    string GetProductById(int id);
}

// Concrete Component
public class ProductService : IProductService
{
    public string GetProductById(int id) => $"Get product id: {id} from database";
}

// Decorator
public abstract class ProductDecorator : IProductService
{
    protected readonly IProductService Inner;
    public ProductDecorator(IProductService inner)
    {
        Inner = inner;
    }

    public virtual string GetProductById(int id)
    {
        return Inner.GetProductById(id);
    }
}

// Concrete Decorator
public class LogDecorator : ProductDecorator
{
    public LogDecorator(IProductService inner) : base(inner)
    {
    }

    public override string GetProductById(int id)
    {
        Console.WriteLine($"[LOG] Getting product id: {id}");
        var result = Inner.GetProductById(id);
        Console.WriteLine($"[LOG] Product id: {id} loaded");
        return result;
    }
}

public class CacheDecorator : ProductDecorator
{
    private readonly Dictionary<int, string> _cache = new();
    public CacheDecorator(IProductService inner) : base(inner)
    {
    }

    public override string GetProductById(int id)
    {
        if(_cache.TryGetValue(id, out var product))
        {
            return $"[Cache] Get product id: {id} from cache";
        }

        var result = Inner.GetProductById(id);
        _cache[id] = result;
        return result;
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        IProductService product = new LogDecorator(new CacheDecorator(new ProductService()));
        Console.WriteLine(product.GetProductById(12));
    }
}