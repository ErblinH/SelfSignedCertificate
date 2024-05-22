using Client.Models;
using Client.Requests;
using Client.Results;

namespace Client.ApiServices;

public class CertificateApiService : ICertificateApiService
{
    public async Task<Certificate> GetCertificateAsync(Guid name)
    {
        return new Certificate()
        {
            Id = 1,
            Name = new Guid("c0ced540-3c9f-4db3-994c-bc852c1d49b8"),
            Subject = "CN=Self Signed Certificate",
            Issuer = "CN=Self Signed Certificate",
            KeyPairs = "Test_Keys",
            FilePath = "Test_Path",
            ValidFrom = DateTime.UtcNow,
            ValidUntil = DateTime.UtcNow.AddDays(1),
            CreationDateTime = DateTime.UtcNow,
            Deleted = false
        };
    }

    public async Task<IList<Certificate>> GetAllCertificatesAsync()
    {
        return new List<Certificate>()
        {
            new Certificate()
            {
                Id = 1,
                Name = new Guid("c0ced540-3c9f-4db3-994c-bc852c1d49b8"),
                Subject = "CN=Self Signed Certificate",
                Issuer = "CN=Self Signed Certificate",
                KeyPairs = "Test_Keys",
                FilePath = "Test_Path",
                ValidFrom = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddDays(1),
                CreationDateTime = DateTime.UtcNow,
                Deleted = false
            }
        };
    }

    public Task<Certificate> CreateCertificateAsync(CreateAndSignRequest createAndSignRequest)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckCertificateAsync(Guid name)
    {
        throw new NotImplementedException();
    }

    public Task<SearchCertificateResult> SearchCertificateAsync(SearchCertificatesRequest searchCertificatesRequest)
    {
        throw new NotImplementedException();
    }
}