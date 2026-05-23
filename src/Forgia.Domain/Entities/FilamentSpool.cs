namespace Forgia.Domain.Entities;

public class FilamentSpool
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public decimal CostPerKg { get; set; }
    public decimal CurrentStockG { get; set; }
    public ICollection<Plate> Plates { get; set; } = [];
}
