using log4net;
using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegmentFetcher : SegmentFetcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegmentFetcher));
        
        private readonly ISegmentChangeFetcher segmentChangeFetcher;
        private ConcurrentDictionary<string, SelfRefreshingSegment> segmentsThreads;
        private SdkReadinessGates gates;
        private int interval;

        public SelfRefreshingSegmentFetcher(ISegmentChangeFetcher segmentChangeFetcher, SdkReadinessGates gates, int interval, ISegmentCache segmentsCache):base(segmentsCache)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segmentsThreads = new ConcurrentDictionary<string, SelfRefreshingSegment>();
            this.interval = interval;
            this.gates = gates;
        }

        public override void InitializeSegment(string name)
        {
            SelfRefreshingSegment segment;
            segmentsThreads.TryGetValue(name, out segment);
            if (segment == null)
            {
                segment = new SelfRefreshingSegment(name, segmentChangeFetcher, gates, interval, segmentCache);
                gates.RegisterSegment(name);
                segment.Start();
                segmentsThreads.TryAdd(name, segment);
            }
        }


    }
}
