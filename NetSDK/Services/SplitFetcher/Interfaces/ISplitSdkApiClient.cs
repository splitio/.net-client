using NetSDK.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SplitFetcher.Interfaces
{
    public interface ISplitSdkApiClient
    {
        string FetchSplitChanges(long since);

        HTTPResult ExecuteGet(string requestUri);
    }
}
