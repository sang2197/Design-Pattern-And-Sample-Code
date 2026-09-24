# ORM (Object-Relational Mapping)

## 1. ORM là gì?

**ORM** là công cụ giúp **ánh xạ (map) giữa class trong code và bảng trong database**. Thay vì viết SQL bằng tay, ta thao tác với object, ORM tự sinh SQL.

| Code (OOP) | Database |
|---|---|
| Class | Bảng (table) |
| Object (instance) | Dòng (row) |
| Property | Cột (column) |
| Tham chiếu tới object khác | Khóa ngoại (foreign key) |

```csharp
// Không dùng ORM: tự viết SQL, tự đọc từng cột
var cmd = new SqlCommand("SELECT Id, Name FROM Users WHERE Id = @id", conn);
cmd.Parameters.AddWithValue("@id", 1);
var reader = cmd.ExecuteReader();
// ... reader["Name"] ...

// Dùng ORM (EF Core): làm việc với object
var user = db.Users.FirstOrDefault(u => u.Id == 1);
```

**Vấn đề ORM giải quyết:** "Object-relational impedance mismatch" — OOP có kế thừa, tham chiếu, collection; còn DB chỉ có bảng, dòng, khóa. ORM làm cầu nối giữa 2 thế giới này.

**ORM phổ biến:** Entity Framework Core (.NET), Hibernate (Java), Sequelize/Prisma/TypeORM (Node.js), Django ORM/SQLAlchemy (Python).
**Micro-ORM:** Dapper (.NET) — chỉ map kết quả SQL sang object, SQL vẫn tự viết.

## 2. Ưu và nhược điểm

| Ưu điểm | Nhược điểm |
|---|---|
| Viết code nhanh, ít SQL lặp lại (CRUD) | Query phức tạp có thể sinh SQL kém hiệu quả |
| Code dễ đọc, dễ bảo trì, type-safe (sai tên cột thì lỗi lúc compile) | Chậm hơn SQL thuần một chút (overhead) |
| Chống SQL Injection (tự dùng parameter) | Dễ dính lỗi hiệu năng nếu không hiểu (N+1, load thừa) |
| Dễ đổi loại DB (SQL Server → PostgreSQL) | Phải học thêm công cụ, "che" mất SQL thật |
| Có migration để quản lý thay đổi schema | Không phù hợp với báo cáo/thống kê cực nặng |

**Khi nào dùng ORM?** Ứng dụng CRUD thông thường, nghiệp vụ nhiều.
**Khi nào dùng SQL thuần / Dapper?** Query phức tạp, báo cáo, cần tối ưu hiệu năng tối đa. Thực tế hay **kết hợp cả hai**.

## 3. Các thành phần chính trong EF Core

- **Entity:** class đại diện cho một bảng.
- **DbContext:** "phiên làm việc" với DB — quản lý kết nối, theo dõi thay đổi, lưu dữ liệu.
- **DbSet\<T\>:** đại diện cho một bảng, dùng để query/thêm/xóa.

```csharp
public class User
{
    public int Id { get; set; }                  // Mặc định: tên "Id" => khóa chính
    public string Name { get; set; } = "";
    public List<Order> Orders { get; set; } = new(); // Navigation property (1 user - n order)
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public int UserId { get; set; }              // Khóa ngoại
    public User User { get; set; } = null!;      // Navigation property
}

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Server=.;Database=Shop;Trusted_Connection=True;");
}
```

## 4. Cấu hình mapping — 3 cách

1. **Convention (quy ước):** tự hiểu theo tên. `Id` hoặc `UserId` → khóa chính; `UserId` + `User` → khóa ngoại.
2. **Data Annotations:** gắn attribute lên class.
3. **Fluent API:** cấu hình trong `OnModelCreating` — mạnh nhất, tách cấu hình khỏi entity.

```csharp
// Data Annotations
public class Product
{
    [Key] public int ProductId { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
}

// Fluent API
protected override void OnModelCreating(ModelBuilder mb)
{
    mb.Entity<Product>(e =>
    {
        e.ToTable("Products");
        e.HasKey(p => p.ProductId);
        e.Property(p => p.Name).IsRequired().HasMaxLength(100);
        e.HasIndex(p => p.Name).IsUnique();
    });
}
```

Thứ tự ưu tiên: **Fluent API > Data Annotations > Convention**.

## 5. Code First vs Database First

| | Code First | Database First |
|---|---|---|
| Bắt đầu từ | Viết class C# trước | DB đã có sẵn |
| Tạo DB bằng | Migration sinh ra DB | Scaffold sinh ra class từ DB |
| Hợp với | Dự án mới | Dự án có DB cũ (legacy) |

**Migration** = "git cho schema DB": mỗi lần đổi model, tạo một migration ghi lại thay đổi, có thể apply hoặc rollback.

```bash
dotnet ef migrations add AddPhoneToUser   # tạo migration từ thay đổi của model
dotnet ef database update                 # áp dụng vào DB
dotnet ef database update PreviousMigration # rollback
dotnet ef dbcontext scaffold "<conn>" Microsoft.EntityFrameworkCore.SqlServer  # Database First
```

## 6. Quan hệ giữa các bảng

```csharp
// 1 - 1: User có 1 Profile
mb.Entity<User>().HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<Profile>(p => p.UserId);

// 1 - n: User có nhiều Order
mb.Entity<Order>().HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId)
  .OnDelete(DeleteBehavior.Cascade);   // xóa User => xóa luôn Order

// n - n: Student - Course (EF Core 5+ tự tạo bảng trung gian StudentCourse)
public class Student { public int Id { get; set; } public List<Course> Courses { get; set; } = new(); }
public class Course  { public int Id { get; set; } public List<Student> Students { get; set; } = new(); }
```

## 7. CRUD cơ bản

```csharp
using var db = new AppDbContext();

// Create
db.Users.Add(new User { Name = "An" });
db.SaveChanges();                       // Lúc này mới thực sự chạy INSERT

// Read
var user = db.Users.Find(1);            // tìm theo khóa chính
var list = db.Users.Where(u => u.Name.StartsWith("A")).OrderBy(u => u.Name).ToList();

// Update
user!.Name = "An Nguyen";
db.SaveChanges();                       // EF tự phát hiện thay đổi => UPDATE

// Delete
db.Users.Remove(user);
db.SaveChanges();
```

**Ghi nhớ:** `Add/Remove/sửa property` chỉ đánh dấu trong bộ nhớ, **chỉ `SaveChanges()` mới gửi xuống DB**, và tất cả được gói trong **1 transaction**.

## 8. Change Tracking (theo dõi thay đổi)

DbContext nhớ trạng thái của từng entity:

| State | Ý nghĩa | Khi SaveChanges |
|---|---|---|
| `Added` | Mới thêm | INSERT |
| `Modified` | Đã sửa | UPDATE |
| `Deleted` | Đã đánh dấu xóa | DELETE |
| `Unchanged` | Load lên, chưa sửa | Không làm gì |
| `Detached` | DbContext không theo dõi | Không làm gì |

**AsNoTracking():** dùng khi **chỉ đọc** (hiển thị danh sách) → không theo dõi → nhanh hơn, ít tốn RAM.

```csharp
var users = db.Users.AsNoTracking().ToList();
```

## 9. Loading dữ liệu liên quan — 3 kiểu

```csharp
// 1. Eager loading: load luôn dữ liệu liên quan trong 1 lần (JOIN)
var users = db.Users.Include(u => u.Orders).ToList();

// 2. Explicit loading: load sau, khi cần, bằng lệnh tường minh
var u = db.Users.Find(1);
db.Entry(u!).Collection(x => x.Orders).Load();

// 3. Lazy loading: tự động load khi truy cập property (cần bật proxy + virtual)
public virtual List<Order> Orders { get; set; }
var count = u.Orders.Count;  // lúc này mới chạy query
```

### Vấn đề N+1 query (rất hay hỏi)

```csharp
// SAI: 1 query lấy N user + N query lấy order của từng user = N+1 query
var users = db.Users.ToList();
foreach (var u in users)
    Console.WriteLine(u.Orders.Count);   // mỗi vòng lặp 1 query (lazy loading)

// ĐÚNG: chỉ 1 query
var users2 = db.Users.Include(u => u.Orders).ToList();

// TỐT HƠN: chỉ lấy đúng cột cần (projection)
var result = db.Users.Select(u => new { u.Name, OrderCount = u.Orders.Count }).ToList();
```

## 10. IQueryable vs IEnumerable & Deferred Execution

- **IQueryable:** câu query **chưa chạy**, còn đang "xây" → EF dịch toàn bộ thành SQL, **lọc ở DB**.
- **IEnumerable (sau ToList):** dữ liệu **đã kéo về RAM**, lọc tiếp là lọc trong bộ nhớ.
- **Deferred execution:** query chỉ chạy khi gọi `ToList()`, `First()`, `Count()`, `foreach`...

```csharp
// TỐT: WHERE chạy trong SQL, chỉ lấy 10 dòng về
var a = db.Users.Where(u => u.Age > 18).Take(10).ToList();

// TỆ: kéo cả bảng về RAM rồi mới lọc
var b = db.Users.ToList().Where(u => u.Age > 18).Take(10);
```

## 11. Transaction, Raw SQL, Concurrency

```csharp
// Transaction tường minh (khi cần nhiều lần SaveChanges thành 1 khối)
using var tx = db.Database.BeginTransaction();
try
{
    db.Accounts.Find(1)!.Balance -= 100; db.SaveChanges();
    db.Accounts.Find(2)!.Balance += 100; db.SaveChanges();
    tx.Commit();
}
catch { tx.Rollback(); throw; }

// Raw SQL (vẫn an toàn nhờ tham số hóa)
var list = db.Users.FromSqlInterpolated($"SELECT * FROM Users WHERE Name = {name}").ToList();

// Optimistic concurrency: 2 người sửa cùng lúc => người sau bị báo lỗi
public class Product { public int Id { get; set; } [Timestamp] public byte[] RowVersion { get; set; } = null!; }
// => DbUpdateConcurrencyException nếu dòng đã bị người khác sửa
```

## 12. Repository & Unit of Work (hay đi kèm ORM)

- **Repository:** gom logic truy cập dữ liệu của 1 entity vào 1 chỗ, service chỉ gọi interface.
- **Unit of Work:** gom nhiều thay đổi thành 1 lần commit. (Thực ra `DbContext` **đã là** Unit of Work, `DbSet` **đã là** Repository.)

```csharp
public interface IUserRepository
{
    User? GetById(int id);
    void Add(User user);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;
    public User? GetById(int id) => _db.Users.Find(id);
    public void Add(User user) => _db.Users.Add(user);
}
```

Lợi ích: service không phụ thuộc EF (đúng **Dependency Inversion**), dễ mock khi viết unit test.

## 13. Mẹo tối ưu hiệu năng

1. Dùng `AsNoTracking()` cho query chỉ đọc.
2. `Select` chỉ lấy cột cần thiết (projection).
3. Tránh N+1: dùng `Include` hoặc projection.
4. Lọc/phân trang ở DB (`Where`, `Skip`, `Take`) **trước** `ToList()`.
5. Dùng `async` (`ToListAsync`, `SaveChangesAsync`) trong web API.
6. Không giữ DbContext sống quá lâu (Web: mỗi request một DbContext — `AddDbContext` mặc định là **Scoped**).
7. Thêm index cho cột hay dùng để tìm kiếm.

## 14. Câu hỏi hay gặp

**H: ORM là gì? Tại sao dùng?**
Đ: Công cụ map class ↔ bảng, giúp thao tác DB bằng object thay vì SQL. Giúp code nhanh, dễ đọc, chống SQL injection, dễ đổi DB.

**H: N+1 là gì, sửa thế nào?**
Đ: Lấy 1 danh sách (1 query) rồi mỗi phần tử lại query thêm dữ liệu liên quan (N query). Sửa bằng eager loading (`Include`) hoặc projection (`Select`).

**H: Eager / Lazy / Explicit loading khác gì?**
Đ: Eager — load luôn từ đầu. Lazy — tự load khi truy cập. Explicit — tự gọi lệnh load khi cần.

**H: Code First khác Database First?**
Đ: Code First viết class trước rồi sinh DB bằng migration; Database First có DB trước rồi sinh class.

**H: Khi nào dùng AsNoTracking?**
Đ: Khi chỉ đọc dữ liệu, không sửa — nhanh hơn và tốn ít bộ nhớ hơn.

**H: IQueryable khác IEnumerable?**
Đ: IQueryable lọc ở DB (dịch ra SQL); IEnumerable lọc trong RAM sau khi đã tải dữ liệu về.

## 15. Tóm tắt học thuộc

- ORM = map **class ↔ bảng, object ↔ dòng, property ↔ cột**.
- EF Core: **Entity + DbContext + DbSet**; cấu hình bằng **Convention / Annotation / Fluent API**.
- **Code First** (class → migration → DB) vs **Database First** (DB → scaffold → class).
- Chỉ **SaveChanges()** mới ghi xuống DB, gói trong 1 transaction.
- Trạng thái entity: **Added / Modified / Deleted / Unchanged / Detached**.
- Loading: **Eager (Include) / Lazy / Explicit**. Coi chừng **N+1**.
- **IQueryable** lọc ở DB, **IEnumerable** lọc trong RAM; query chạy khi gọi `ToList()`.
- Tối ưu: **AsNoTracking, Select, phân trang ở DB, async, index**.
