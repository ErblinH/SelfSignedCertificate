using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace GenerateCertificate;

public class GenerateCertificate : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Example data to sign

        AsymmetricCipherKeyPair keyPair;
        var certificate = GenerateSelfSignedCertificate(out keyPair);

        // Example data to sign
        string documentPath = "C:\\Users\\e.halabaku\\OneDrive - Buckaroo\\Desktop\\FSHMN, Master\\Viti_1\\Semestri_2\\Siguria\\SelfSignedCertificate\\test.txt";
        byte[] dataToSign = File.ReadAllBytes(documentPath);

        // Sign the document
        byte[] signature = SignData(dataToSign, keyPair);
        Console.WriteLine($"Signature: {Convert.ToBase64String(signature)}");

        // Optionally, verify the signature (demonstration purposes)
        ISigner verifier = SignerUtilities.GetSigner("SHA256WITHRSA");
        verifier.Init(false, keyPair.Public);
        verifier.BlockUpdate(dataToSign, 0, dataToSign.Length);
        bool isVerified = verifier.VerifySignature(signature);
        Console.WriteLine($"Signature is valid: {isVerified}");


        using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
        {
            store.Open(OpenFlags.ReadOnly);

            var oki = store.Certificates;

            Console.WriteLine($"Certificates in {StoreName.My} Store:");
            foreach (X509Certificate2 cert in store.Certificates)
            {
                Console.WriteLine($"Subject: {cert.Subject}");
                Console.WriteLine($"Issuer: {cert.Issuer}");
                Console.WriteLine($"Thumbprint: {cert.Thumbprint}");
                Console.WriteLine($"Expiration date: {cert.NotAfter}");
            }
        }


        return Task.CompletedTask;
    }

    public static X509Certificate GenerateSelfSignedCertificate(out AsymmetricCipherKeyPair keyPair)
    {
        // Generate a key pair
        var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 2048);
        var keyPairGenerator = new RsaKeyPairGenerator();
        keyPairGenerator.Init(keyGenerationParameters);
        keyPair = keyPairGenerator.GenerateKeyPair();

        // Define certificate generator
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

    public static byte[] SignData(byte[] dataToSign, AsymmetricCipherKeyPair keyPair)
    {
        ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");
        signer.Init(true, keyPair.Private);
        signer.BlockUpdate(dataToSign, 0, dataToSign.Length);
        return signer.GenerateSignature();
    }

    public static string SerializeKeyPair(AsymmetricCipherKeyPair keyPair)
    {
        StringWriter writer = new StringWriter();
        PemWriter pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(keyPair.Private);
        pemWriter.WriteObject(keyPair.Public);
        pemWriter.Writer.Flush();
        return writer.ToString();
    }

    public static AsymmetricCipherKeyPair DeserializeKeyPair(string serializedKeyPair)
    {
        StringReader reader = new StringReader(serializedKeyPair);
        PemReader pemReader = new PemReader(reader);

        AsymmetricCipherKeyPair keyPair = new AsymmetricCipherKeyPair(
            (AsymmetricKeyParameter)pemReader.ReadObject(),  // Private key
            (AsymmetricKeyParameter)pemReader.ReadObject()   // Public key
        );

        return keyPair;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}