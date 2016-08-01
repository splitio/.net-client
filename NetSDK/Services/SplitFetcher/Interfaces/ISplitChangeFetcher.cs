using NetSDK.Domain;
using NetSDK.Services.SplitFetcher.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.Services.SplitFetcher.Interfaces
{
    public interface ISplitChangeFetcher
    {
        SplitChangesResult Fetch(long since);
    }
}
