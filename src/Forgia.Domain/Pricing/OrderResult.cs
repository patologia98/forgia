namespace Forgia.Domain.Pricing;

public record OrderResult(
    decimal PlatesCost,
    decimal LaborCost,
    decimal DirectCost,
    decimal QuoteTotal);
