using Exercises.Builder;
using Xunit;

namespace Exercises.Tests;

public class BuilderExerciseTests
{
    [Fact]
    public void Build_WithAllFields_ReturnsConfiguredRequest()
    {
        HttpRequest request = new HttpRequestBuilder()
            .WithUrl("https://api.example.com/products")
            .WithMethod("POST")
            .AddHeader("Authorization", "Bearer token123")
            .WithBody("{\"name\":\"Product A\"}")
            .Build();

        Assert.Equal("https://api.example.com/products", request.Url);
        Assert.Equal("POST", request.Method);
        Assert.Equal("Bearer token123", request.Headers["Authorization"]);
        Assert.Equal("{\"name\":\"Product A\"}", request.Body);
    }

    [Fact]
    public void Build_DefaultMethod_IsGet()
    {
        HttpRequest request = new HttpRequestBuilder()
            .WithUrl("https://api.example.com")
            .Build();

        Assert.Equal("GET", request.Method);
    }

    [Fact]
    public void Build_WithoutUrl_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => new HttpRequestBuilder().Build());
    }
}
