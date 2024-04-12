using Domain.Interfaces.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching;

public class RedisCachingHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private bool _clearRedisOnStartup;

    public RedisCachingHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<Connections> options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _clearRedisOnStartup = options.Value.Redis.ClearRedisOnStartup;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var redisSearchService = scope.ServiceProvider.GetRequiredService<IRedisCachingService>();

            if (_clearRedisOnStartup)
                await redisSearchService.DeleteAllCacheItemsAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}