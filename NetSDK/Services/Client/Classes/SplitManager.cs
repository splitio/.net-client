using Splitio.Domain;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class SplitManager : ISplitManager
    {
        ISplitFetcher splitFetcher;

        public SplitManager (ISplitFetcher splitFetcher)
        {
            this.splitFetcher = splitFetcher;
        }

        public List<LightSplit> Splits()
        {
            var currentSplits = splitFetcher.FetchAll();

            var lightSplits = currentSplits.Select(x =>
                new LightSplit()
                {
                    name = x.name,
                    killed = x.killed,
                    changeNumber = x.changeNumber,
                    treatments = x.conditions[0].partitions.Select(y => y.treatment).ToList(),
                    trafficType = x.trafficTypeName
                });

            return lightSplits.ToList();
        }
    }
}
