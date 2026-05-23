using Microsoft.EntityFrameworkCore;

namespace Forgia.Infrastructure.Persistence;

public class ForgiaDbContext : DbContext
{
    public ForgiaDbContext(DbContextOptions<ForgiaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
