using Estimator.Domain;
using Microsoft.EntityFrameworkCore;

namespace Estimator.Data;

public class ApplicationContext:DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        
    }
    public DbSet<Tarifficator> Tarifficators { get; set; } = null!;
    public DbSet<TarifficatorItem> TarifficatorItems { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи 1 ко многим между TariffFile и TariffItem
        modelBuilder.Entity<Tarifficator>()
            .HasMany(tf => tf.TarifficatorItems)
            .WithOne(ti => ti.Tarifficator)
            .HasForeignKey(ti => ti.TarificatorId)
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление

        // Настройка индексов
        modelBuilder.Entity<TarifficatorItem>()
            .HasIndex(ti => ti.ItemCode)
            .IsUnique(false); // Неуникальный индекс
        
        modelBuilder.Entity<TarifficatorItem>()
            .HasIndex(ti => ti.Name);
    }
    
    
}