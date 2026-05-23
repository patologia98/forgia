using Forgia.Domain.Entities;
using Forgia.Domain.Pricing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Forgia.Infrastructure.Pdf;

public static class QuoteExporter
{
    public static void ExportToFile(
        Order order,
        IReadOnlyList<(Plate Plate, Printer Printer, FilamentSpool Spool, PlateResult Result)> plates,
        OrderResult orderResult,
        decimal electricityRatePerKwh,
        string outputPath)
    {
        var doc = new QuoteDocument(order, plates, orderResult, electricityRatePerKwh);
        doc.GeneratePdf(outputPath);
    }

    public static byte[] ExportToBytes(
        Order order,
        IReadOnlyList<(Plate Plate, Printer Printer, FilamentSpool Spool, PlateResult Result)> plates,
        OrderResult orderResult,
        decimal electricityRatePerKwh)
    {
        var doc = new QuoteDocument(order, plates, orderResult, electricityRatePerKwh);
        return doc.GeneratePdf();
    }
}
