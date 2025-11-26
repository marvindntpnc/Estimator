using Estimator.Data;
using Estimator.Inerfaces;
using PagedList;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Estimator.Domain;

namespace Estimator.Services;

public class RepositoryService<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ApplicationContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    #region Ctor

    public RepositoryService(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    #endregion
    
    #region Methods
    
    /// <summary>
    /// Get the entity entry
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entry
    /// </returns>
    public async Task<TEntity?> GetByIdAsync(int? id)
    {
        return await Table.FirstOrDefaultAsync(entity=>entity != null && entity.Id==Convert.ToInt32(id));
    }

    /// <summary>
    /// Get entity entries by identifiers
    /// </summary>
    /// <param name="ids">Entity entry identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public async Task<List<TEntity?>> GetByIdsAsync(IList<int> ids)
    {
        return await Table.Where(entity => entity != null && ids.Contains(entity.Id)).ToListAsync();
    }

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    public async Task<List<TEntity?>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
    {
        var query=Table;
        query = func != null ? func(query!) : query;

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get paged list of all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    public async Task<IPagedList<TEntity?>> GetAllPagedAsync(
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null, int pageIndex = 0, int pageSize = int.MaxValue,
        bool getOnlyTotalCount = false)
    {
        var query=Table;
        var result =await (func != null ? func(query!) : query).ToListAsync();

        return result.ToPagedList(pageIndex, pageSize);
    }
    
    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        //TODO update methods with events later
        // if (publishEvent)
        //     await _eventPublisher.EntityInsertedAsync(entity);
    }

    /// <summary>
    /// Insert entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);
        
        await _dbSet.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
        
        //TODO update methods with events later
        // if (publishEvent)
        // {
        //     foreach (var entity in entities)
        //         await _eventPublisher.EntityInsertedAsync(entity);
        // }
    }

    /// <summary>
    /// Update the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();

        //TODO update methods with events later
        // if (publishEvent)
        //     await _eventPublisher.EntityUpdatedAsync(entity);
    }

    /// <summary>
    /// Update entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (!entities.Any())
            return;

        foreach (var entity in entities) 
            _dbSet.Update(entity);
        
        await _dbContext.SaveChangesAsync();
        
        //TODO update methods with events later
        // if (publishEvent)
        // {
        //     foreach (var entity in entities)
        //         await _eventPublisher.EntityUpdatedAsync(entity);
        // }
    }
    
    /// <summary>
    /// Delete the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(TEntity entity, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
        
        //TODO update methods with events later
        // if (publishEvent)
        //     await _eventPublisher.EntityDeletedAsync(entity);
    }

    /// <summary>
    /// Delete entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
    {
        ArgumentNullException.ThrowIfNull(entities);
        
        _dbSet.RemoveRange(entities);
        //await _dbContext.SaveChangesAsync();
        //TODO update methods with events later
        // if (publishEvent)
        // {
        //     foreach (var entity in entities)
        //         await _eventPublisher.EntityDeletedAsync(entity);
        // }
    }
    #endregion
    
    #region Properties
    /// <summary>
    /// Gets a table
    /// </summary>
    public IQueryable<TEntity?> Table=> _dbSet.AsQueryable();

    #endregion
}
