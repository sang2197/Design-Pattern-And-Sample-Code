using Exercises.Adapter.Example2;
using Xunit;

namespace Adapter.Tests;

public class Example2Tests
{
    [Fact]
    public void Log_DelegatesToWriteXmlLog()
    {
        var legacyLogger = new LegacyXmlLogger();
        ILogger logger = new XmlLoggerAdapter(legacyLogger);

        logger.Log("Ung dung da khoi dong");

        Assert.Single(legacyLogger.XmlLogs);
        Assert.Equal("<log>Ung dung da khoi dong</log>", legacyLogger.XmlLogs[0]);
    }
}
