using System.Threading;
using System.Threading.Tasks;

namespace Splitio.CommonLibraries
{
    public interface ISdkApiClient
    {
        Task<HTTPResult> ExecuteGet(string requestUri, CancellationToken token = default(CancellationToken));

        Task<HTTPResult> ExecutePost(string requestUri, string data, CancellationToken token = default(CancellationToken));
    }
}
