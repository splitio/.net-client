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
        public bool initialized { get; private set; }
        public object initializedLock;
        public bool stopped { get; private set; }

        public SelfRefreshingSegment(string name, ISegmentChangeFetcher segmentChangeFetcher, int interval, long change_number = -1) : base(name, change_number)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.interval = interval;
            this.stopped = true;
            this.initialized = false;
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
                    if (response == null)
                    {
                        return;
                    }
                    if (change_number >= response.till)
                    {
                        if (!initialized)
                        {
                            lock (initializedLock)
                            {
                                initialized = true;
                            }
                            foreach (var notificationFlag in notificationFlags)
                            {
                                notificationFlag.Signal();
                                notificationFlags.Remove(notificationFlag);
                            }
                            Log.Debug(name + " segment initialized");
                        }
                        return;
                    }

                    if (response.added.Count() > 0 || response.removed.Count() > 0)
                    {
                        var tempKeys = new HashSet<string>(keys);

                        tempKeys.UnionWith(response.added);
                        tempKeys.ExceptWith(response.removed);

                        keys = tempKeys;

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
