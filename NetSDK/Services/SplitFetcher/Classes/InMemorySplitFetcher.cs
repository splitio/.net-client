using Splitio.Domain;
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
        protected ConcurrentDictionary<string, ParsedSplit> splits;

        public InMemorySplitFetcher(ConcurrentDictionary<string, ParsedSplit> splits = null)
        {
            this.splits = splits ?? new ConcurrentDictionary<string, ParsedSplit>();
        }

        public ParsedSplit Fetch(string feature)
        {
            ParsedSplit value;
            splits.TryGetValue(feature, out value);
            return value;
        }

        public List<ParsedSplit> FetchAll()
        {
            return splits.Values.ToList(); 
        }
    }
}
