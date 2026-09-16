using Exercises.Decorator.Example3;
using Xunit;

namespace Decorator.Tests;

public class Example3Tests
{
    [Fact]
    public void CompressThenEncrypt_AppliesBothInWrapOrder()
    {
        IDataSource source = new EncryptionDecorator(new CompressionDecorator(new FileDataSource()));
        Assert.Equal("[encrypted]olleh]desserpmoc[", source.Write("hello"));
    }
}
