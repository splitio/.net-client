using log4net;
using NetSDK.Domain;
using NetSDK.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegmentFetcher : ISegmentFetcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegmentFetcher));
        private readonly ISegmentChangeFetcher segmentChangeFetcher;
        private HashSet<SelfRefreshingSegment> segments;
        private int interval;

        public SelfRefreshingSegmentFetcher(ISegmentChangeFetcher segmentChangeFetcher, HashSet<SelfRefreshingSegment> segments = null, int interval = 30)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segments = segments ?? new HashSet<SelfRefreshingSegment>();
            this.interval = interval;
        }

        public Segment Fetch(string name)
        {
            var segment = segments.FirstOrDefault(x => x.name == name);
            if (segment != null)
            {
                return segment;
            }

            segment = new SelfRefreshingSegment(name, segmentChangeFetcher, interval);
            segment.Start();
            segments.Add(segment);
            return segment;
        }


    }
}
