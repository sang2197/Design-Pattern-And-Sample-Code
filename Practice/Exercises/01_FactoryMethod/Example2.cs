namespace Exercises.FactoryMethod.Example2;

// Factory Method - Vi du 2: Xuat bao cao theo dinh dang file (PDF / Excel)
// Xem lai: Factory-Method-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - interface IReportExporter { string Export(string content); }
// - class PdfReportExporter : IReportExporter -> Export() tra ve "[PDF] {content}"
// - class ExcelReportExporter : IReportExporter -> Export() tra ve "[EXCEL] {content}"
// - abstract class ReportGenerator
//     protected abstract IReportExporter CreateExporter();
//     public string GenerateReport(string content) -> goi CreateExporter() roi tra ve ket qua Export(content)
// - class PdfReportGenerator : ReportGenerator -> CreateExporter() tra ve PdfReportExporter
// - class ExcelReportGenerator : ReportGenerator -> CreateExporter() tra ve ExcelReportExporter
