using Forgia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forgia.Infrastructure.Persistence;

public class ForgiaDbContext : DbContext
{
    public ForgiaDbContext(DbContextOptions<ForgiaDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Printer> Printers => Set<Printer>();
    public DbSet<FilamentSpool> FilamentSpools => Set<FilamentSpool>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Plate> Plates => Set<Plate>();
    public DbSet<LaborActivity> LaborActivities => Set<LaborActivity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Printer>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.PurchaseCost).HasColumnType("TEXT");
            e.Property(p => p.UsefulLifeH).HasColumnType("TEXT");
            e.Property(p => p.MaintenanceCostPerH).HasColumnType("TEXT");
        });

        modelBuilder.Entity<FilamentSpool>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.Material).IsRequired().HasMaxLength(50);
            e.Property(s => s.CostPerKg).HasColumnType("TEXT");
            e.Property(s => s.CurrentStockG).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.MarginRate).HasColumnType("TEXT");
            e.Property(o => o.VatRate).HasColumnType("TEXT");
            e.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.HasOne(p => p.Order)
                .WithMany(o => o.Projects)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Plate>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.FilamentUsageG).HasColumnType("TEXT");
            e.Property(p => p.WasteRate).HasColumnType("TEXT");
            e.Property(p => p.PrintTime)
                .HasConversion(
                    v => v.Ticks,
                    v => TimeSpan.FromTicks(v));
            e.HasOne(p => p.Project)
                .WithMany(pr => pr.Plates)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Printer)
                .WithMany(pr => pr.Plates)
                .HasForeignKey(p => p.PrinterId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Spool)
                .WithMany(s => s.Plates)
                .HasForeignKey(p => p.SpoolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LaborActivity>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Description).IsRequired().HasMaxLength(500);
            e.Property(l => l.Minutes).HasColumnType("TEXT");
            e.Property(l => l.HourlyRate).HasColumnType("TEXT");
            e.HasOne(l => l.Order)
                .WithMany(o => o.LaborActivities)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
