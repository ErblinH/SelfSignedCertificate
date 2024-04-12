using Domain.ElasticSearch;
using Domain.Entities;
using Domain.Request;

namespace Domain.Interfaces.Service;

public interface ICertificateService
{
    Task<Certificate> GetCertificateAsync(Guid name);

    Task<IList<Certificate>> GetAllCertificatesAsync();

    Task<Certificate> CreateCertificateAsync(CreateAndSignRequest createAndSignRequest);

    Task<bool> CheckCertificateAsync(Guid name);

    Task<SearchCertificateResult> SearchCertificateAsync(SearchCertificatesRequest searchCertificatesRequest);
}