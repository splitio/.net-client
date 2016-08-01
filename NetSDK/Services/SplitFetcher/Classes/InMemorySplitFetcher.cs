using NetSDK.Domain;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSDK.Services.SplitFetcher.Classes
{
    public class InMemorySplitFetcher: ISplitFetcher
    {
        protected Dictionary<string, Split> splits;

        public InMemorySplitFetcher(Dictionary<string, Split> splits = null)
        {
            this.splits = splits ?? new Dictionary<string, Split>();
        }

        public Split Fetch(string feature)
        {
            Split value;
            splits.TryGetValue(feature, out value);
            return value;
        }

        public List<Split> FetchAll()
        {
            return splits.Values.ToList(); 
        }
    }
}
