using NetSDK.Domain;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetSDK.Services.SplitFetcher.Classes
{
    public class SelfRefreshingSplitFetcher: InMemorySplitFetcher
    {
        private readonly ISplitChangeFetcher splitChangeFetcher;
        private readonly SplitParser splitParser;
        private int interval;
        private bool greedy;
        private long change_number;
        public bool stopped { get; private set; }

        public SelfRefreshingSplitFetcher(ISplitChangeFetcher splitChangeFetcher, SplitParser splitParser, int interval = 30, bool greedy = true,
                 long change_number = -1, List<Split> splits = null) : base(splits)
        {
            this.splitChangeFetcher = splitChangeFetcher;
            this.splitParser = splitParser;
            this.interval = interval;
            this.greedy = greedy;
            this.change_number = change_number;
            this.stopped = true;   
        }

        public void Start()
        {
            Thread thread = new Thread(StartRefreshing);
            thread.Start();
        }

        public void Stop()
        {
            stopped = true;
        }

        private void StartRefreshing()
        {
            if (!stopped)
            {
                return;
            }

            stopped = false;

            while(!stopped)
            {
                RefreshSplits();
                Thread.Sleep(interval);
            }
        }


        private List<T> Clone<T>( List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        

        private void UpdateSplitsFromChangeFetcherResponse(List<Split> splitChanges)
        {
            List<Split> addedSplits = new List<Split>();
            List<Split> removedSplits = new List<Split>();

            var tempSplits = Clone<Split>(splits);

            foreach (Split split in splitChanges)
            {
                //If not active --> Remove Split
                if (split.status != StatusEnum.ACTIVE)
                {
                    tempSplits.RemoveAll(x => x.name == split.name);
                    removedSplits.Add(split);
                }
                else
                {
                    //Test if its a new Split, remove if existing
                    int removedCount = tempSplits.RemoveAll(x => x.name == split.name);

                    if (removedCount == 0)
                    {
                        //If not existing in _splits, its a new split
                        addedSplits.Add(split);
                    }

                    tempSplits.Add(split);
                }
            }
            splits = tempSplits;
            //TODO: log addedSplits if count > 0
            //TODO: log removedSplits if count > 0
        }

        private void RefreshSplits()
        {
            var changeNumberBefore = change_number;
            try
            {
                var result = splitChangeFetcher.Fetch(change_number);
                if (change_number >= result.till)
                {
                    //There are no new split changes
                    return;
                }
                if (result.splits != null && result.splits.Count > 0)
                {
                    UpdateSplitsFromChangeFetcherResponse(result.splits);
                    change_number = result.till;
                }
                if (!greedy)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                //TODO: log exception Exception caught refreshing splits
                stopped = true;
            }
            finally
            {
                //TODO: log split fetch before: %s, after: %s', change_number_before, self._change_number
            }
        }

    }
}
