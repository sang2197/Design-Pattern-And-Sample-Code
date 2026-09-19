# Proxy Pattern

## 1. Khái niệm cơ bản

**Proxy** là một Structural Design Pattern, tạo ra một đối tượng đại diện (**Proxy**) đứng trước một đối tượng thật (**RealSubject**), cả hai cùng hiện thực chung một interface (**Subject**), để **kiểm soát việc truy cập** vào đối tượng thật đó — ví dụ trì hoãn khởi tạo, kiểm tra quyền, hay cache lại kết quả.

Pattern gồm 3 thành phần:

- **Subject (interface):** interface chung cho cả `RealSubject` và `Proxy`, để Client thao tác với cả hai theo cùng một cách.
- **RealSubject:** đối tượng thật, chứa logic nghiệp vụ chính.
- **Proxy:** hiện thực `Subject`, giữ tham chiếu tới `RealSubject` và kiểm soát quyền truy cập/lời gọi tới nó.

## 2. Khi nào nên dùng

Dùng khi:

✅ Cần kiểm soát việc truy cập vào một object mà không sửa code của object đó

✅ Việc khởi tạo object thật tốn kém, muốn trì hoãn tới khi thực sự cần dùng

✅ Cần thêm kiểm tra quyền, logging hoặc cache trước khi gọi tới object thật

✅ Muốn Client dùng Proxy y hệt như dùng trực tiếp object thật (trong suốt)

## 3. Code examples

### Ví dụ 1 — Virtual Proxy: trì hoãn tải ảnh lớn

**Bài toán:** Ứng dụng cần hiển thị hình ảnh, nhưng việc tải ảnh từ đĩa tốn thời gian và tài nguyên. Nếu `RealImage` tải ảnh ngay trong constructor, thì mỗi lần khởi tạo danh sách ảnh (ví dụ một gallery với hàng trăm ảnh) toàn bộ ảnh sẽ bị tải ngay cả khi người dùng chưa thực sự xem tới, gây lãng phí tài nguyên và làm chậm ứng dụng. Nếu để Client tự kiểm tra "đã tải ảnh chưa" trước mỗi lần hiển thị, logic trì hoãn tải sẽ bị lặp lại ở mọi nơi gọi tới ảnh thay vì nằm gọn một chỗ.

**Ý nghĩa của Proxy trong ví dụ này:** `ImageProxy` hiện thực cùng interface `IImage` với `RealImage`, nên Client vẫn thao tác qua `IImage` như bình thường mà không biết đang làm việc với Proxy hay với ảnh thật. `ImageProxy` chỉ giữ tên file lúc khởi tạo, chưa tạo `RealImage`; đến khi `Display()` được gọi lần đầu tiên, nó mới khởi tạo `RealImage` — lúc đó ảnh mới thực sự được tải từ đĩa. Những lần gọi `Display()` sau đó tái sử dụng `RealImage` đã có sẵn nên không tải lại, thể hiện qua dòng `"Dang tai anh..."` chỉ xuất hiện đúng một lần dù `Display()` được gọi hai lần.

**Cách implementation (C#):**

```csharp
// Subject
public interface IImage
{
    void Display();
}

// RealSubject
public class RealImage : IImage
{
    private readonly string _fileName;

    public RealImage(string fileName)
    {
        _fileName = fileName;
        LoadFromDisk();
    }

    private void LoadFromDisk()
    {
        Console.WriteLine($"[RealImage] Dang tai anh {_fileName} tu dia...");
    }

    public void Display()
    {
        Console.WriteLine($"[RealImage] Hien thi anh {_fileName}");
    }
}

// Proxy
public class ImageProxy : IImage
{
    private readonly string _fileName;
    private RealImage _realImage;

    public ImageProxy(string fileName)
    {
        _fileName = fileName;
    }

    public void Display()
    {
        // Chi tai anh that khi lan dau Display() duoc goi
        if (_realImage == null)
        {
            _realImage = new RealImage(_fileName);
        }

        _realImage.Display();
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IImage image = new ImageProxy("photo.png");
        Console.WriteLine("Anh chua duoc tai...");

        image.Display();
        image.Display();
    }
}
// Anh chua duoc tai...
// [RealImage] Dang tai anh photo.png tu dia...
// [RealImage] Hien thi anh photo.png
// [RealImage] Hien thi anh photo.png
```

### Ví dụ 2 — Protection Proxy: kiểm tra quyền trước khi cho phép xoá tài liệu

**Bài toán:** Hệ thống quản lý tài liệu cho phép xoá tài liệu, nhưng theo nghiệp vụ chỉ tài khoản có vai trò `Admin` mới được thực hiện thao tác này. Nếu để Client gọi thẳng `DeleteDocument()` trên `DocumentService`, thì logic kiểm tra vai trò hoặc phải nhúng thẳng vào `DocumentService`, hoặc phải lặp lại ở mọi nơi gọi tới nó. Cách nào cũng khiến `DocumentService` lẫn lộn giữa logic nghiệp vụ chính (xoá tài liệu) và logic bảo mật (kiểm tra quyền), đồng thời dễ bị bỏ sót kiểm tra nếu sau này có thêm điểm gọi mới.

**Ý nghĩa của Proxy trong ví dụ này:** `DocumentServiceProxy` hiện thực `IDocumentService` giống hệt `DocumentService` thật, nên Client vẫn gọi `DeleteDocument()` qua interface như bình thường, không cần biết có lớp kiểm tra quyền đứng giữa. `DocumentServiceProxy` giữ vai trò người dùng hiện tại và kiểm tra điều kiện đó trước khi quyết định có chuyển tiếp lời gọi xuống `_realService.DeleteDocument()` hay không — nếu không phải `Admin`, Proxy từ chối ngay mà không hề gọi tới `RealSubject`, còn `DocumentService` hoàn toàn không biết gì về logic phân quyền này, giữ được đúng một trách nhiệm.

**Cách implementation (C#):**

```csharp
// Subject
public interface IDocumentService
{
    void DeleteDocument(string documentId);
}

// RealSubject
public class DocumentService : IDocumentService
{
    public void DeleteDocument(string documentId)
    {
        Console.WriteLine($"[DocumentService] Da xoa tai lieu {documentId}");
    }
}

// Proxy
public class DocumentServiceProxy : IDocumentService
{
    private readonly DocumentService _realService;
    private readonly string _currentUserRole;

    public DocumentServiceProxy(DocumentService realService, string currentUserRole)
    {
        _realService = realService;
        _currentUserRole = currentUserRole;
    }

    public void DeleteDocument(string documentId)
    {
        if (_currentUserRole != "Admin")
        {
            Console.WriteLine($"[Proxy] Tu choi: {_currentUserRole} khong co quyen xoa tai lieu");
            return;
        }

        _realService.DeleteDocument(documentId);
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        var realService = new DocumentService();

        IDocumentService userProxy = new DocumentServiceProxy(realService, "User");
        userProxy.DeleteDocument("DOC001");

        IDocumentService adminProxy = new DocumentServiceProxy(realService, "Admin");
        adminProxy.DeleteDocument("DOC001");
    }
}
// [Proxy] Tu choi: User khong co quyen xoa tai lieu
// [DocumentService] Da xoa tai lieu DOC001
```

### Ví dụ 3 — Caching Proxy: cache lại kết quả truy vấn tốn kém

**Bài toán:** Truy vấn thông tin sản phẩm từ database tốn thời gian và tài nguyên (I/O, network...). Nếu cùng một `id` sản phẩm được truy vấn nhiều lần trong thời gian ngắn — ví dụ trang chi tiết sản phẩm được người dùng mở lại nhiều lần — gọi thẳng `ProductRepository` mỗi lần sẽ khiến hệ thống truy vấn database lặp lại không cần thiết, làm chậm phản hồi. Nếu nhúng logic cache trực tiếp vào `ProductRepository`, lớp này sẽ vừa phải lo truy vấn dữ liệu vừa phải lo quản lý cache, vi phạm Single Responsibility.

**Ý nghĩa của Proxy trong ví dụ này:** `CachingProductRepositoryProxy` hiện thực `IProductRepository`, nên Client chỉ biết làm việc qua interface này và gọi `GetProductById()` như bình thường mà không cần tự quản lý cache ở phía gọi. Proxy giữ một `Dictionary` làm cache: khi `GetProductById()` được gọi, Proxy kiểm tra cache trước — nếu đã có dữ liệu cho `id` đó, trả về ngay từ cache mà không hề gọi xuống `_realRepository`; nếu chưa có, mới gọi `ProductRepository.GetProductById()` thật, lưu kết quả vào cache rồi trả về. Ở lần gọi `GetProductById(1)` thứ hai, dòng `"Query DB..."` không còn xuất hiện lại, chứng minh Proxy đã trả kết quả từ cache thay vì gọi lại `RealSubject`.

**Cách implementation (C#):**

```csharp
// Subject
public interface IProductRepository
{
    string GetProductById(int id);
}

// RealSubject
public class ProductRepository : IProductRepository
{
    public string GetProductById(int id)
    {
        Console.WriteLine($"[ProductRepository] Query DB cho san pham {id}...");
        return $"Product-{id}";
    }
}

// Proxy
public class CachingProductRepositoryProxy : IProductRepository
{
    private readonly ProductRepository _realRepository;
    private readonly Dictionary<int, string> _cache = new Dictionary<int, string>();

    public CachingProductRepositoryProxy(ProductRepository realRepository)
    {
        _realRepository = realRepository;
    }

    public string GetProductById(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            Console.WriteLine($"[Proxy] Lay san pham {id} tu cache");
            return cached;
        }

        var product = _realRepository.GetProductById(id);
        _cache[id] = product;
        return product;
    }
}
```

**Cách sử dụng (C#):**

```csharp
public class Program
{
    public static void Main()
    {
        IProductRepository repository = new CachingProductRepositoryProxy(new ProductRepository());

        Console.WriteLine(repository.GetProductById(1));
        Console.WriteLine(repository.GetProductById(1));
        Console.WriteLine(repository.GetProductById(2));
    }
}
// [ProductRepository] Query DB cho san pham 1...
// Product-1
// [Proxy] Lay san pham 1 tu cache
// Product-1
// [ProductRepository] Query DB cho san pham 2...
// Product-2
```
