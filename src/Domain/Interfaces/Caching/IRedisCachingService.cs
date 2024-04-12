namespace Domain.Interfaces.Caching;

public interface IRedisCachingService
{
    Task<T?> GetCacheItemAsync<T>(string key);

    Task SetCacheItemAsync<T>(string key, T value);

    Task<T?> GetOrSetCacheItemAsync<T>(string key, Func<Task<T?>> populateFunction);

    Task DeleteCacheItemAsync(string key);

    Task DeleteAllCacheItemsAsync();
}