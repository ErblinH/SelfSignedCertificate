using Domain.Entities;

namespace Data.Repositories;

public partial interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, string key = null);
    Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);
    Task<bool> InsertAsync(TEntity entity);
}