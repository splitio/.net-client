using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public static class SegmentTaskQueue
    {
        public static ConcurrentQueue<SelfRefreshingSegment> segmentsQueue = new ConcurrentQueue<SelfRefreshingSegment>();
    }
}
