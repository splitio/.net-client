using Splitio.CommonLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SplitFetcher.Interfaces
{
    public interface ISplitSdkApiClient
    {
        string FetchSplitChanges(long since);
    }
}
