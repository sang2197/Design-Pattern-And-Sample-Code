# Thiết kế Database & Chuẩn hóa (Normal Forms)

## 1. Các khái niệm cơ bản

- **Database quan hệ (RDBMS):** lưu dữ liệu dạng **bảng** (table) có quan hệ với nhau. VD: SQL Server, MySQL, PostgreSQL, Oracle.
- **Bảng (table / relation)** — **dòng (row / record / tuple)** — **cột (column / field / attribute)**.
- **Schema:** "bản thiết kế" của DB — gồm bảng, cột, kiểu dữ liệu, khóa, ràng buộc.

### Các loại khóa (Key)

| Khóa | Ý nghĩa | Ví dụ (bảng `Students`) |
|---|---|---|
| **Super Key** | Tập cột bất kỳ xác định **duy nhất** 1 dòng | `{StudentId}`, `{StudentId, Name}`, `{Email}` |
| **Candidate Key** | Super key **tối thiểu** (bỏ bớt cột nào là không còn duy nhất) | `{StudentId}`, `{Email}` |
| **Primary Key (PK)** | Candidate key **được chọn** làm khóa chính. Không NULL, không trùng | `StudentId` |
| **Alternate Key** | Candidate key **không được chọn** làm PK | `Email` (đặt UNIQUE) |
| **Foreign Key (FK)** | Cột tham chiếu tới PK của bảng khác → tạo **quan hệ** | `Orders.StudentId` → `Students.StudentId` |
| **Composite Key** | Khóa gồm **nhiều cột** | `(StudentId, CourseId)` trong bảng `Enrollments` |

**Natural key vs Surrogate key:**
- **Natural key:** khóa có ý nghĩa thực tế (số CCCD, email). Nhược: có thể thay đổi, dài.
- **Surrogate key:** khóa "nhân tạo" không mang ý nghĩa (`Id INT IDENTITY`, `GUID`). **Thường dùng trong thực tế** vì ổn định, ngắn, không bao giờ đổi.

### Các loại quan hệ

| Quan hệ | Ví dụ | Cách thiết kế |
|---|---|---|
| **1 – 1** | 1 người – 1 hộ chiếu | FK đặt ở 1 bên + UNIQUE (hoặc dùng chung PK) |
| **1 – n** | 1 khách hàng – nhiều đơn hàng | FK đặt ở **bên nhiều** (`Orders.CustomerId`) |
| **n – n** | Sinh viên – Môn học | Tạo **bảng trung gian** (`Enrollments(StudentId, CourseId)`) |

**ERD (Entity Relationship Diagram):** sơ đồ thể hiện các thực thể (bảng), thuộc tính (cột) và quan hệ giữa chúng — vẽ **trước** khi tạo bảng.

```
Customers 1 ────< n Orders 1 ────< n OrderDetails n >──── 1 Products
```

## 2. Ràng buộc toàn vẹn (Constraints)

```sql
CREATE TABLE Customers (
    CustomerId  INT IDENTITY PRIMARY KEY,          -- PK: không NULL, không trùng
    Email       VARCHAR(100) NOT NULL UNIQUE,      -- bắt buộc, không trùng
    FullName    NVARCHAR(100) NOT NULL,
    Age         INT CHECK (Age >= 0),              -- điều kiện hợp lệ
    CreatedAt   DATETIME2 DEFAULT SYSDATETIME()    -- giá trị mặc định
);

CREATE TABLE Orders (
    OrderId     INT IDENTITY PRIMARY KEY,
    CustomerId  INT NOT NULL,
    OrderDate   DATE NOT NULL,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId)
        REFERENCES Customers(CustomerId)
        ON DELETE CASCADE                          -- xóa khách => xóa luôn đơn
);
```

- **Entity integrity:** PK không NULL, không trùng.
- **Referential integrity:** FK phải trỏ tới dòng **có tồn tại** (không có đơn hàng của khách "ma").
- Hành vi khi xóa/sửa dòng cha: `CASCADE` (xóa theo), `SET NULL`, `NO ACTION / RESTRICT` (cấm xóa).

## 3. Tại sao phải chuẩn hóa? — Các dị thường (Anomaly)

Xét bảng thiết kế **tồi**, gộp mọi thứ vào 1 bảng:

| OrderId | OrderDate | CustomerId | CustomerName | ProductId | ProductName | Qty |
|---|---|---|---|---|---|---|
| 1 | 10/01 | C1 | An | P1 | Bút | 2 |
| 1 | 10/01 | C1 | An | P2 | Vở | 3 |
| 2 | 11/01 | C1 | An | P1 | Bút | 1 |

Các vấn đề:
- **Dư thừa dữ liệu:** tên "An", "Bút" lặp lại nhiều lần → tốn chỗ.
- **Update anomaly (dị thường khi sửa):** đổi tên khách C1 phải sửa **nhiều dòng**, sót 1 dòng là dữ liệu **mâu thuẫn**.
- **Insert anomaly (dị thường khi thêm):** muốn thêm sản phẩm mới P3 nhưng chưa ai mua → **không thêm được** (vì thiếu OrderId).
- **Delete anomaly (dị thường khi xóa):** xóa đơn hàng số 2 → nếu đó là đơn duy nhất có sản phẩm X thì **mất luôn thông tin sản phẩm X**.

➡ **Chuẩn hóa (Normalization)** = tách bảng hợp lý để **giảm dư thừa** và **loại bỏ dị thường**.

## 4. Phụ thuộc hàm (Functional Dependency) — nền tảng để hiểu NF

**A → B** (A xác định B): biết giá trị A thì biết **duy nhất** giá trị B.
- `StudentId → StudentName` (biết mã SV thì biết tên).
- `(OrderId, ProductId) → Qty`.

Các loại cần nhớ:
- **Phụ thuộc đầy đủ:** B phụ thuộc vào **toàn bộ** khóa. `(OrderId, ProductId) → Qty`.
- **Phụ thuộc bộ phận (partial):** B chỉ phụ thuộc vào **một phần** của khóa ghép. `(OrderId, ProductId)` nhưng `ProductName` chỉ cần `ProductId` → **vi phạm 2NF**.
- **Phụ thuộc bắc cầu (transitive):** A → B và B → C ⇒ A → C (với B không phải khóa). `OrderId → CustomerId → CustomerName` → **vi phạm 3NF**.

## 5. Các dạng chuẩn (Normal Forms)

> Mỗi dạng chuẩn sau phải **thỏa dạng chuẩn trước** + thêm điều kiện mới.

### 1NF — Dạng chuẩn 1

**Điều kiện:**
1. Mỗi ô chỉ chứa **1 giá trị nguyên tố** (atomic) — không chứa danh sách, không tách được nữa.
2. Không có **nhóm cột lặp lại** (`Phone1, Phone2, Phone3`).
3. Mỗi bảng có **khóa chính**.

**Vi phạm:**

| OrderId | CustomerName | Products |
|---|---|---|
| 1 | An | Bút x2, Vở x3 |

**Sửa → 1NF:** mỗi sản phẩm 1 dòng, khóa chính là `(OrderId, ProductId)`.

| **OrderId** | **ProductId** | OrderDate | CustomerId | CustomerName | ProductName | Qty |
|---|---|---|---|---|---|---|
| 1 | P1 | 10/01 | C1 | An | Bút | 2 |
| 1 | P2 | 10/01 | C1 | An | Vở | 3 |

### 2NF — Dạng chuẩn 2

**Điều kiện:** đạt 1NF + **mọi cột không phải khóa phải phụ thuộc vào TOÀN BỘ khóa chính** (không có phụ thuộc bộ phận).

> Chỉ cần xét khi khóa chính là **khóa ghép**. Bảng có PK 1 cột đạt 1NF thì tự động đạt 2NF.

**Vi phạm** (bảng 1NF ở trên, khóa `(OrderId, ProductId)`):
- `OrderDate, CustomerId, CustomerName` chỉ phụ thuộc `OrderId`.
- `ProductName` chỉ phụ thuộc `ProductId`.
- Chỉ `Qty` phụ thuộc đầy đủ vào `(OrderId, ProductId)`.

**Sửa → 2NF:** tách theo "cột phụ thuộc vào phần nào của khóa".

- `Orders(`**`OrderId`**`, OrderDate, CustomerId, CustomerName)`
- `Products(`**`ProductId`**`, ProductName)`
- `OrderDetails(`**`OrderId, ProductId`**`, Qty)`

### 3NF — Dạng chuẩn 3

**Điều kiện:** đạt 2NF + **không có phụ thuộc bắc cầu** — cột không khóa **chỉ phụ thuộc vào khóa**, không phụ thuộc vào cột không khóa khác.

> Câu thần chú: *"Mọi cột phụ thuộc vào **khóa**, **toàn bộ khóa**, và **không gì khác ngoài khóa**."* (key – whole key – nothing but the key)

**Vi phạm:** trong `Orders`: `OrderId → CustomerId → CustomerName`. `CustomerName` phụ thuộc vào `CustomerId` (không phải khóa).

**Sửa → 3NF:**
- `Customers(`**`CustomerId`**`, CustomerName)`
- `Orders(`**`OrderId`**`, OrderDate, CustomerId)` — `CustomerId` là FK
- `Products(`**`ProductId`**`, ProductName)`
- `OrderDetails(`**`OrderId, ProductId`**`, Qty)`

Kết quả: sửa tên khách chỉ sửa **1 dòng**, thêm sản phẩm không cần đơn hàng, xóa đơn không mất thông tin sản phẩm. ✅

> **Lưu ý thực tế:** lưu `UnitPrice` trong `OrderDetails` **không phải** vi phạm — đó là **giá tại thời điểm mua**, khác với giá hiện tại trong `Products` (giá có thể thay đổi sau này).

### BCNF — Boyce-Codd Normal Form (3.5NF)

**Điều kiện:** với **mọi** phụ thuộc hàm `X → Y`, **X phải là super key**. (Chặt hơn 3NF: 3NF vẫn cho phép Y là một phần của khóa.)

**Ví dụ:** `Teaching(Student, Subject, Teacher)`. Quy tắc: mỗi giáo viên chỉ dạy 1 môn; mỗi SV học 1 môn với 1 giáo viên. Khóa: `(Student, Subject)`.
- Có `Teacher → Subject`, nhưng `Teacher` **không phải** super key → vi phạm BCNF (vẫn đạt 3NF vì `Subject` là một phần của khóa).
- Hậu quả: giáo viên đổi môn phải sửa nhiều dòng.

**Sửa:** `TeacherSubjects(`**`Teacher`**`, Subject)` và `StudentTeachers(`**`Student, Teacher`**`)`.

### 4NF và 5NF (biết khái niệm là đủ)

- **4NF:** đạt BCNF + **không có phụ thuộc đa trị** độc lập trong cùng bảng.
  VD: `(Employee, Skill, Language)` — kỹ năng và ngoại ngữ **không liên quan nhau** → nhân viên có 3 skill, 2 ngôn ngữ phải lưu 3×2 = 6 dòng. Tách thành `EmployeeSkills(Employee, Skill)` và `EmployeeLanguages(Employee, Language)`.
- **5NF:** đạt 4NF + không thể tách bảng nhỏ hơn nữa mà không mất thông tin (join dependency). Rất hiếm gặp.

**Thực tế:** hầu hết hệ thống chỉ cần đạt **3NF** (hoặc BCNF).

### Bảng tóm tắt các NF

| NF | Điều kiện thêm | Loại bỏ |
|---|---|---|
| **1NF** | Giá trị nguyên tố, không nhóm lặp, có PK | Ô chứa nhiều giá trị |
| **2NF** | 1NF + phụ thuộc **đầy đủ** vào khóa | Phụ thuộc **bộ phận** (vào 1 phần khóa ghép) |
| **3NF** | 2NF + không phụ thuộc **bắc cầu** | Cột không khóa phụ thuộc cột không khóa |
| **BCNF** | Mọi `X → Y` thì X là super key | Phụ thuộc vào cột không phải super key |
| **4NF** | BCNF + không phụ thuộc đa trị | Nhiều danh sách độc lập trong 1 bảng |
| **5NF** | 4NF + không còn join dependency | — |

## 6. Phi chuẩn hóa (Denormalization)

**Denormalization** = **cố ý** thêm dư thừa dữ liệu (gộp bảng, lưu cột tính sẵn) để **đọc nhanh hơn**, giảm JOIN.

| Chuẩn hóa | Phi chuẩn hóa |
|---|---|
| Ít dư thừa, dữ liệu nhất quán | Có dư thừa, phải tự đảm bảo đồng bộ |
| Ghi (INSERT/UPDATE) nhanh, đơn giản | Đọc (SELECT) nhanh, ít JOIN |
| Hợp với hệ thống giao dịch (**OLTP**): bán hàng, ngân hàng | Hợp với báo cáo, thống kê (**OLAP**), data warehouse |

Ví dụ: lưu sẵn cột `Orders.TotalAmount` thay vì mỗi lần phải `SUM` từ `OrderDetails`.

Nguyên tắc: **chuẩn hóa trước (đến 3NF), chỉ phi chuẩn hóa khi có vấn đề hiệu năng thật sự**.

## 7. Index (Chỉ mục)

**Index** giống **mục lục của cuốn sách**: giúp tìm dòng nhanh mà không phải quét toàn bộ bảng (full table scan). Thường cài đặt bằng cấu trúc **B-Tree**.

- **Clustered index:** sắp xếp **chính dữ liệu vật lý** của bảng theo khóa → mỗi bảng chỉ có **1** (thường là PK).
- **Non-clustered index:** cấu trúc riêng, trỏ về dòng dữ liệu → một bảng có **nhiều**.
- **Unique index:** đảm bảo không trùng giá trị.
- **Composite index:** index trên nhiều cột; thứ tự cột quan trọng (index `(LastName, FirstName)` hỗ trợ tìm theo `LastName`, không hỗ trợ tìm riêng `FirstName`).

```sql
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE UNIQUE INDEX IX_Customers_Email ON Customers(Email);
```

**Ưu:** tăng tốc `SELECT`, `WHERE`, `JOIN`, `ORDER BY`.
**Nhược:** tốn dung lượng; làm **chậm INSERT/UPDATE/DELETE** (phải cập nhật index).
**Nên đánh index:** cột FK, cột hay dùng trong `WHERE/JOIN/ORDER BY`. **Không nên:** bảng rất nhỏ, cột ít giá trị khác nhau (VD: giới tính), cột bị sửa liên tục.

## 8. Transaction & ACID

**Transaction** = nhóm thao tác được coi là **một khối**: hoặc **thành công hết**, hoặc **không có gì xảy ra**.

VD chuyển tiền: trừ tài khoản A **và** cộng tài khoản B — không được chỉ làm 1 trong 2.

```sql
BEGIN TRANSACTION;
    UPDATE Accounts SET Balance = Balance - 100 WHERE Id = 1;
    UPDATE Accounts SET Balance = Balance + 100 WHERE Id = 2;
COMMIT;      -- lỗi giữa chừng thì ROLLBACK
```

| ACID | Ý nghĩa |
|---|---|
| **A**tomicity (Nguyên tử) | Tất cả hoặc không gì cả |
| **C**onsistency (Nhất quán) | Trước và sau transaction, dữ liệu luôn hợp lệ (đúng ràng buộc) |
| **I**solation (Cô lập) | Các transaction chạy đồng thời không ảnh hưởng lẫn nhau |
| **D**urability (Bền vững) | Đã commit thì dữ liệu được lưu vĩnh viễn, kể cả khi mất điện |

**Các lỗi khi chạy đồng thời & mức cô lập (Isolation Level):**
- **Dirty read:** đọc dữ liệu transaction khác **chưa commit**.
- **Non-repeatable read:** đọc 1 dòng 2 lần trong cùng transaction, ra 2 kết quả khác (bị người khác sửa).
- **Phantom read:** chạy lại cùng query thì **xuất hiện thêm/mất dòng** (bị người khác thêm/xóa).

| Isolation Level | Dirty read | Non-repeatable | Phantom |
|---|---|---|---|
| Read Uncommitted | Có thể | Có thể | Có thể |
| Read Committed *(mặc định SQL Server)* | ✅ Chặn | Có thể | Có thể |
| Repeatable Read | ✅ | ✅ | Có thể |
| Serializable | ✅ | ✅ | ✅ |

Mức càng cao → càng an toàn nhưng **càng chậm** (khóa nhiều hơn).

## 9. Quy trình thiết kế DB

1. **Thu thập yêu cầu:** hệ thống lưu gì, ai dùng, nghiệp vụ ra sao.
2. **Xác định thực thể** (danh từ trong yêu cầu: Khách hàng, Đơn hàng, Sản phẩm) và **thuộc tính**.
3. **Xác định quan hệ** (1-1, 1-n, n-n) → vẽ **ERD**.
4. **Chọn khóa chính**, khóa ngoại, bảng trung gian cho n-n.
5. **Chuẩn hóa** đến 3NF.
6. **Chọn kiểu dữ liệu** phù hợp, thêm **ràng buộc** (NOT NULL, UNIQUE, CHECK, FK).
7. **Thêm index** cho cột hay truy vấn.
8. Cân nhắc **phi chuẩn hóa** nếu cần hiệu năng.

**Mẹo chọn kiểu dữ liệu & quy ước:**
- Tiền → `DECIMAL(18,2)`, **không dùng** `FLOAT` (sai số làm tròn).
- Tiếng Việt có dấu → `NVARCHAR`. Đặt độ dài hợp lý, tránh `NVARCHAR(MAX)` bừa bãi.
- Ngày giờ → `DATE`, `DATETIME2`; lưu **UTC** nếu có nhiều múi giờ.
- Đặt tên nhất quán: bảng số nhiều (`Customers`), PK `Id` hoặc `CustomerId`, FK `CustomerId`, không dấu, không khoảng trắng.
- Thêm cột audit: `CreatedAt`, `UpdatedAt`, `CreatedBy`. Cân nhắc **soft delete** (`IsDeleted`) thay vì xóa thật.

## 10. Ví dụ tổng hợp: Hệ thống quản lý thư viện

**Yêu cầu:** Thư viện có nhiều sách, mỗi sách thuộc 1 thể loại, có thể có nhiều tác giả. Độc giả mượn nhiều sách, mỗi lần mượn ghi ngày mượn, ngày trả.

```
Categories(CategoryId PK, Name)
Books(BookId PK, Title, PublishYear, CategoryId FK)          -- 1 thể loại - n sách
Authors(AuthorId PK, FullName)
BookAuthors(BookId FK, AuthorId FK, PK(BookId, AuthorId))    -- n - n sách - tác giả
Readers(ReaderId PK, FullName, Email UNIQUE, Phone)
Loans(LoanId PK, ReaderId FK, BookId FK, BorrowDate, DueDate, ReturnDate NULL)
```

Kiểm tra: mỗi ô 1 giá trị (1NF); các bảng có khóa ghép chỉ có cột khóa (2NF); không cột nào phụ thuộc cột không khóa (3NF) — tên thể loại nằm ở `Categories`, không lặp trong `Books`.

## 11. Câu hỏi hay gặp

**H: Chuẩn hóa là gì? Tại sao cần?**
Đ: Tách bảng để giảm dư thừa và loại bỏ dị thường khi thêm/sửa/xóa, giúp dữ liệu nhất quán.

**H: Phân biệt 1NF, 2NF, 3NF?**
Đ: 1NF — mỗi ô 1 giá trị, không nhóm lặp, có PK. 2NF — 1NF + không phụ thuộc bộ phận vào khóa ghép. 3NF — 2NF + không phụ thuộc bắc cầu.

**H: 3NF khác BCNF?**
Đ: BCNF chặt hơn: mọi phụ thuộc `X → Y` đều phải có X là super key. 3NF vẫn chấp nhận trường hợp Y là một phần của khóa.

**H: PK khác UNIQUE?**
Đ: PK không được NULL và mỗi bảng chỉ có 1 PK. UNIQUE cho phép NULL (tùy DB) và một bảng có nhiều cột UNIQUE.

**H: Khi nào phi chuẩn hóa?**
Đ: Khi hệ thống đọc nhiều, JOIN quá nặng gây chậm (báo cáo, thống kê). Đổi lại phải chấp nhận dư thừa và tự đồng bộ dữ liệu.

**H: Index là gì? Có nên đánh index mọi cột?**
Đ: Cấu trúc giúp tìm kiếm nhanh như mục lục sách. Không nên vì index tốn dung lượng và làm chậm thao tác ghi.

**H: ACID là gì?**
Đ: 4 tính chất của transaction: Atomicity, Consistency, Isolation, Durability.

**H: Thiết kế quan hệ n-n như thế nào?**
Đ: Tạo bảng trung gian chứa 2 FK trỏ về 2 bảng, khóa chính là cặp 2 FK đó.

## 12. Tóm tắt học thuộc

- Khóa: **Super ⊃ Candidate ⊃ Primary**; **FK** tạo quan hệ; **Composite** = nhiều cột; ưu tiên **surrogate key**.
- Quan hệ: **1-1** (FK + UNIQUE), **1-n** (FK ở bên nhiều), **n-n** (**bảng trung gian**).
- Không chuẩn hóa → **dư thừa + dị thường Insert / Update / Delete**.
- **1NF:** giá trị nguyên tố, không nhóm lặp, có PK.
- **2NF:** 1NF + không **phụ thuộc bộ phận** vào khóa ghép.
- **3NF:** 2NF + không **phụ thuộc bắc cầu**. → *"The key, the whole key, and nothing but the key."*
- **BCNF:** mọi `X → Y` thì X là **super key**. **4NF:** không phụ thuộc đa trị.
- Thực tế đạt **3NF** là đủ; **denormalize** có chủ đích để đọc nhanh.
- **Index** = mục lục: đọc nhanh, ghi chậm. **Clustered** (1/bảng) vs **Non-clustered** (nhiều).
- Transaction **ACID**: Atomicity – Consistency – Isolation – Durability.
