namespace Forgia.Domain.Entities;

public class Printer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PowerW { get; set; }
    public decimal PurchaseCost { get; set; }
    public decimal UsefulLifeH { get; set; }
    public decimal MaintenanceCostPerH { get; set; }
    public ICollection<Plate> Plates { get; set; } = [];
}
