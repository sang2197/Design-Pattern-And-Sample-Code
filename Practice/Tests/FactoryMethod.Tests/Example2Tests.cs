using Exercises.FactoryMethod.Example2;
using Xunit;

namespace FactoryMethod.Tests;

public class Example2Tests
{
    [Fact]
    public void PdfReportGenerator_GenerateReport_ReturnsPdfFormat()
    {
        ReportGenerator generator = new PdfReportGenerator();
        Assert.Equal("[PDF] Bao cao doanh thu", generator.GenerateReport("Bao cao doanh thu"));
    }

    [Fact]
    public void ExcelReportGenerator_GenerateReport_ReturnsExcelFormat()
    {
        ReportGenerator generator = new ExcelReportGenerator();
        Assert.Equal("[EXCEL] Bao cao doanh thu", generator.GenerateReport("Bao cao doanh thu"));
    }
}
