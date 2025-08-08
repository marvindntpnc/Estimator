using Estimator.Data;
using Estimator.Inerfaces;
using Microsoft.EntityFrameworkCore;

namespace Estimator.Services;

public class RepositoryService<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public RepositoryService(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}