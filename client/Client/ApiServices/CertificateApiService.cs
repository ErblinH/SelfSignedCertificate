using Client.Models;
using Client.Requests;
using Client.Results;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Client.ApiServices;

public class CertificateApiService : ICertificateApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientCredentialsTokenRequest _tokenRequest;

    public CertificateApiService(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest tokenRequest)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _tokenRequest = tokenRequest;
    }

    public async Task<Certificate> GetCertificateAsync(Guid name)
    {
        return JsonConvert.DeserializeObject<Certificate>(await GetResponse(_httpClientFactory.CreateClient("CertificateAPIClient")));
    }

    public async Task<IList<Certificate>> GetAllCertificatesAsync()
    {
        return JsonConvert.DeserializeObject<List<Certificate>>(await GetResponse(_httpClientFactory.CreateClient("CertificateAPIClient")));
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

    private async Task<string> GetResponse(HttpClient httpClient)
    {
        var httpTokenClient = _httpClientFactory.CreateClient("IDPClient");
        var tokenResponse = await httpTokenClient.RequestClientCredentialsTokenAsync(_tokenRequest);

        if (tokenResponse.IsError)
        {
            throw new HttpRequestException("Something went wrong while requesting the access token");
        }

        httpClient.SetBearerToken(tokenResponse.AccessToken);

        var response = await httpClient.GetAsync("https://localhost:7188/Certificate/list");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}