using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitio.Services.SplitFetcher.Classes
{
    public class InMemorySplitFetcher: ISplitFetcher
    {
        protected ISplitCache splitCache;
        public InMemorySplitFetcher(ISplitCache splitCache)
        {
            this.splitCache = splitCache ?? new SplitCache(new ConcurrentDictionary<string, ParsedSplit>());
        }

        public ParsedSplit Fetch(string feature)
        {
            return splitCache.GetSplit(feature);
        }

        public List<ParsedSplit> FetchAll()
        {
            return splitCache.GetAllSplits();
        }
    }
}
