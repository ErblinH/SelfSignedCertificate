namespace Domain.Request;

public class SearchCertificatesRequest
{
    public string Needle { get; set; }
    public Guid Name { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}