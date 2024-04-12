namespace Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid Name { get; set; }
    public string Subject { get; set; }
    public string Issuer { get; set; }
    public string KeyPairs { get; set; }
    public byte[] Data { get; set; }
    public string FilePath { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }

}