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
    public DbSet<EstimateItem> EstimateItems { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка индексов
        modelBuilder.Entity<TarifficatorItem>()
            .HasIndex(ti => ti.ItemCode)
            .IsUnique(false); // Неуникальный индекс
        
        modelBuilder.Entity<TarifficatorItem>()
            .HasIndex(ti => ti.Name);
        
        // Estimate — EstimateItem: каскадное удаление
        modelBuilder.Entity<Estimate>()
            .HasMany(e => e.EstimateItems)
            .WithOne(ei => ei.Estimate)
            .HasForeignKey(ei => ei.EstimateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    
}