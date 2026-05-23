using Forgia.Domain.Entities;
using Forgia.Domain.Pricing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Forgia.Infrastructure.Pdf;

public class QuoteDocument : IDocument
{
    private readonly Order _order;
    private readonly IReadOnlyList<(Plate Plate, Printer Printer, FilamentSpool Spool, PlateResult Result)> _plates;
    private readonly OrderResult _orderResult;
    private readonly decimal _electricityRatePerKwh;

    public QuoteDocument(
        Order order,
        IReadOnlyList<(Plate, Printer, FilamentSpool, PlateResult)> plates,
        OrderResult orderResult,
        decimal electricityRatePerKwh)
    {
        _order = order;
        _plates = plates;
        _orderResult = orderResult;
        _electricityRatePerKwh = electricityRatePerKwh;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(t => t.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }

    private void ComposeHeader(IContainer c)
    {
        c.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("FORGIA — Quote").Bold().FontSize(18);
                col.Item().Text($"Date: {_order.CreatedAt:yyyy-MM-dd}");
                col.Item().Text($"Customer: {_order.Customer.Name}");
            });
        });
    }

    private void ComposeContent(IContainer c)
    {
        c.Column(col =>
        {
            col.Spacing(10);

            col.Item().Element(ComposePlatesSection);
            col.Item().Element(ComposeLaborSection);
            col.Item().Element(ComposeTotalsSection);
        });
    }

    private void ComposePlatesSection(IContainer c)
    {
        c.Column(col =>
        {
            col.Item().Text("Print Plates").Bold().FontSize(12);
            col.Item().LineHorizontal(1);

            foreach (var (plate, printer, spool, result) in _plates)
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);
                        cols.RelativeColumn(1);
                    });

                    void Row(string label, string value)
                    {
                        table.Cell().Text(label);
                        table.Cell().AlignRight().Text(value);
                    }

                    table.Header(h =>
                    {
                        h.Cell().ColumnSpan(2).Text(
                            $"{printer.Name} — {spool.Name} ({spool.Material}) — {plate.PrintTime:h\\h\\ mm\\m}").SemiBold();
                    });

                    Row("Material cost", $"€ {result.MaterialCost:F4}");
                    Row("Electricity", $"€ {result.ElectricityCost:F4}");
                    Row("Amortization", $"€ {result.Amortization:F4}");
                    Row("Maintenance", $"€ {result.Maintenance:F4}");
                    Row("Waste overhead", $"€ {result.WasteOverhead:F4}");
                    Row("Plate subtotal", $"€ {result.PlateCost:F2}");
                });
            }
        });
    }

    private void ComposeLaborSection(IContainer c)
    {
        var labor = _order.LaborActivities.ToList();
        if (labor.Count == 0) return;

        c.Column(col =>
        {
            col.Item().Text("Labor").Bold().FontSize(12);
            col.Item().LineHorizontal(1);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(1);
                    cols.RelativeColumn(1);
                });

                table.Header(h =>
                {
                    h.Cell().Text("Description").SemiBold();
                    h.Cell().AlignRight().Text("Min").SemiBold();
                    h.Cell().AlignRight().Text("€/h").SemiBold();
                    h.Cell().AlignRight().Text("Total").SemiBold();
                });

                foreach (var l in labor)
                {
                    var lineTotal = l.Minutes * l.HourlyRate / 60m;
                    table.Cell().Text(l.Description);
                    table.Cell().AlignRight().Text($"{l.Minutes:F0}");
                    table.Cell().AlignRight().Text($"€ {l.HourlyRate:F2}");
                    table.Cell().AlignRight().Text($"€ {lineTotal:F2}");
                }
            });
        });
    }

    private void ComposeTotalsSection(IContainer c)
    {
        c.Column(col =>
        {
            col.Item().Text("Summary").Bold().FontSize(12);
            col.Item().LineHorizontal(1);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(1);
                });

                void Row(string label, string value, bool bold = false)
                {
                    var l = table.Cell().Text(label);
                    var r = table.Cell().AlignRight().Text(value);
                    if (bold) { l.Bold(); r.Bold(); }
                }

                Row("Plates cost", $"€ {_orderResult.PlatesCost:F2}");
                Row("Labor cost", $"€ {_orderResult.LaborCost:F2}");
                Row("Direct cost", $"€ {_orderResult.DirectCost:F2}");
                Row($"Margin ({_order.MarginRate * 100:F0}%)",
                    $"€ {_orderResult.DirectCost * _order.MarginRate:F2}");
                Row($"VAT ({_order.VatRate * 100:F0}%)",
                    $"€ {_orderResult.DirectCost * (1 + _order.MarginRate) * _order.VatRate:F2}");
                Row("TOTAL", $"€ {_orderResult.QuoteTotal:F2}", bold: true);
            });
        });
    }
}
