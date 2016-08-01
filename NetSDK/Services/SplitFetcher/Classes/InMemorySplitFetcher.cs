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
        protected List<Split> splits;

        public InMemorySplitFetcher(List<Split> splits = null)
        {
            this.splits = splits ?? new List<Split>();
        }

        public Split Fetch(string feature)
        {
            return splits.FirstOrDefault(x => x.name == feature);
        }

        public List<Split> FetchAll()
        {
            return splits;
        }
    }
}
