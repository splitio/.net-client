using Splitio.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SplitFetcher.Interfaces
{
    public interface ISegmentSdkApiClient
    {
        string FetchSegmentChanges(string name, long since);

        HTTPResult ExecuteGet(string requestUri);
    }
}
