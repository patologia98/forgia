namespace Forgia.Domain.Pricing;

public record PlateResult(
    decimal MaterialCost,
    decimal ElectricityCost,
    decimal Amortization,
    decimal Maintenance,
    decimal WasteOverhead,
    decimal PlateCost);
