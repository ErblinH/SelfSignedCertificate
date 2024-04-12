using Domain.Interfaces.Caching;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Caching;

public class RedisCachingService : IRedisCachingService
{
    private IDatabase CacheDatabase { get; }
    private double ExpirationInterval { get; }
    private string RedisKeyPrefix { get; }

    public RedisCachingService(IConnectionMultiplexer cacheDatabase, IOptions<Connections> options)
    {
        this.CacheDatabase = cacheDatabase.GetDatabase();
        this.ExpirationInterval = options.Value.Redis.SlidingExpirationInHours;
        this.RedisKeyPrefix = options.Value.Redis.KeyPrefix;
    }

    public async Task<T?> GetCacheItemAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException(message: $"Argument {nameof(key)} can not be null or empty.");

        var valueJson = new RedisValue();

        try
        {
            valueJson = await this.CacheDatabase.StringGetAsync($"{this.RedisKeyPrefix}_{key}");
        }
        catch (Exception exception)
        {
            Log.Error("Exception message={error}", exception.Message);
            return default;
        }

        try
        {
            return valueJson.IsNull ? default : JsonSerializer.Deserialize<T>(valueJson);
        }
        catch (Exception exception)
        {
            Log.Error("Unable to deserialize cache item with key={key} with exception message={error}", key, exception.Message);
            return default;
        }
    }

    public async Task SetCacheItemAsync<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException($"Argument {nameof(key)} can not be null or empty.");
        if (value is null) throw new ArgumentNullException(paramName: nameof(value));

        try
        {
            await this.CacheDatabase.StringSetAsync($"{this.RedisKeyPrefix}_{key}", JsonSerializer.Serialize(value), TimeSpan.FromHours(this.ExpirationInterval));
        }
        catch (Exception exception)
        {
            Log.Error("{method} called with faulty redis server. with exception message={error}", nameof(this.DeleteCacheItemAsync), exception.Message);
        }
    }

    public async Task<T?> GetOrSetCacheItemAsync<T>(string key, Func<Task<T?>> populateFunction)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException($"Argument {nameof(key)} can not be null or empty.");
        if (populateFunction is null) throw new ArgumentNullException(nameof(populateFunction));

        var value = await this.GetCacheItemAsync<T?>(key);

        if (value != null && ((value as IEnumerable<object>)?.Any() ?? true))
            return value;

        value = await populateFunction();

        if (value != null) await this.SetCacheItemAsync(key, value);

        return value;
    }

    public Task DeleteCacheItemAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllCacheItemsAsync()
    {
        var keys = (string[])this.CacheDatabase.Execute("KEYS", $"{this.RedisKeyPrefix}*");

        if (keys.Any())
            this.CacheDatabase.Execute("DEL", keys);

        return Task.CompletedTask;
    }
}