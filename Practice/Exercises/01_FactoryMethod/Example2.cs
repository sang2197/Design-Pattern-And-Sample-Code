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

// Product
public interface IReportExporter
{
    string Export(string content);
}

// Concrete Product
public class PdfReportExporter : IReportExporter
{
    public string Export(string content) => $"[PDF] {content}";
}

public class ExcelReportExporter : IReportExporter
{
    public string Export(string content) => $"[Excel] {content}";
}

// Creator
public abstract class ReportProcessor
{
    protected abstract IReportExporter CreateProcessor();

    public string GenerateReport(string content)
    {
        var processor = CreateProcessor();
        return processor.Export(content);
    }
}

// Concrete Creator
public class PdfReportProcessor : ReportProcessor
{
    protected override IReportExporter CreateProcessor() => new PdfReportExporter();
}

public class ExcelReportProcessor : ReportProcessor
{
    protected override IReportExporter CreateProcessor() => new ExcelReportExporter();
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var pdfReport = new PdfReportProcessor();
        var excelReport = new ExcelReportProcessor();

        Console.WriteLine(pdfReport.GenerateReport("Da hoan thanh bao cao"));
        Console.WriteLine(excelReport.GenerateReport("Da hoan thanh bao cao"));
    }
}