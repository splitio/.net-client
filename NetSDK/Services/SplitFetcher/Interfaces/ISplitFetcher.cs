using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.Services.SplitFetcher.Interfaces
{
    public interface ISplitFetcher
    {
        ParsedSplit Fetch(String feature);
        List<ParsedSplit> FetchAll();
    }
}
