using Estimator.Domain;
using Estimator.Models.Shared;
using PagedList;

namespace Estimator.Inerfaces;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    #region Methods
    
    /// <summary>
    /// Get the entity entry
    /// </summary>
    /// <param name="id">Entity entry identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entry
    /// </returns>
    Task<TEntity?> GetByIdAsync(int? id);

    /// <summary>
    /// Get entity entries by identifiers
    /// </summary>
    /// <param name="ids">Entity entry identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    Task<List<TEntity?>> GetByIdsAsync(IList<int> ids);

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the entity entries
    /// </returns>
    Task<List<TEntity?>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null);

    /// <summary>
    /// Get all entity entries
    /// </summary>
    /// <param name="func">Function to select entries</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the paged list of entity entries
    /// </returns>
    Task<IPagedList<TEntity?>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null,
        int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
    /// <summary>
    /// Insert the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAsync(TEntity entity, bool publishEvent = true);
    /// <summary>
    /// Insert entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAsync(IList<TEntity> entities, bool publishEvent = true);
    /// <summary>
    /// Update the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAsync(TEntity entity, bool publishEvent = true);
    /// <summary>
    /// Update entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true);
    /// <summary>
    /// Delete the entity entry
    /// </summary>
    /// <param name="entity">Entity entry</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteAsync(TEntity entity, bool publishEvent = true);
    /// <summary>
    /// Delete entity entries
    /// </summary>
    /// <param name="entities">Entity entries</param>
    /// <param name="publishEvent">Whether to publish event notification</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true);
    
    #endregion
    
    #region Properties

    /// <summary>
    /// Gets a table
    /// </summary>
    IQueryable<TEntity?> Table { get; }

    #endregion
}