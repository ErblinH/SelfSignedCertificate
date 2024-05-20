using Domain.Entities;
using Domain.Request;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace Application.Adapters;

public static class CertificateAdapter
{
    public static Certificate ToDomain(this CreateAndSignRequest createAndSignRequest, X509CertificateStructure x509certificate, AsymmetricCipherKeyPair keypairs, byte[] data)
    {
        var certificate = new Certificate();

        certificate.Name = Guid.NewGuid();
        certificate.Subject = x509certificate.Subject.ToString();
        certificate.Issuer = x509certificate.Issuer.ToString();
        certificate.KeyPairs = SerializeKeyPair(keypairs);
        certificate.Data = data;
        certificate.FilePath = createAndSignRequest.FilePath;
        certificate.ValidFrom = createAndSignRequest.ValidFrom;
        certificate.ValidUntil = createAndSignRequest.ValidUntil;
        certificate.CreationDateTime = DateTime.UtcNow;

        return certificate;
    }

    private static string SerializeKeyPair(AsymmetricCipherKeyPair keyPair)
    {
        StringWriter writer = new StringWriter();
        PemWriter pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(keyPair.Private);
        pemWriter.WriteObject(keyPair.Public);
        pemWriter.Writer.Flush();
        return writer.ToString();
    }
}