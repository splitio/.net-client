using NetSDK.Domain;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using NetSDK.Services.SegmentFetcher.Interfaces;

namespace NetSDK.Services.SegmentFetcher.Classes
{
    public class ApiSegmentChangeFetcher: SegmentChangeFetcher, ISegmentChangeFetcher
    {
        private readonly ISegmentSdkApiClient apiClient;

        public ApiSegmentChangeFetcher(ISegmentSdkApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        protected override SegmentChange FetchFromBackend(string name, long since)
        {
            var fetchResult = apiClient.FetchSegmentChanges(name, since);

            var segmentChange = JsonConvert.DeserializeObject<SegmentChange>(fetchResult);
            return segmentChange;
        }
    }
}
