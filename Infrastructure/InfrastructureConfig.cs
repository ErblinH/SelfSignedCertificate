namespace Infrastructure;

public record Connections
{
    public Database ContextDatabase { get; init; } = null!;

    public record Database
    {
        public string DatabaseName { get; init; } = "CertificateService";
        public string Host { get; init; } = null!;
        public string User { get; init; } = null!;
        public string Password { get; init; } = null!;

        public string ConnectionString =>
            $"Server={this.Host};Database={this.DatabaseName};User={this.User};Password={this.Password};";
    }

    public RedisDatabase Redis { get; init; } = null!;

    public record RedisDatabase
    {
        public string Host { get; init; } = null!;
        public int Port { get; init; }
        public int DefaultDatabase { get; init; }
        public string AuthToken { get; init; } = null!;
        public bool UseSsl { get; init; }
        public int SlidingExpirationInHours { get; init; }
        public string KeyPrefix { get; init; } = null!;
        public bool ClearRedisOnStartup { get; init; }

        public string ConnectionString =>
            $"{this.Host}:{this.Port},defaultDatabase={this.DefaultDatabase},ssl={this.UseSsl},password={this.AuthToken},channelPrefix={this.KeyPrefix}";
    }

    public ElasticSearchSession ElasticSearch { get; init; } = null!;

    public record ElasticSearchSession
    {
        public string Connection { get; init; } = null!;
        public string Index { get; init; } = null!;
    }
}