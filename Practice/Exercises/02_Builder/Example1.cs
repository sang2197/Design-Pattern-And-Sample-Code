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
