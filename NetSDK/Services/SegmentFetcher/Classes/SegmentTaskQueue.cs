using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public static class SegmentTaskQueue
    {
        //ConcurrentQueue<T> performs best when one dedicated thread is queuing and one dedicated thread is de-queuing
        public static ConcurrentQueue<SelfRefreshingSegment> segmentsQueue = new ConcurrentQueue<SelfRefreshingSegment>();
    }
}
