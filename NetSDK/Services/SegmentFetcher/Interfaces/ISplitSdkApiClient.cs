using NetSDK.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SplitFetcher.Interfaces
{
    public interface ISegmentSdkApiClient
    {
        string FetchSegmentChanges(string name, long since);

        HTTPResult ExecuteGet(string requestUri);
    }
}
