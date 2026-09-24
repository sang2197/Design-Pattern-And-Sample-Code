# API & RESTful API

## 1. API là gì?

**API (Application Programming Interface)** là **"cửa giao tiếp"** giữa các phần mềm — quy định phần mềm A có thể **gọi gì**, **gửi gì**, **nhận lại gì** từ phần mềm B, mà không cần biết bên trong B làm thế nào.

Ví dụ đời thường: ở nhà hàng, **bạn (client)** không vào bếp, bạn gọi món qua **người phục vụ (API)**, **bếp (server)** nấu rồi phục vụ mang ra.

Trong web, API thường là **Web API**: client (web, mobile app) gửi **HTTP request** tới server, server trả **HTTP response** (thường là JSON).

```
Client (React / Mobile)  --- HTTP request: GET /api/products/5 --->  Server (ASP.NET Core)
                         <-- HTTP response: 200 OK + JSON ---------
```

## 2. Các kiểu API phổ biến

| Kiểu | Đặc điểm | Dùng khi |
|---|---|---|
| **REST** | Dựa trên HTTP, tài nguyên qua URL, thường trả JSON | Phổ biến nhất, web/mobile |
| **SOAP** | Dùng XML, có chuẩn chặt (WSDL), nặng | Hệ thống cũ, ngân hàng, doanh nghiệp |
| **GraphQL** | 1 endpoint, client **tự chọn trường** muốn lấy | Frontend cần dữ liệu linh hoạt, tránh lấy thừa/thiếu |
| **gRPC** | Nhị phân (Protobuf), HTTP/2, rất nhanh | Giao tiếp giữa các microservice |
| **WebSocket** | Kết nối 2 chiều, giữ liên tục | Realtime: chat, thông báo, game |

## 3. REST là gì?

**REST (REpresentational State Transfer)** là một **phong cách kiến trúc** để thiết kế API. API tuân theo REST gọi là **RESTful API**. Mọi thứ đều là **tài nguyên (resource)**, được xác định bằng **URL**, và thao tác bằng **HTTP method**.

### 6 ràng buộc của REST

1. **Client – Server:** tách biệt giao diện và xử lý dữ liệu, phát triển độc lập.
2. **Stateless (không lưu trạng thái):** mỗi request phải **tự chứa đủ thông tin** (VD: kèm token). Server **không nhớ** request trước.
3. **Cacheable:** response cho biết có được cache hay không → giảm tải.
4. **Uniform Interface (giao diện thống nhất):** dùng URL + HTTP method theo chuẩn chung.
5. **Layered System:** client không cần biết đang nói chuyện với server thật hay qua load balancer, proxy, gateway.
6. **Code on Demand** *(tùy chọn)*: server có thể gửi code (JS) cho client chạy.

## 4. HTTP Methods

| Method | Mục đích | Ví dụ | Safe | Idempotent |
|---|---|---|---|---|
| **GET** | Lấy dữ liệu | `GET /products/5` | ✅ | ✅ |
| **POST** | Tạo mới | `POST /products` | ❌ | ❌ |
| **PUT** | Cập nhật **toàn bộ** (thay thế) | `PUT /products/5` | ❌ | ✅ |
| **PATCH** | Cập nhật **một phần** | `PATCH /products/5` | ❌ | ❌ (thường) |
| **DELETE** | Xóa | `DELETE /products/5` | ❌ | ✅ |

- **Safe:** không làm thay đổi dữ liệu trên server.
- **Idempotent:** gọi **1 lần hay 10 lần kết quả trên server như nhau**. VD: `DELETE /products/5` gọi nhiều lần thì sản phẩm 5 vẫn chỉ bị xóa. Còn `POST` gọi 10 lần có thể tạo 10 bản ghi.

**PUT vs PATCH:** PUT gửi **cả object** (thiếu trường nào coi như bỏ trống); PATCH chỉ gửi **trường cần sửa**.

## 5. HTTP Status Code

| Nhóm | Ý nghĩa | Hay gặp |
|---|---|---|
| **1xx** | Thông tin | 101 Switching Protocols |
| **2xx** | Thành công | **200** OK, **201** Created, **204** No Content |
| **3xx** | Chuyển hướng | 301 Moved Permanently, 304 Not Modified |
| **4xx** | **Lỗi phía client** | **400** Bad Request, **401** Unauthorized, **403** Forbidden, **404** Not Found, 405 Method Not Allowed, **409** Conflict, 422 Unprocessable Entity, **429** Too Many Requests |
| **5xx** | **Lỗi phía server** | **500** Internal Server Error, 502 Bad Gateway, **503** Service Unavailable |

Hay nhầm:
- **401 Unauthorized:** **chưa đăng nhập** / token sai → "Bạn là ai?"
- **403 Forbidden:** đã biết bạn là ai nhưng **không có quyền** → "Bạn không được vào đây."
- **400:** dữ liệu gửi lên sai định dạng / thiếu. **404:** không tìm thấy tài nguyên.
- **201 Created:** dùng sau POST tạo thành công, kèm header `Location` trỏ tới tài nguyên mới.
- **204 No Content:** thành công nhưng không trả body (hay dùng cho DELETE, PUT).

## 6. Thiết kế URL (Endpoint) chuẩn REST

Quy tắc:
- Dùng **danh từ số nhiều**, không dùng động từ (method đã nói hành động rồi).
- Chữ thường, nối bằng dấu gạch ngang `-`.
- Quan hệ cha–con thể hiện bằng URL lồng nhau (không quá 2 cấp).
- Lọc, sắp xếp, phân trang dùng **query string**.

```
❌ GET  /getAllProducts          ✅ GET    /api/products
❌ POST /createProduct           ✅ POST   /api/products
❌ POST /deleteProduct?id=5      ✅ DELETE /api/products/5
❌ GET  /api/Product_List        ✅ GET    /api/products/5
                                 ✅ GET    /api/users/10/orders          (đơn hàng của user 10)
                                 ✅ GET    /api/products?category=phone&sort=-price&page=2&pageSize=20
```

## 7. Cấu trúc HTTP Request / Response

```http
POST /api/products HTTP/1.1                ← Method + URL
Host: shop.com
Content-Type: application/json             ← Headers
Authorization: Bearer eyJhbGciOi...

{ "name": "iPhone", "price": 20000000 }    ← Body
```

```http
HTTP/1.1 201 Created                       ← Status code
Content-Type: application/json
Location: /api/products/15

{ "id": 15, "name": "iPhone", "price": 20000000 }
```

Header hay gặp: `Content-Type`, `Accept`, `Authorization`, `Cache-Control`, `Location`.

Cách truyền dữ liệu lên server:
- **Route/Path param:** `/products/5` → xác định tài nguyên.
- **Query string:** `?page=2&sort=name` → lọc, sắp xếp, phân trang.
- **Body:** dữ liệu tạo/sửa (POST, PUT, PATCH).
- **Header:** token, định dạng dữ liệu.

## 8. Ví dụ API với ASP.NET Core

```csharp
// DTO (Data Transfer Object): object chỉ dùng để nhận/trả dữ liệu qua API,
// tách khỏi entity DB => không lộ trường nhạy cảm, API không đổi khi DB đổi.
public record ProductDto(int Id, string Name, decimal Price);
public record CreateProductRequest([Required] string Name, [Range(0, double.MaxValue)] decimal Price);

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;

    [HttpGet]                                   // GET /api/products?page=1&pageSize=20
    public async Task<ActionResult<List<ProductDto>>> GetAll(int page = 1, int pageSize = 20)
        => Ok(await _service.GetPagedAsync(page, pageSize));

    [HttpGet("{id}")]                           // GET /api/products/5
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);          // 404 / 200
    }

    [HttpPost]                                  // POST /api/products
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest req)
    {
        var created = await _service.CreateAsync(req);              // [ApiController] tự trả 400 nếu req không hợp lệ
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);   // 201
    }

    [HttpPut("{id}")]                           // PUT /api/products/5
    public async Task<IActionResult> Update(int id, CreateProductRequest req)
    {
        var ok = await _service.UpdateAsync(id, req);
        return ok ? NoContent() : NotFound();                       // 204 / 404
    }

    [HttpDelete("{id}")]                        // DELETE /api/products/5
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
```

Kiến trúc thường gặp: **Controller** (nhận request, trả response) → **Service** (logic nghiệp vụ) → **Repository** (truy cập DB). Controller nên **mỏng**, không chứa logic nghiệp vụ.

## 9. Xác thực & phân quyền (Authentication & Authorization)

- **Authentication (xác thực):** *Bạn là ai?* → đăng nhập, kiểm tra token.
- **Authorization (phân quyền):** *Bạn được làm gì?* → admin mới được xóa.

| Cách | Mô tả |
|---|---|
| **API Key** | Gửi 1 khóa cố định trong header. Đơn giản, dùng cho server-to-server |
| **Basic Auth** | Gửi `username:password` mã hóa Base64 — chỉ an toàn khi dùng HTTPS |
| **JWT (JSON Web Token)** | Đăng nhập xong server cấp token; mỗi request gửi `Authorization: Bearer <token>`. Server không cần lưu session → hợp với **stateless** |
| **OAuth 2.0** | Chuẩn **ủy quyền**: "Đăng nhập bằng Google" — app được cấp quyền truy cập mà không biết mật khẩu của bạn |

**JWT gồm 3 phần:** `Header.Payload.Signature`
- Header: thuật toán ký. Payload: thông tin (userId, role, hạn dùng `exp`). Signature: chữ ký để chống sửa.
- Payload chỉ **mã hóa Base64, không phải mã hóa bí mật** → **không để mật khẩu** trong token.
- Thường dùng **access token** (ngắn hạn, vài phút) + **refresh token** (dài hạn, để xin access token mới).

```csharp
[Authorize]                          // phải đăng nhập (không có => 401)
[Authorize(Roles = "Admin")]         // phải là Admin (không đủ quyền => 403)
[HttpDelete("{id}")]
public IActionResult Delete(int id) { ... }
```

## 10. Các vấn đề khác khi thiết kế API

**Versioning (đánh phiên bản):** khi thay đổi làm hỏng client cũ (breaking change), tạo phiên bản mới.
- URL: `/api/v1/products`, `/api/v2/products` (phổ biến nhất)
- Query: `/api/products?api-version=2`
- Header: `Api-Version: 2`

**Pagination (phân trang):** không bao giờ trả toàn bộ bảng.
```json
{ "items": [ ... ], "page": 2, "pageSize": 20, "totalCount": 135 }
```

**Format lỗi thống nhất** (chuẩn **ProblemDetails** — RFC 7807):
```json
{ "type": "...", "title": "Validation failed", "status": 400,
  "errors": { "Price": ["Price must be greater than 0"] } }
```

**Khác:**
- **HTTPS** bắt buộc — tránh lộ token, dữ liệu.
- **Validation** mọi input từ client — không bao giờ tin client.
- **CORS:** trình duyệt chặn web ở domain A gọi API ở domain B, trừ khi server cho phép → cấu hình CORS trên server.
- **Rate limiting:** giới hạn số request / phút để chống spam, DDoS → trả 429.
- **Caching:** header `Cache-Control`, `ETag` → giảm tải server.
- **Tài liệu API:** **Swagger / OpenAPI** — tự sinh trang tài liệu và cho test API trực tiếp.
- **Công cụ test API:** Postman, Swagger UI, curl.

## 11. REST vs SOAP vs GraphQL

| | REST | SOAP | GraphQL |
|---|---|---|---|
| Định dạng | JSON (chủ yếu) | XML | JSON |
| Endpoint | Nhiều (mỗi resource 1 URL) | 1 | 1 |
| Lấy dữ liệu | Server quyết định trả gì → có thể thừa/thiếu | Theo hợp đồng WSDL | **Client chọn** trường cần |
| Độ nặng | Nhẹ | Nặng | Nhẹ, nhưng server phức tạp hơn |
| Cache | Dễ (HTTP cache) | Khó | Khó hơn |

## 12. Câu hỏi hay gặp

**H: API là gì? REST là gì?**
Đ: API là giao diện để các phần mềm giao tiếp với nhau. REST là phong cách thiết kế API dựa trên HTTP, coi mọi thứ là tài nguyên (URL) và thao tác bằng HTTP method.

**H: Stateless nghĩa là gì?**
Đ: Server không lưu trạng thái của client giữa các request; mỗi request tự mang đủ thông tin (VD: token).

**H: PUT khác PATCH? PUT khác POST?**
Đ: PUT thay thế toàn bộ, PATCH sửa một phần. POST tạo mới (không idempotent), PUT cập nhật/thay thế (idempotent).

**H: Idempotent là gì? Method nào idempotent?**
Đ: Gọi nhiều lần cho kết quả như gọi 1 lần. GET, PUT, DELETE (và HEAD, OPTIONS) là idempotent; POST thì không.

**H: 401 khác 403?**
Đ: 401 — chưa xác thực (chưa đăng nhập / token sai). 403 — đã xác thực nhưng không có quyền.

**H: JWT là gì?**
Đ: Token dạng `header.payload.signature`, server cấp sau khi đăng nhập, client gửi kèm mỗi request trong header `Authorization: Bearer`. Server kiểm tra chữ ký, không cần lưu session.

**H: Tại sao dùng DTO thay vì trả entity trực tiếp?**
Đ: Không lộ dữ liệu nhạy cảm (password hash), tránh vòng lặp khi serialize, API ổn định khi DB thay đổi, chỉ trả đúng dữ liệu cần.

## 13. Tóm tắt học thuộc

- API = **cửa giao tiếp** giữa các phần mềm. REST = phong cách thiết kế API trên **HTTP**, **resource + URL + method**.
- 6 ràng buộc REST: **Client-Server, Stateless, Cacheable, Uniform Interface, Layered, Code on Demand**.
- **GET** lấy – **POST** tạo – **PUT** thay toàn bộ – **PATCH** sửa một phần – **DELETE** xóa.
- **Idempotent:** GET, PUT, DELETE. **Không:** POST.
- Status: **2xx** OK, **3xx** chuyển hướng, **4xx** lỗi client, **5xx** lỗi server. **200, 201, 204, 400, 401, 403, 404, 409, 500**.
- URL: **danh từ số nhiều**, không động từ; lọc/phân trang bằng **query string**.
- Bảo mật: **HTTPS, JWT/OAuth2, validate input, CORS, rate limit**.
- Thiết kế: **DTO, versioning, pagination, format lỗi thống nhất, Swagger**.
