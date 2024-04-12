using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories;

public partial interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);
    Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);
    Task<bool> InsertAsync(TEntity entity);
}