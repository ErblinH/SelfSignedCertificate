using Api.Middleware;
using Application.Services;
using Data;
using Data.Repositories;
using Domain.Interfaces.Caching;
using Domain.Interfaces.ElasticSearch;
using Domain.Interfaces.Service;
using Infrastructure;
using Infrastructure.Caching;
using Infrastructure.ElasticSearch;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Connections>()
    .Bind(builder.Configuration.GetSection(nameof(Connections)));

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IRedisCachingService, RedisCachingService>();
builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
builder.Services.AddScoped<AntiXssMiddleware>();

builder.Services.AddHostedService<ElasticSearchHostedService>();
builder.Services.AddHostedService<RedisCachingHostedService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connections = builder.Configuration.GetSection(nameof(Connections)).Get<Connections>();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(connections.ElasticSearch.Connection))
    {
        TypeName = null,
        AutoRegisterTemplate = true,
        IndexFormat = connections.ElasticSearch.Index,
        BatchAction = ElasticOpType.Create,

    })
    .CreateLogger();

builder.Services.AddLogging();

//var ok = "Server=localhost;Database=cert;User=root;Password=admin;";

builder.Services.AddDbContext<CertificateDbContext>(options =>
    options.UseMySql(connections.ContextDatabase.ConnectionString, ServerVersion.AutoDetect(connections.ContextDatabase.ConnectionString), b => b.MigrationsAssembly("Data")));

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var ok = "localhost:6379,defaultDatabase=3,ssl=false,password=\"\",channelPrefix=certificateservice";

    var configuration = ConfigurationOptions.Parse(connections.Redis.ConnectionString);
    configuration.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(configuration);
});

var app = builder.Build();

app.UseMiddleware<AntiXssMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();