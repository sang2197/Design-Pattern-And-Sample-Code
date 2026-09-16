using Exercises.Proxy.Example3;
using Xunit;

namespace Proxy.Tests;

public class Example3Tests
{
    [Fact]
    public void GetProductById_CachesResultAfterFirstQuery()
    {
        var realRepository = new ProductRepository();
        IProductRepository repository = new CachingProductRepositoryProxy(realRepository);

        Assert.Equal("Product-1", repository.GetProductById(1));
        Assert.Equal("Product-1", repository.GetProductById(1));
        Assert.Equal(1, realRepository.QueryCount);

        Assert.Equal("Product-2", repository.GetProductById(2));
        Assert.Equal(2, realRepository.QueryCount);
    }
}
