using log4net;
using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegment
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegment));

        private string name;
        private SdkReadinessGates gates;
        private ISegmentChangeFetcher segmentChangeFetcher;
        private ISegmentCache segmentCache;
        private int interval;
        public bool stopped { get; private set; }

        public SelfRefreshingSegment(string name, ISegmentChangeFetcher segmentChangeFetcher, SdkReadinessGates gates, int interval, ISegmentCache segmentCache)
        {
            this.name = name;
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segmentCache = segmentCache;
            this.interval = interval;
            this.stopped = true;
            this.gates = gates;
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
                RefreshSegment();
                Thread.Sleep(interval * 1000);
            }
        }


        private void RefreshSegment()
        {        
            while (true)
            {
                try
                {
                    var changeNumber = segmentCache.GetChangeNumber(name);
                    var response = segmentChangeFetcher.Fetch(name, changeNumber);
                    if (response == null)
                    {
                        return;
                    }
                    if (changeNumber >= response.till)
                    {
                        gates.SegmentIsReady(name);
                        return;
                    }

                    if (response.added.Count() > 0 || response.removed.Count() > 0)
                    {

                        segmentCache.AddToSegment(name, response.added);
                        segmentCache.RemoveFromSegment(name, response.removed);

                        if (response.added.Count() > 0)
                        {
                            Log.Info(String.Format("Segment {0} - Added : {1}", name, String.Join(" - ", response.added)));
                        }
                        if (response.removed.Count() > 0)
                        {
                            Log.Info(String.Format("Segment {0} - Removed : {1}", name, String.Join(" - ", response.removed)));
                        }
                    }

                    segmentCache.SetChangeNumber(name, response.till);                  
                }
                catch (Exception e)
                {
                    Log.Error("Exception caught refreshing segment", e);
                    stopped = true;
                }               
            }
        }
    }
}
