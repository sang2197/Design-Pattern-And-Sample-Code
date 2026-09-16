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
