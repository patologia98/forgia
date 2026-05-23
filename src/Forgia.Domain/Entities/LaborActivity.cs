namespace Forgia.Domain.Entities;

public class LaborActivity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Minutes { get; set; }
    public decimal HourlyRate { get; set; }
}
