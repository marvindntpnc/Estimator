using Estimator.Models.Shared;

namespace Estimator.Inerfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<List<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    List<TEntity> GetWhereAsync(Func<TEntity, bool> predicate);
    Task<PagedList<TEntity>> GetPagedAsync(Func<TEntity, bool> predicate, int pageIndex, int pageSize);
    PagedList<TEntity>ToPagedListAsync(List<TEntity> query,int pageNumber, int pageSize);
}
public interface IEntity
{
    int Id { get; set; }
}