using Splitio.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Splitio.Services.SegmentFetcher.Interfaces
{
    public interface ISegmentChangeFetcher
    {
        Task<SegmentChange> Fetch(string name, long change_number, CancellationToken token = default(CancellationToken));
    }
}
