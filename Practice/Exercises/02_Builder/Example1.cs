namespace Exercises.Builder.Example1;

// Builder - Vi du 1: Fluent Builder dung HttpRequest
// Xem lai: Builder-Pattern.md - Vi du 1
//
// Viet lai TU DAU cac thanh phan sau:
//
// - class HttpRequest (Product)
//     string Url { get; set; } = ""
//     string Method { get; set; } = "GET"
//     Dictionary<string, string> Headers { get; } = new Dictionary<string, string>()
//     string Body { get; set; } = ""
// - class HttpRequestBuilder
//     WithUrl(string), WithMethod(string), AddHeader(string key, string value), WithBody(string)
//       -> moi method chi gan 1 phan cua request roi return this (method chaining)
//     Build() -> neu Url rong thi throw new InvalidOperationException("Url is required"), nguoc lai tra ve request

// Product
public class HttpRequest
{
    public string Url { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    public string Body { get; set; }
}

// Builder
public class HttpRequestBuilder
{
    private readonly HttpRequest request = new HttpRequest();

    public HttpRequestBuilder WithUrl(string url)
    {
        request.Url = url;
        return this;
    }

    public HttpRequestBuilder WithMethod(string method)
    {
        request.Method = method;
        return this;
    }

    public HttpRequestBuilder AddHeader(string key, string value)
    {
        request.Headers[key] = value;
        return this;
    }

    public HttpRequestBuilder WithBody(string body)
    {
        request.Body = body;
        return this;
    }

    public HttpRequest Build()
    {
        if (string.IsNullOrEmpty(request.Url))
        {
            throw new InvalidOperationException("Url khong duoc no trong");
        }
        return request;
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var request = new HttpRequestBuilder()
            .WithUrl("https://api.example.com")
            .WithMethod("Post")
            .AddHeader("Authorization", "Bear ABC")
            .AddHeader("Contetn-Type", "application/json")
            .WithBody("{\"name\": \"Product A\"}")
            .Build();
    }
}