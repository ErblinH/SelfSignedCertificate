using Client.Models;

namespace Client.Results;
public class SearchCertificateResult
{
    public uint Total { get; }
    public uint Offset { get; }
    public uint Limit { get; }
    public uint Count { get; }
    public IList<Certificate> Results { get; }

    public SearchCertificateResult(uint total, uint offset, uint limit, IList<Certificate> results)
    {
        this.Total = total;
        this.Offset = offset;
        this.Limit = limit;
        this.Count = (uint)results.Count;
        this.Results = results;
    }
}