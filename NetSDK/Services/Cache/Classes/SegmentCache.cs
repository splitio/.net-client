using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.SegmentFetcher.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Classes
{
    public class SegmentCache : ISegmentCache
    {
        private ConcurrentDictionary<string, Segment> segments;

        public SegmentCache(ConcurrentDictionary<string, Segment> segments)
        {
            this.segments = segments;
        }

        public void RegisterSegment(string segmentName) 
        {
            Segment segment = new Segment(segmentName);
            segments.TryAdd(segmentName, segment);
        }

        public void AddToSegment(string segmentName, List<string> segmentKeys)
        {
            Segment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                segment.AddKeys(segmentKeys);
            }
        }

        public void RemoveFromSegment(string segmentName, List<string> segmentKeys)
        {
            Segment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                segment.RemoveKeys(segmentKeys);
            }
        }

        public bool IsInSegment(string segmentName, string key)
        {
            Segment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                return segment.Contains(key);
            }

            return false;
        }

        public void SetChangeNumber(string segmentName, long changeNumber)
        {
            Segment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                segment.changeNumber = changeNumber;
            }
        }

        public long GetChangeNumber(string segmentName)
        {
            Segment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                return segment.changeNumber;
            }

            return -1;
        }
    }
}
