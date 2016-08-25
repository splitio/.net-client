using log4net;
using Splitio.Domain;
using Splitio.Services.Client.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegmentFetcher : ISegmentFetcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegmentFetcher));
        private readonly ISegmentChangeFetcher segmentChangeFetcher;
        private ConcurrentDictionary<string, SelfRefreshingSegment> segments;
        private SdkReadinessGates gates;
        private int interval;

        public SelfRefreshingSegmentFetcher(ISegmentChangeFetcher segmentChangeFetcher, SdkReadinessGates gates, ConcurrentDictionary<string, SelfRefreshingSegment> segments = null, int interval = 30)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segments = segments ?? new ConcurrentDictionary<string, SelfRefreshingSegment>();
            this.interval = interval;
            this.gates = gates;
        }

        public Segment Fetch(string name)
        {
            SelfRefreshingSegment segment;
            segments.TryGetValue(name, out segment);
            if (segment != null)
            {
                return segment;
            }

            segment = new SelfRefreshingSegment(name, segmentChangeFetcher, gates, interval);
            gates.RegisterSegment(name);
            segment.Start();
            segments.TryAdd(name, segment);
            return segment;
        }


    }
}
