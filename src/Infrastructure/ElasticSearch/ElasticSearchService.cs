using Domain.ElasticSearch;
using Domain.Entities;
using Domain.Interfaces.ElasticSearch;
using Domain.Request;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using Serilog;

namespace Infrastructure.ElasticSearch;

public class ElasticSearchService : IElasticSearchService
{
    private SingleNodeConnectionPool ConnectionPool { get; }
    private string _index { get; }

    public ElasticSearchService(IOptions<Connections> connectionsOptions)
    {
        ConnectionPool = new SingleNodeConnectionPool(new Uri(connectionsOptions.Value.ElasticSearch.Connection));
        _index = connectionsOptions.Value.ElasticSearch.Index;
    }

    public async Task InsertOrUpdateCertificatesAsync(IList<Certificate> certificatesCollection)
    {
        var response = await this.GetClient().BulkAsync(a => a.IndexMany(certificatesCollection, (descriptor, certificate) => descriptor.Index($"{_index}").Id(certificate.Name)).Refresh(Refresh.True));

        if (!response?.IsValid ?? true)
            Log.Error("Failed to index the following merchants={merchants} where the response={response}",
                string.Join(",", certificatesCollection.Select(merchant => merchant.Id)), response);
    }

    public async Task<SearchCertificateResult> SearchCertificatesAsync(SearchCertificatesRequest searchCertificateParameters)
    {
        var query = CreateCertificateQuery(searchCertificateParameters);

        var result = await this.GetClient().SearchAsync<Certificate>(search => search
            .Index(_index)
            .Sort(sort => sort.Descending(desc => desc.CreationDateTime))
            .Size((int)searchCertificateParameters.Limit)
            .From((int)searchCertificateParameters.Offset)
            .Query(query));

        if (!result?.IsValid ?? true)
        {
            var error = $"Failed to search Certificates with Guid={searchCertificateParameters.Name}.";
            Log.Error(error);
            throw new IOException(error);
        }

        return new SearchCertificateResult
        (
            (uint)(result?.Total ?? 0),
            (uint)searchCertificateParameters.Offset,
            (uint)searchCertificateParameters.Limit,
            result?.Documents.ToList() ?? new List<Certificate>()
        );
    }

    public async Task CreateIndexAsync()
    {
        var elasticSearchClient = this.GetClient();
        var index = _index;
        var indexToBeCreated = await elasticSearchClient.Indices.ExistsAsync(index);

        if (indexToBeCreated.Exists)
            return;

        var result = await elasticSearchClient.Indices.CreateAsync(index, create => create
            .Settings(setting => setting
                .Analysis(analysis => analysis
                    .Normalizers(normalizer => normalizer
                        .Custom("case_insensitive", custom => custom
                            .Filters("lowercase")))))
            .Map(map => map
                .DynamicTemplates(dynamictemplate => dynamictemplate
                    .DynamicTemplate("string_to_keyword", dynamictemplatedescriptor => dynamictemplatedescriptor
                        .MatchMappingType("string")
                        .Mapping(singlemapping => singlemapping.Keyword(keyword => keyword.Normalizer("case_insensitive")))))));

        if (!result?.IsValid ?? true)
            Log.Error("Failed to create index={index} at Elastic Search with result={result}.", _index, result);
    }

    private IElasticClient GetClient()
    {
        return new ElasticClient(new ConnectionSettings(this.ConnectionPool));
    }

    private static Func<QueryContainerDescriptor<Certificate>, QueryContainer> CreateCertificateQuery(SearchCertificatesRequest searchCertificateParameters)
    {
        return query => query
            .Bool(boolQuery =>
            {
                var mustParams = new List<Func<QueryContainerDescriptor<Certificate>, QueryContainer>>();

                if (!string.IsNullOrWhiteSpace(searchCertificateParameters.Needle))
                {
                    boolQuery.MinimumShouldMatch(1);

                    var needle = $"*{searchCertificateParameters.Needle.ToLowerInvariant()}*";

                    var shouldParams = new List<Func<QueryContainerDescriptor<Certificate>, QueryContainer>>()
                    {
                        should => should.Wildcard(w => w.Field(field => field.Subject).Value(needle)),
                        should => should.Wildcard(w => w.Field(field => field.Issuer).Value(needle))
                    };

                    boolQuery.Should(shouldParams);
                }

                mustParams.Add(must => must.Term(search => search.Field(field => field.Name).Value(searchCertificateParameters.Name.ToString()!.ToLowerInvariant())));

                return boolQuery.Must(mustParams);
            });
    }
}