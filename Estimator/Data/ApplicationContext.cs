using Estimator.Domain;
using Microsoft.EntityFrameworkCore;

namespace Estimator.Data;

public class ApplicationContext:DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        
    }
    public DbSet<TarifficatorItem> TarifficatorItems { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Estimate> Estimates { get; set; } = null!;
    public DbSet<EstimateCurrencyRate> EstimateCurrencyRates { get; set; } = null!;
    public DbSet<EstimateItem> EstimateItems { get; set; } = null!;
    public DbSet<Facility> Facilities { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<EstimateFacilities> EstimateFacilities { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TarifficatorItem>()
            .HasIndex(ti => ti.ItemCode)
            .IsUnique(false);
        
        modelBuilder.Entity<TarifficatorItem>()
            .HasIndex(ti => ti.Name);
        
        modelBuilder.Entity<Estimate>()
            .HasMany(e => e.EstimateItems)
            .WithOne(ei => ei.Estimate)
            .HasForeignKey(ei => ei.EstimateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EstimateCurrencyRate>()
            .HasKey(ecr => new { ecr.EstimateId, ecr.CurrencyType });

        modelBuilder.Entity<EstimateCurrencyRate>()
            .Property(ecr => ecr.Rate)
            .HasPrecision(18, 5);

        modelBuilder.Entity<EstimateItem>()
            .Property(ei => ei.CustomRate)
            .HasPrecision(18, 5);

        modelBuilder.Entity<TarifficatorItem>()
            .Property(t => t.Price)
            .HasPrecision(18, 5);
        
        modelBuilder.Entity<Facility>()
            .HasMany(f => f.ContractList)
            .WithOne(c => c.Facility)
            .HasForeignKey(c => c.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Facility>()
            .HasMany(f => f.DiscountRequirements)
            .WithOne(dr => dr.Facility)
            .HasForeignKey(dr => dr.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<EstimateFacilities>()
            .HasKey(ef => new { ef.EstimateId, ef.FacilityId });
        
        modelBuilder.Entity<EstimateFacilities>()
            .HasOne<Estimate>()
            .WithMany()
            .HasForeignKey(ef => ef.EstimateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<EstimateFacilities>()
            .HasOne<Facility>()
            .WithMany()
            .HasForeignKey(ef => ef.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}