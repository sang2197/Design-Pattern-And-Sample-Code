namespace Exercises.Adapter.Example2;

// Adapter - Vi du 2: Ghi log qua thu vien XML cu (legacy)
// Xem lai: Adapter-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface ILogger (Target) { void Log(string message); }
// - class LegacyXmlLogger (Adaptee)
//     List<string> XmlLogs { get; } = new List<string>()
//     void WriteXmlLog(string xmlContent) -> them vao XmlLogs chuoi "<log>{xmlContent}</log>"
// - class XmlLoggerAdapter : ILogger (Adapter)
//     constructor nhan LegacyXmlLogger
//     Log(message) -> goi thang _legacyLogger.WriteXmlLog(message)

// Target
public interface ILogger
{
    void Log(string content);
}

// Adaptee
public class LegacyXmlLogger
{
    public void XmlLog(string xmlContent)
    {
        Console.WriteLine($"[LegacyXml] <log>{xmlContent}</log>");
    }
}

// Adapter
public class XmlLoggerAdapter : ILogger
{
    private readonly LegacyXmlLogger _logger;
    public XmlLoggerAdapter(LegacyXmlLogger logger)
    {
        _logger = logger;
    }

    public void Log(string content)
    {
        _logger.XmlLog(content);
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        ILogger logger = new XmlLoggerAdapter(new LegacyXmlLogger());
        logger.Log("Da khoi dong ung dung");
    }
}