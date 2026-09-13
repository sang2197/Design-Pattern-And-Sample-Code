# Builder Pattern

## 1. Khái niệm

**Builder** là một Creational Design Pattern, dùng để xây dựng một đối tượng phức tạp từng bước, thay vì truyền quá nhiều tham số vào một constructor.

Builder đặc biệt hữu ích khi đối tượng có nhiều thuộc tính, nhiều thuộc tính optional hoặc quá trình khởi tạo cần validation.

Các thành phần chính:

- **Product:** đối tượng phức tạp cần được xây dựng.
- **Builder:** định nghĩa các bước để cấu hình Product.
- **ConcreteBuilder:** thực hiện các bước xây dựng và trả về Product hoàn chỉnh qua `Build()`.
- **Director (tùy chọn):** định nghĩa sẵn thứ tự/cách gọi Builder để tạo một số cấu hình thường dùng. Với Fluent Builder, Client thường gọi Builder trực tiếp nên không nhất thiết cần Director.

## 2. Ý nghĩa

- **Không có Builder:** Khi một object có nhiều thuộc tính, constructor có thể trở nên dài và khó đọc. Các tham số cùng kiểu dữ liệu dễ bị truyền nhầm vị trí.
- **Dùng Builder:** Builder tách quá trình cấu hình object thành từng bước có tên rõ ràng. Lợi ích chính: Code khởi tạo dễ đọc hơn, Dễ xử lý nhiều thuộc tính optional, Có thể kiểm tra tính hợp lệ của Product trong Build(), Có thể xây dựng object qua nhiều bước, Dễ thêm các tùy chọn mới mà không làm constructor ngày càng dài.
- Builder không nhằm thay thế mọi constructor. Với object đơn giản, constructor hoặc Object Initializer của C# thường là đủ. Builder phù hợp khi quá trình khởi tạo object thực sự phức tạp.

## 3. Code mẫu

### Ví dụ 1 — Product và fluent Builder

    public class HttpRequest
    {
        public string Url { get; set; }
        public string Method { get; set; } = "GET";
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public string Body { get; set; }
    }

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

### Ví dụ 2 — Sử dụng tại nơi gọi

    var request = new HttpRequestBuilder()
        .WithUrl("https://api.example.com/products")
        .WithMethod("POST")
        .AddHeader("Authorization", "Bearer token123")
        .AddHeader("Content-Type", "application/json")
        .WithBody("{\"name\":\"Product A\"}")
        .Build();

### Ví dụ 3 — Director dựng sẵn các cấu hình thường dùng

    public class HttpRequestDirector
    {
        public HttpRequest BuildJsonGetRequest(string url)
        {
            return new HttpRequestBuilder()
                .WithUrl(url)
                .WithMethod("GET")
                .AddHeader("Accept", "application/json")
                .Build();
        }

        public HttpRequest BuildAuthorizedPostRequest(string url, string token, string body)
        {
            return new HttpRequestBuilder()
                .WithUrl(url)
                .WithMethod("POST")
                .AddHeader("Authorization", $"Bearer {token}")
                .AddHeader("Content-Type", "application/json")
                .WithBody(body)
                .Build();
        }
    }
