using Exercises.Proxy.Example2;
using Xunit;

namespace Proxy.Tests;

public class Example2Tests
{
    [Fact]
    public void NonAdmin_CannotDeleteDocument()
    {
        var realService = new DocumentService();
        IDocumentService userProxy = new DocumentServiceProxy(realService, "User");

        userProxy.DeleteDocument("DOC001");

        Assert.Empty(realService.DeletedDocuments);
    }

    [Fact]
    public void Admin_CanDeleteDocument()
    {
        var realService = new DocumentService();
        IDocumentService adminProxy = new DocumentServiceProxy(realService, "Admin");

        adminProxy.DeleteDocument("DOC001");

        Assert.Equal("DOC001", realService.DeletedDocuments[0]);
    }
}
