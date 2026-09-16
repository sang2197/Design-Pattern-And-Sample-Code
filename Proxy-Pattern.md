# Proxy Pattern

## 1. Khái niệm cơ bản

**Proxy** là một Structural Design Pattern, tạo ra một đối tượng đại diện (**Proxy**) đứng trước một đối tượng thật (**RealSubject**), cả hai cùng hiện thực chung một interface (**Subject**), để **kiểm soát việc truy cập** vào đối tượng thật đó — ví dụ trì hoãn khởi tạo, kiểm tra quyền, hay cache lại kết quả.

Pattern gồm 3 thành phần:

- **Subject (interface):** interface chung cho cả `RealSubject` và `Proxy`, để Client thao tác với cả hai theo cùng một cách.
- **RealSubject:** đối tượng thật, chứa logic nghiệp vụ chính.
- **Proxy:** hiện thực `Subject`, giữ tham chiếu tới `RealSubject` và kiểm soát quyền truy cập/lời gọi tới nó.

## 2. Bài toán

Có những trường hợp không nên (hoặc không thể) để Client truy cập trực tiếp vào `RealSubject`:

- `RealSubject` khởi tạo tốn kém (tải ảnh lớn, mở kết nối mạng...) nhưng không phải lúc nào Client cũng cần dùng ngay — khởi tạo quá sớm gây lãng phí tài nguyên.
- Cần kiểm tra quyền truy cập trước khi cho phép gọi `RealSubject`, nhưng không muốn nhúng logic kiểm tra quyền thẳng vào `RealSubject` — điều đó sẽ trộn lẫn logic bảo mật với logic nghiệp vụ chính, vi phạm Single Responsibility.
- Muốn cache lại kết quả gọi `RealSubject` để tránh phải gọi lại nhiều lần tốn kém (ví dụ truy vấn database), nhưng không muốn sửa code của `RealSubject` để nhúng logic cache vào.

Nếu để Client gọi thẳng `RealSubject`, mọi nhu cầu phụ này (trì hoãn khởi tạo, kiểm tra quyền, cache...) buộc phải nhúng thẳng vào chính `RealSubject`, khiến nó ngày càng cồng kềnh và làm nhiều việc không thuộc về nó.

## 3. Ý nghĩa của Proxy

- `Proxy` hiện thực **cùng interface** với `RealSubject`, nên Client sử dụng object đã được bọc **giống hệt như dùng trực tiếp `RealSubject`** — không cần biết có Proxy đứng giữa hay không (tính trong suốt/transparent).
- **Tách logic kiểm soát truy cập** (trì hoãn khởi tạo, kiểm tra quyền, cache...) ra khỏi `RealSubject`, giữ `RealSubject` chỉ tập trung đúng vào nghiệp vụ chính của nó.
- Có thể **trì hoãn khởi tạo `RealSubject`** tới khi thực sự cần dùng (Virtual Proxy) — tiết kiệm tài nguyên nếu `RealSubject` cuối cùng không được dùng tới.
- Có thể **kiểm soát quyền truy cập** trước khi gọi `RealSubject` (Protection Proxy) mà hoàn toàn không phải sửa code của `RealSubject`.
- Tuân thủ **Open/Closed Principle**: muốn thêm một kiểu kiểm soát truy cập mới (ví dụ thêm logging, thêm rate-limit) chỉ cần viết thêm một Proxy mới, không đụng vào `RealSubject` hay các Proxy khác đang có.

## 4. Code mẫu

Mỗi ví dụ dưới đây là một minh họa **đầy đủ** cho một loại Proxy khác nhau: nêu rõ khi nào nên dùng, cách sử dụng, cách hiện thực, và một `Main` chạy thử để in kết quả ra console.

### Ví dụ 1 — Virtual Proxy: trì hoãn tải ảnh lớn

**Khi nào dùng:** tải ảnh từ đĩa tốn thời gian, chỉ nên tải khi thực sự cần hiển thị (`Display()`), không tải ngay lúc khởi tạo đối tượng ảnh.

**Cách sử dụng:** Client làm việc với `IImage` như bình thường, không biết bên dưới là `ImageProxy` hay `RealImage`.

**Cách hiện thực:** `ImageProxy` chỉ khởi tạo `RealImage` (và do đó chỉ tải ảnh) ở lần đầu tiên `Display()` được gọi; các lần gọi sau tái sử dụng `RealImage` đã có.

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

// Chạy thử
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

Chú ý dòng `"Dang tai anh..."` chỉ xuất hiện **một lần** dù `Display()` được gọi hai lần — ảnh chỉ thực sự được tải ở lần gọi đầu tiên.

### Ví dụ 2 — Protection Proxy: kiểm tra quyền trước khi cho phép xoá tài liệu

**Khi nào dùng:** chỉ tài khoản có vai trò `Admin` mới được xoá tài liệu, nhưng không muốn nhúng logic kiểm tra vai trò vào thẳng `DocumentService`.

**Cách sử dụng:** Client vẫn gọi `DeleteDocument()` qua interface `IDocumentService` như bình thường; Proxy tự quyết định có chuyển tiếp lời gọi xuống `RealSubject` hay không.

**Cách hiện thực:** `DocumentServiceProxy` giữ vai trò người dùng hiện tại, kiểm tra điều kiện trước khi gọi `_realService.DeleteDocument()`.

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

// Chạy thử
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

**Khi nào dùng:** truy vấn thông tin sản phẩm từ "database" tốn thời gian; nếu cùng một `id` được truy vấn nhiều lần, nên trả về từ cache thay vì query lại.

**Cách sử dụng:** Client chỉ biết `IProductRepository`, gọi `GetProductById()` như bình thường mà không cần tự quản lý cache ở phía gọi.

**Cách hiện thực:** `CachingProductRepositoryProxy` giữ một `Dictionary` làm cache; chỉ gọi xuống `RealSubject` khi cache chưa có dữ liệu cho `id` đó.

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

// Chạy thử
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

Ở lần gọi `GetProductById(1)` thứ hai, dòng `"Query DB..."` không xuất hiện lại — chứng minh Proxy đã trả kết quả từ cache thay vì gọi lại `RealSubject`.
