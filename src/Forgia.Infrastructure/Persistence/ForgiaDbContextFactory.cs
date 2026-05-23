using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Forgia.Infrastructure.Persistence;

public class ForgiaDbContextFactory : IDesignTimeDbContextFactory<ForgiaDbContext>
{
    public ForgiaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ForgiaDbContext>()
            .UseSqlite("Data Source=forgia.db;Foreign Keys=True")
            .Options;
        return new ForgiaDbContext(options);
    }
}
