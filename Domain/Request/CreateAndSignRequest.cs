namespace Domain.Request;

public class CreateAndSignRequest
{
    public string FilePath { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
}