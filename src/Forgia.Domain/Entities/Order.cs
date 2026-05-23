namespace Forgia.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal MarginRate { get; set; }
    public decimal VatRate { get; set; }
    public ICollection<Project> Projects { get; set; } = [];
    public ICollection<LaborActivity> LaborActivities { get; set; } = [];
}
