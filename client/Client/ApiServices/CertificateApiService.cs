using Client.Models;
using Client.Requests;
using Client.Results;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Client.ApiServices;

public class CertificateApiService : ICertificateApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClientCredentialsTokenRequest _tokenRequest;

    public CertificateApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ClientCredentialsTokenRequest tokenRequest)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _tokenRequest = tokenRequest;
    }

    public async Task<Certificate> GetCertificateAsync(Guid name)
    {
        var httpClient = _httpClientFactory.CreateClient("CertificateAPIClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/c0ced540-3c9f-4db3-994c-bc852c1d49b8");

        var response = await httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var retrievedCertificate = JsonConvert.DeserializeObject<Certificate>(content);

        return retrievedCertificate;
    }

    public async Task<IList<Certificate>> GetAllCertificatesAsync()
    {
        var httpTokenClient = _httpClientFactory.CreateClient("IDPClient");
        var tokenResponse = await httpTokenClient.RequestClientCredentialsTokenAsync(_tokenRequest);

        if (tokenResponse.IsError)
        {
            throw new HttpRequestException("Something went wrong while requesting the access token");
        }

        var httpClient = _httpClientFactory.CreateClient("CertificateAPIClient");

        var request = new HttpRequestMessage(HttpMethod.Get, "list");
        httpClient.SetBearerToken(tokenResponse.AccessToken);

        //var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        var response = await httpClient.GetAsync("https://localhost:7188/Certificate/list");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var retrievedCertificates = JsonConvert.DeserializeObject<List<Certificate>>(content);

        return retrievedCertificates;


        //var apiClientCredentials = new ClientCredentialsTokenRequest
        //{
        //    Address = "https://localhost:7168/connect/token",

        //    ClientId = "certificateClient",
        //    ClientSecret = "secret",

        //    Scope = "certificateAPI"
        //};

        //var client = new HttpClient();

        //var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);

        //if (tokenResponse.IsError)
        //    return null;

        //client.SetBearerToken(tokenResponse.AccessToken);

        //var response = await client.GetAsync("https://localhost:7188/Certificate/list");
        //response.EnsureSuccessStatusCode();

        //var content = await response.Content.ReadAsStringAsync();
        //var retrievedCertificates = JsonConvert.DeserializeObject<List<Certificate>>(content);

        //return retrievedCertificates;
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