namespace Exercises.Builder;

public class HttpRequest
{
    public string Url { get; set; } = "";
    public string Method { get; set; } = "GET";
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    public string Body { get; set; } = "";
}

public class HttpRequestBuilder
{
    private readonly HttpRequest _request = new HttpRequest();

    // TODO: gan _request.Url = url, tra ve "this" de chain tiep duoc
    public HttpRequestBuilder WithUrl(string url)
    {
        throw new NotImplementedException();
    }

    // TODO: gan _request.Method = method, tra ve "this"
    public HttpRequestBuilder WithMethod(string method)
    {
        throw new NotImplementedException();
    }

    // TODO: gan _request.Headers[key] = value, tra ve "this"
    public HttpRequestBuilder AddHeader(string key, string value)
    {
        throw new NotImplementedException();
    }

    // TODO: gan _request.Body = body, tra ve "this"
    public HttpRequestBuilder WithBody(string body)
    {
        throw new NotImplementedException();
    }

    // TODO: neu _request.Url rong -> throw InvalidOperationException("Url is required")
    // nguoc lai tra ve _request
    public HttpRequest Build()
    {
        throw new NotImplementedException();
    }
}
