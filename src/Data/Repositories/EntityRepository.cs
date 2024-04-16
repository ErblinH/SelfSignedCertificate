using Domain.Entities;
using Domain.Interfaces.Caching;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly CertificateDbContext _databaseContext;
    protected readonly DbSet<TEntity> _table;
    private IRedisCachingService _redisCachingService;
    public virtual IQueryable<TEntity> Table => _table;

    public EntityRepository(
        CertificateDbContext databaseContext,
        IRedisCachingService redisCachingService)
    {
        _databaseContext = databaseContext;
        _redisCachingService = redisCachingService;
        _table = _databaseContext.Set<TEntity>();
    }

    private async Task<TEntity> GetFromDbAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
    {
        try
        {
            var query = func != null ? func(Table) : Table;
            query = query.Where(x => !x.Deleted);
            return await query.FirstOrDefaultAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<TEntity> GetAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, string key = null)
    {
        return await _redisCachingService.GetOrSetCacheItemAsync($"GetCertificate_{key}", () => GetFromDbAsync(func));
    }

    private async Task<IList<TEntity>> GetAllFromDbAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
    {
        var query = func != null ? func(Table) : Table;
        query = query.Where(x => !x.Deleted);
        return await query.ToListAsync();
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
    {
        return await _redisCachingService.GetOrSetCacheItemAsync("GetAllCertificate", () => GetAllFromDbAsync(func));
    }

    public virtual async Task<bool> InsertAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        try
        {
            await _table.AddAsync(entity);
            return await _databaseContext.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be added: {ex.Message} and {ex.InnerException}");
        }
    }
}