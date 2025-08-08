using Estimator.Data;
using Estimator.Inerfaces;
using Estimator.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace Estimator.Services;

public class RepositoryService<TEntity> : IRepository<TEntity> where TEntity : class,IEntity
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
    public List<TEntity> GetWhereAsync(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate).ToList();
    }

    public async Task<PagedList<TEntity>> GetPagedAsync(Func<TEntity, bool> predicate, int pageIndex, int pageSize)
    {
        var filteredQuery = _dbSet.Where(predicate);
        int totalCount = filteredQuery.Count();
        int skip = pageIndex * pageSize;
        
        var items = filteredQuery
            .OrderBy(i => i.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToList();
            
        var pagedList = new PagedList<TEntity>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
        return pagedList;
    }

    public PagedList<TEntity> ToPagedListAsync(List<TEntity> query,int pageIndex, int pageSize)
    {
        int totalCount = _dbSet.Count();
        int skip = (pageIndex) * pageSize;
        var items=query.OrderBy(i=>i.Id)
            .Skip(skip)
            .Take(pageSize).ToList();
        var pagedList=new PagedList<TEntity>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
        return pagedList;
    }

}