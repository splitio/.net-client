﻿using log4net;
using Splitio.Domain;
using Splitio.Services.Client.Classes;
using Splitio.Services.Parsing;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Splitio.Services.SplitFetcher.Classes
{
    public class SelfRefreshingSplitFetcher : InMemorySplitFetcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSplitFetcher));
        private readonly ISplitChangeFetcher splitChangeFetcher;
        private readonly SplitParser splitParser;
        private int interval;
        private long change_number;
        private bool stopped;
        private SdkReadinessGates gates;


        public SelfRefreshingSplitFetcher(ISplitChangeFetcher splitChangeFetcher, SplitParser splitParser, SdkReadinessGates gates, int interval = 30,
                 long change_number = -1, ConcurrentDictionary<string, ParsedSplit> splits = null)
            : base(splits)
        {
            this.splitChangeFetcher = splitChangeFetcher;
            this.splitParser = splitParser;
            this.gates = gates;
            this.interval = interval;
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

            while (!stopped)
            {
                RefreshSplits();
                Thread.Sleep(interval);
            }
        }

        private void UpdateSplitsFromChangeFetcherResponse(List<Split> splitChanges)
        {
            List<Split> addedSplits = new List<Split>();
            List<Split> removedSplits = new List<Split>();

            var tempSplits = new ConcurrentDictionary<string, ParsedSplit>(splits);

            foreach (Split split in splitChanges)
            {
                ParsedSplit parsedSplit;
                //If not active --> Remove Split
                if (split.status != StatusEnum.ACTIVE)
                {                    
                    tempSplits.TryRemove(split.name, out parsedSplit);
                    removedSplits.Add(split);
                }
                else
                {
                    //Test if its a new Split, remove if existing
                    bool isRemoved = tempSplits.TryRemove(split.name, out parsedSplit);

                    if (!isRemoved)
                    {
                        //If not existing in _splits, its a new split
                        addedSplits.Add(split);
                    }
                    parsedSplit = splitParser.Parse(split);
                    tempSplits.TryAdd(parsedSplit.name, parsedSplit);
                }
            }
            splits = tempSplits;

            if (addedSplits.Count() > 0)
            {
                var addedFeatureNames = addedSplits.Select(x => x.name).ToList();
                Log.Info(String.Format("Added features: {0}", String.Join(" - ", addedFeatureNames)));
            }
            if (removedSplits.Count() > 0)
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
                    gates.SplitsAreReady();
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
