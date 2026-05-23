using Forgia.Domain.Entities;
using Forgia.Domain.Pricing;
using FluentAssertions;

namespace Forgia.Domain.Tests.Pricing;

public class QuoteCalculatorTests
{
    private static Printer DefaultPrinter() => new()
    {
        PowerW = 500,
        PurchaseCost = 1000m,
        UsefulLifeH = 10000m,
        MaintenanceCostPerH = 0.05m,
    };

    private static FilamentSpool DefaultSpool() => new()
    {
        CostPerKg = 20m,
    };

    private static Plate DefaultPlate(decimal filamentG = 100m, decimal wasteRate = 0.05m) => new()
    {
        PrintTime = TimeSpan.FromHours(2),
        FilamentUsageG = filamentG,
        WasteRate = wasteRate,
    };

    [Fact]
    public void CalculatePlate_StandardInputs_ReturnsCorrectBreakdown()
    {
        var plate = DefaultPlate(filamentG: 100m, wasteRate: 0.05m);
        var printer = DefaultPrinter();
        var spool = DefaultSpool();
        const decimal electricityRate = 0.25m;

        var result = QuoteCalculator.CalculatePlate(plate, printer, spool, electricityRate);

        // material: 100 * 20 / 1000 = 2.00
        result.MaterialCost.Should().Be(2.00m);
        // electricity: 500 * 2 * 0.25 / 1000 = 0.25
        result.ElectricityCost.Should().Be(0.25m);
        // amortization: 1000 * (2 / 10000) = 0.20
        result.Amortization.Should().Be(0.20m);
        // maintenance: 0.05 * 2 = 0.10
        result.Maintenance.Should().Be(0.10m);
        // waste: (2.00 + 0.25) * 0.05 = 0.1125
        result.WasteOverhead.Should().Be(0.1125m);
        // total: 2.00 + 0.25 + 0.20 + 0.10 + 0.1125 = 2.6625
        result.PlateCost.Should().Be(2.6625m);
    }

    [Fact]
    public void CalculatePlate_ZeroWasteRate_WasteOverheadIsZero()
    {
        var plate = DefaultPlate(wasteRate: 0m);
        var result = QuoteCalculator.CalculatePlate(plate, DefaultPrinter(), DefaultSpool(), 0.25m);

        result.WasteOverhead.Should().Be(0m);
        result.PlateCost.Should().Be(
            result.MaterialCost + result.ElectricityCost + result.Amortization + result.Maintenance);
    }

    [Fact]
    public void CalculatePlate_ZeroPrintTime_ElectricityAmortizationMaintenanceAreZero()
    {
        var plate = new Plate { PrintTime = TimeSpan.Zero, FilamentUsageG = 50m, WasteRate = 0.05m };
        var result = QuoteCalculator.CalculatePlate(plate, DefaultPrinter(), DefaultSpool(), 0.25m);

        result.ElectricityCost.Should().Be(0m);
        result.Amortization.Should().Be(0m);
        result.Maintenance.Should().Be(0m);
        result.MaterialCost.Should().Be(1.00m); // 50 * 20 / 1000
    }

    [Fact]
    public void CalculateOrder_WithLabor_CorrectDirectCostAndTotal()
    {
        var plateResult = new PlateResult(2m, 0.25m, 0.20m, 0.10m, 0.1125m, 2.6625m);
        var labor = new LaborActivity { Minutes = 30m, HourlyRate = 40m };

        var result = QuoteCalculator.CalculateOrder([plateResult], [labor], 0.20m, 0.22m);

        // labor: 30 * 40 / 60 = 20.00
        result.LaborCost.Should().Be(20.00m);
        // direct: 2.6625 + 20 = 22.6625
        result.DirectCost.Should().Be(22.6625m);
        // total: 22.6625 * 1.20 * 1.22 = 33.1866...
        result.QuoteTotal.Should().Be(22.6625m * 1.20m * 1.22m);
    }

    [Fact]
    public void CalculateOrder_NoLabor_LaborCostIsZero()
    {
        var plateResult = new PlateResult(2m, 0.25m, 0.20m, 0.10m, 0.1125m, 2.6625m);

        var result = QuoteCalculator.CalculateOrder([plateResult], [], 0m, 0m);

        result.LaborCost.Should().Be(0m);
        result.DirectCost.Should().Be(2.6625m);
        result.QuoteTotal.Should().Be(2.6625m);
    }

    [Fact]
    public void CalculateOrder_MultiplePlates_SumsCosts()
    {
        var p1 = new PlateResult(1m, 0.1m, 0.05m, 0.02m, 0.055m, 1.225m);
        var p2 = new PlateResult(3m, 0.3m, 0.15m, 0.06m, 0.165m, 3.675m);

        var result = QuoteCalculator.CalculateOrder([p1, p2], [], 0m, 0m);

        result.PlatesCost.Should().Be(4.90m);
    }

    [Fact]
    public void CalculateOrder_ZeroMarginAndVat_QuoteTotalEqualsDirectCost()
    {
        var plateResult = new PlateResult(5m, 0.5m, 0.5m, 0.25m, 0.275m, 6.525m);

        var result = QuoteCalculator.CalculateOrder([plateResult], [], 0m, 0m);

        result.QuoteTotal.Should().Be(result.DirectCost);
    }
}
