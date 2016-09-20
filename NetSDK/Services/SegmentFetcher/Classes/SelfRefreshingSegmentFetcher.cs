using log4net;
using Splitio.CommonLibraries;
using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegmentFetcher : SegmentFetcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegmentFetcher));
        
        private readonly ISegmentChangeFetcher segmentChangeFetcher;
        private ConcurrentDictionary<string, SelfRefreshingSegment> segmentsThreads;
        private SegmentTaskWorker worker;
        private SdkReadinessGates gates;
        private int interval;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource(); 

        public SelfRefreshingSegmentFetcher(ISegmentChangeFetcher segmentChangeFetcher, SdkReadinessGates gates, int interval, ISegmentCache segmentsCache, int numberOfParallelSegments):base(segmentsCache)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segmentsThreads = new ConcurrentDictionary<string, SelfRefreshingSegment>();
            worker = new SegmentTaskWorker(numberOfParallelSegments); 
            this.interval = interval;
            this.gates = gates;
            StartWorker();
        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
        }

        private void StartWorker()
        {
            Task workerTask = Task.Factory.StartNew(
                () => worker.ExecuteTasks(), 
                cancelTokenSource.Token);
        }

        public void StartScheduler()
        {
            Task schedulerTask = PeriodicTaskFactory.Start(
                    () => AddSegmentsToQueue(),
                    intervalInMilliseconds: interval * 1000,
                    cancelToken: cancelTokenSource.Token);
        }

        public override void InitializeSegment(string name)
        {
            SelfRefreshingSegment segment;
            segmentsThreads.TryGetValue(name, out segment);
            if (segment == null)
            {
                segment = new SelfRefreshingSegment(name, segmentChangeFetcher, gates, segmentCache);
                gates.RegisterSegment(name);
                segmentsThreads.TryAdd(name, segment);
                SegmentTaskQueue.segmentsQueue.TryAdd(segment);
            }
        }

        private void AddSegmentsToQueue()
        {
            foreach (SelfRefreshingSegment segment in segmentsThreads.Values)
            {
                SegmentTaskQueue.segmentsQueue.TryAdd(segment);
                Log.Info(String.Format("Segment queued: {0}", segment.name));
            }
        }
    }
}
