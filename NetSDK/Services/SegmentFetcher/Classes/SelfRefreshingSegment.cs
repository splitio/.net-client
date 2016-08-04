using log4net;
using NetSDK.Domain;
using NetSDK.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetSDK.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegment: Segment
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegment));
        private ISegmentChangeFetcher segmentChangeFetcher;
        private int interval;
        private bool greedy;
        public bool stopped { get; private set; }

        public SelfRefreshingSegment(string name, ISegmentChangeFetcher segmentChangeFetcher, int interval, long change_number = -1, bool greedy = true) : base(name, change_number)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.interval = interval;
            this.greedy = greedy;
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
                RefreshSegment();
                Thread.Sleep(interval);
            }
        }

        private void RefreshSegment()
        {
            while (true)
            {
                try
                {
                    var response = segmentChangeFetcher.Fetch(name, change_number);
                    if (change_number > response.till)
                    {
                        return;
                    }

                    if (response.added.Count() > 0 || response.removed.Count() > 0)
                    {
                        keys.UnionWith(response.added);
                        keys.ExceptWith(response.removed);

                        if (response.added.Count() > 0)
                        {
                            Log.Info(String.Format("Added : {0}", String.Join(" - ", response.added)));
                        }
                        if (response.removed.Count() > 0)
                        {
                            Log.Info(String.Format("Removed : {0}", String.Join(" - ", response.removed)));
                        }

                    }

                    change_number = response.till;

                    if (!greedy)
                    {
                        return;
                    }
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
