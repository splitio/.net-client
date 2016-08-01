using NetSDK.Domain;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace NetSDK.Services.SplitFetcher.Classes
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
            var fetchResult = apiClient.FetchSplitChanges(since);

            var splitChangesResult = JsonConvert.DeserializeObject<SplitChangesResult>(fetchResult);
            return splitChangesResult;
        }
    }
}
