using Application.Adapters;
using Data.Repositories;
using Domain.ElasticSearch;
using Domain.Entities;
using Domain.Interfaces.ElasticSearch;
using Domain.Interfaces.Service;
using Domain.Request;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Serilog;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace Application.Services;

public class CertificateService : ICertificateService
{
    private readonly IRepository<Certificate> _certificateRepository;
    private IElasticSearchService _elasticSearchService { get; }

    public CertificateService(IRepository<Certificate> certificateRepository, IElasticSearchService elasticSearchService)
    {
        _certificateRepository = certificateRepository;
        _elasticSearchService = elasticSearchService;
    }

    #region Create

    public async Task<Certificate> CreateCertificateAsync(CreateAndSignRequest createAndSignRequest)
    {
        Log.Information("Started to sign the document with path={path}", createAndSignRequest.FilePath);

        AsymmetricCipherKeyPair keyPair;
        var certificate = GenerateSelfSignedCertificate(out keyPair);

        var dataToSign = File.ReadAllBytes(createAndSignRequest.FilePath);
        var signature = SignData(dataToSign, keyPair);

        var createdCertificate = createAndSignRequest.ToDomain(certificate.CertificateStructure, keyPair, signature);

        await _certificateRepository.InsertAsync(createdCertificate);

        await _elasticSearchService.InsertOrUpdateCertificatesAsync(new List<Certificate> { createdCertificate });

        return createdCertificate;
    }

    #endregion Create

    #region Get

    public async Task<Certificate> GetCertificateAsync(Guid name)
    {
        Log.Information("Started to retrieve the document with name={GuidName}", name);

        return await _certificateRepository.GetAsync(query => query.Where(cert => cert.Name == name), name.ToString());
    }

    #endregion Get

    #region GetAll

    public async Task<IList<Certificate>> GetAllCertificatesAsync()
    {
        Log.Information("Started to retrieve all documents");

        return await _certificateRepository.GetAllAsync();
    }

    #endregion GetAll

    #region Check

    public async Task<bool> CheckCertificateAsync(Guid name)
    {
        Log.Information("Started to check the document signature with name={GuidName}", name);

        var retrievedCertificate = await _certificateRepository.GetAsync(query => query.Where(cert => cert.Name == name), name.ToString());

        var keyPairs = DeserializeKeyPair(retrievedCertificate.KeyPairs);

        ISigner verifier = SignerUtilities.GetSigner("SHA256WITHRSA");
        verifier.Init(false, keyPairs.Public);

        var dataToCheck = File.ReadAllBytes(retrievedCertificate.FilePath);

        verifier.BlockUpdate(dataToCheck, 0, dataToCheck.Length);
        return verifier.VerifySignature(retrievedCertificate.Data);
    }

    #endregion Check

    #region Search

    public async Task<SearchCertificateResult> SearchCertificateAsync(SearchCertificatesRequest searchCertificatesRequest)
    {
        return await _elasticSearchService.SearchCertificatesAsync(searchCertificatesRequest);
    }

    #endregion

    #region Private method

    private static X509Certificate GenerateSelfSignedCertificate(out AsymmetricCipherKeyPair keyPair)
    {
        // Generate a key pair used for RSA function
        var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 2048);
        var keyPairGenerator = new RsaKeyPairGenerator();
        keyPairGenerator.Init(keyGenerationParameters);
        keyPair = keyPairGenerator.GenerateKeyPair();

        // Define certificate generator using BouncyCastle
        var certificateGenerator = new X509V3CertificateGenerator();
        var serialNumber = BigIntegers.CreateRandomInRange(Org.BouncyCastle.Math.BigInteger.One, Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), new SecureRandom());
        certificateGenerator.SetSerialNumber(serialNumber);
        certificateGenerator.SetIssuerDN(new X509Name("CN=Self Signed Certificate"));
        certificateGenerator.SetSubjectDN(new X509Name("CN=Self Signed Certificate"));
        certificateGenerator.SetNotBefore(DateTime.UtcNow.Date);
        certificateGenerator.SetNotAfter(DateTime.UtcNow.Date.AddYears(1));
        certificateGenerator.SetPublicKey(keyPair.Public);

        // Define signature algorithm which is used for hashing
        var signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", keyPair.Private);

        // Generate the certificate
        return certificateGenerator.Generate(signatureFactory);
    }

    private static byte[] SignData(byte[] dataToSign, AsymmetricCipherKeyPair keyPair)
    {
        ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
        signer.Init(true, keyPair.Private);
        signer.BlockUpdate(dataToSign, 0, dataToSign.Length);
        return signer.GenerateSignature();
    }

    private static AsymmetricCipherKeyPair DeserializeKeyPair(string serializedKeyPair)
    {
        StringReader reader = new StringReader(serializedKeyPair);
        PemReader pemReader = new PemReader(reader);

        object readObject = pemReader.ReadObject();

        if (readObject is AsymmetricCipherKeyPair keyPair)
        {
            return keyPair;  // Directly return the key pair if it's the correct type
        }
        else
        {
            throw new InvalidCastException("The serialized data does not contain a valid asymmetric key pair.");
        }
    }

    #endregion Private method
}