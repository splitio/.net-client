using Splitio.Domain;
using Splitio.Services.SplitFetcher.Interfaces;
using Newtonsoft.Json;

namespace Splitio.Services.SplitFetcher.Classes
{
    public class ApiSplitChangeFetcher: SplitChangeFetcher, ISplitChangeFetcher 
    {
        private readonly ISplitSdkApiClient apiClient;

        public ApiSplitChangeFetcher(ISplitSdkApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override SplitChangesResult FetchFromBackend(long since)
        {
            var fetchResultTask = apiClient.FetchSplitChanges(since);
            fetchResultTask.Wait();

            var splitChangesResult = JsonConvert.DeserializeObject<SplitChangesResult>(fetchResultTask.Result);
            return splitChangesResult;
        }
    }
}
