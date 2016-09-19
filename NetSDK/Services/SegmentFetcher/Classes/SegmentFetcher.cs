using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SegmentFetcher: ISegmentFetcher
    {
        protected ISegmentCache segmentCache;

        public SegmentFetcher(ISegmentCache segmentCache)
        {
            this.segmentCache = segmentCache ?? new InMemorySegmentCache(new ConcurrentDictionary<string, Segment>());
        }

        public virtual void InitializeSegment(string name)
        {
        }
    }
}
