﻿using log4net;
using NetSDK.Domain;
using NetSDK.Services.Parsing;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSplitFetcher));
        private readonly ISplitChangeFetcher splitChangeFetcher;
        private readonly SplitParser splitParser;
        private int interval;
        private long change_number;
        public bool stopped { get; private set; }
        private bool splitsInitialized;
        public bool initialized
        {
            get
            {
                return splitsInitialized && splits.All(x => x.Value.initialized);
            }
        }

        public SelfRefreshingSplitFetcher(ISplitChangeFetcher splitChangeFetcher, SplitParser splitParser, int interval = 30,
                 long change_number = -1, Dictionary<string, ParsedSplit> splits = null)
            : base(splits)
        {
            this.splitChangeFetcher = splitChangeFetcher;
            this.splitParser = splitParser;
            this.interval = interval;
            this.change_number = change_number;
            this.stopped = true;
            this.splitsInitialized = false;
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

        private void UpdateSplitsFromChangeFetcherResponse(List<Split> splitChanges)
        {
            List<Split> addedSplits = new List<Split>();
            List<Split> removedSplits = new List<Split>();

            var tempSplits = new Dictionary<string, ParsedSplit>(splits);

            foreach (Split split in splitChanges)
            {
                //If not active --> Remove Split
                if (split.status != StatusEnum.ACTIVE)
                {
                    tempSplits.Remove(split.name);
                    removedSplits.Add(split);
                }
                else
                {
                    //Test if its a new Split, remove if existing
                    bool isRemoved = tempSplits.Remove(split.name);

                    if (!isRemoved)
                    {
                        //If not existing in _splits, its a new split
                        addedSplits.Add(split);
                    }
                    ParsedSplit parsedSplit = splitParser.Parse(split);
                    tempSplits.Add(parsedSplit.name, parsedSplit);
                }
            }
            splits = tempSplits;

            if(addedSplits.Count() > 0)
            {
               var addedFeatureNames = addedSplits.Select(x => x.name).ToList();
               Log.Info(String.Format("Added features: {0}", String.Join(" - ", addedFeatureNames)));
            }
            if(removedSplits.Count() > 0)
            {
               var removedFeatureNames = removedSplits.Select(x => x.name).ToList();
               Log.Info(String.Format("Deleted features: {0}", String.Join(" - ", removedFeatureNames)));
            }
        }

        private void RefreshSplits()
        {
            var changeNumberBefore = change_number;
            try
            {
                var result = splitChangeFetcher.Fetch(change_number);
                if (result == null)
                {
                    return;
                }
                if (change_number >= result.till)
                {
                    if (!splitsInitialized)
                    {
                        splitsInitialized = true;
                    }
                    //There are no new split changes
                    return;
                }
                if (result.splits != null && result.splits.Count > 0)
                {
                    UpdateSplitsFromChangeFetcherResponse(result.splits);
                    change_number = result.till;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception caught refreshing splits", e);
                stopped = true;
            }
            finally
            {
                Log.Info(String.Format("split fetch before: {0}, after: {1}", changeNumberBefore, change_number));
            }
        }

    }
}