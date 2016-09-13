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
        private ConcurrentDictionary<string, SelfRefreshingSegment> segments;

        public void AddToSegment(string segmentName, HashSet<string> segmentKeys)
        {
            SelfRefreshingSegment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                segment.AddKeys(segmentKeys);
            }
        }

        public void RemoveFromSegment(string segmentName, HashSet<string> segmentKeys)
        {
            SelfRefreshingSegment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                segment.RemoveKeys(segmentKeys);
            }
        }

        public bool IsInSegment(string segmentName, string key)
        {
            SelfRefreshingSegment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                return segment.Contains(key);
            }

            return false;
        }

        public void SetChangeNumber(string segmentName, long changeNumber)
        {
            SelfRefreshingSegment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                segment.changeNumber = changeNumber;
            }
        }

        public long GetChangeNumber(string segmentName)
        {
            SelfRefreshingSegment segment;
            if (segments.TryGetValue(segmentName, out segment))
            {
                return segment.changeNumber;
            }

            return -1;
        }
    }
}
