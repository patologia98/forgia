namespace Forgia.Domain.Entities;

public class Plate
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int PrinterId { get; set; }
    public Printer Printer { get; set; } = null!;
    public int SpoolId { get; set; }
    public FilamentSpool Spool { get; set; } = null!;
    public TimeSpan PrintTime { get; set; }
    public decimal FilamentUsageG { get; set; }
    public decimal WasteRate { get; set; }
}
