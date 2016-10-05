using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
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
        ISplitCache splitCache;

        public SplitManager(ISplitCache splitCache)
        {
            this.splitCache = splitCache;
        }

        public List<SplitView> Splits()
        {
            if (splitCache == null)
            {
                return null;
            }

            var currentSplits = splitCache.GetAllSplits();

            var lightSplits = currentSplits.Select(x =>
                new SplitView()
                {
                    name = x.name,
                    killed = x.killed,
                    changeNumber = x.changeNumber,
                    treatments = x.conditions[0].partitions.Select(y => y.treatment).ToList(),
                    trafficType = x.trafficTypeName
                });

            return lightSplits.ToList();
        }


        public SplitView Split(string featureName)
        {
            if (splitCache == null)
            {
                return null;
            }

            var split = splitCache.GetSplit(featureName);

            var lightSplit = new SplitView()
                {
                    name = split.name,
                    killed = split.killed,
                    changeNumber = split.changeNumber,
                    treatments = split.conditions[0].partitions.Select(y => y.treatment).ToList(),
                    trafficType = split.trafficTypeName
                };

            return lightSplit;
        }
    }
}
