namespace Forgia.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public ICollection<Plate> Plates { get; set; } = [];
}
