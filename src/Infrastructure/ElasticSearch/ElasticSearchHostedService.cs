using Domain.Interfaces.ElasticSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.ElasticSearch;

public class ElasticSearchHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ElasticSearchHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var elasticSearchService = scope.ServiceProvider.GetRequiredService<IElasticSearchService>();
            await elasticSearchService.CreateIndexAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}