using Client.Models;
using Client.Requests;
using Client.Results;

namespace Client.ApiServices;

public interface ICertificateApiService
{
    Task<Certificate> GetCertificateAsync(Guid name);

    Task<IList<Certificate>> GetAllCertificatesAsync();

    Task<Certificate> CreateCertificateAsync(CreateAndSignRequest createAndSignRequest);

    Task<bool> CheckCertificateAsync(Guid name);

    Task<SearchCertificateResult> SearchCertificateAsync(SearchCertificatesRequest searchCertificatesRequest);
}