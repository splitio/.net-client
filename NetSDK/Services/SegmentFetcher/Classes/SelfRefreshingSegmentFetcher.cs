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
        private HashSet<Segment> segments;
        private int interval;

        public SelfRefreshingSegmentFetcher(ISegmentChangeFetcher segmentChangeFetcher, HashSet<Segment> segments = null, int interval = 30)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segments = segments ?? new HashSet<Segment>();
            this.interval = interval;
        }

        public Segment Fetch(string name)
        {
            var segment = segments.FirstOrDefault(x => x.name == name);
            if (segment != null)
            {
                return segment;
            }

            //TODO: finish this
            //segment = new SelfRefreshingSegment(name, segmentChangeFetcher, interval);
            segments.Add(segment);
            //segment.Start();
            return segment;
        }


    }
}
