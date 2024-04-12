using Domain.ElasticSearch;
using Domain.Entities;
using Domain.Request;

namespace Domain.Interfaces.ElasticSearch;

public interface IElasticSearchService
{
    Task InsertOrUpdateCertificatesAsync(IList<Certificate> certificatesCollection);

    Task<SearchCertificateResult> SearchCertificatesAsync(SearchCertificatesRequest searchCertificateParameters);

    Task CreateIndexAsync();
}