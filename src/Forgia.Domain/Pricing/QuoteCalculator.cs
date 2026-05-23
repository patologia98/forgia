using Forgia.Domain.Entities;

namespace Forgia.Domain.Pricing;

public static class QuoteCalculator
{
    public static PlateResult CalculatePlate(
        Plate plate,
        Printer printer,
        FilamentSpool spool,
        decimal electricityRatePerKwh)
    {
        var printTimeH = (decimal)plate.PrintTime.TotalHours;
        var materialCost = plate.FilamentUsageG * spool.CostPerKg / 1000m;
        var electricityCost = printer.PowerW * printTimeH * electricityRatePerKwh / 1000m;
        var amortization = printer.PurchaseCost * (printTimeH / printer.UsefulLifeH);
        var maintenance = printer.MaintenanceCostPerH * printTimeH;
        var wasteOverhead = (materialCost + electricityCost) * plate.WasteRate;
        var plateCost = materialCost + electricityCost + amortization + maintenance + wasteOverhead;

        return new PlateResult(materialCost, electricityCost, amortization, maintenance, wasteOverhead, plateCost);
    }

    public static OrderResult CalculateOrder(
        IReadOnlyList<PlateResult> plateResults,
        IReadOnlyList<LaborActivity> laborActivities,
        decimal marginRate,
        decimal vatRate)
    {
        var platesCost = plateResults.Sum(p => p.PlateCost);
        var laborCost = laborActivities.Sum(l => l.Minutes * l.HourlyRate / 60m);
        var directCost = platesCost + laborCost;
        var quoteTotal = directCost * (1m + marginRate) * (1m + vatRate);

        return new OrderResult(platesCost, laborCost, directCost, quoteTotal);
    }
}
