using Splitio.Domain;
using Splitio.Services.SplitFetcher.Interfaces;
using Newtonsoft.Json;
using Splitio.Services.SegmentFetcher.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class ApiSegmentChangeFetcher: SegmentChangeFetcher, ISegmentChangeFetcher
    {
        private readonly ISegmentSdkApiClient apiClient;

        public ApiSegmentChangeFetcher(ISegmentSdkApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override async Task<SegmentChange> FetchFromBackend(string name, long since, CancellationToken token)
        {
            var fetchResult = await apiClient.FetchSegmentChanges(name, since, token);

            var segmentChange = JsonConvert.DeserializeObject<SegmentChange>(fetchResult);
            return segmentChange;
        }
    }
}
