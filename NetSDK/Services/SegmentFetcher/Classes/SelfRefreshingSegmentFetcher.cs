﻿using Common.Logging;
using Splitio.CommonLibraries;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Classes;
using Splitio.Services.SegmentFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SelfRefreshingSegmentFetcher : SegmentFetcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelfRefreshingSegmentFetcher));
        
        private readonly ISegmentChangeFetcher segmentChangeFetcher;
        private ConcurrentDictionary<string, SelfRefreshingSegment> segments;
        private SegmentTaskWorker worker;
        private IReadinessGatesCache gates;
        private int interval;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource(); 

        public SelfRefreshingSegmentFetcher(ISegmentChangeFetcher segmentChangeFetcher, IReadinessGatesCache gates, int interval, ISegmentCache segmentsCache, int numberOfParallelSegments):base(segmentsCache)
        {
            this.segmentChangeFetcher = segmentChangeFetcher;
            this.segments = new ConcurrentDictionary<string, SelfRefreshingSegment>();
            worker = new SegmentTaskWorker(numberOfParallelSegments); 
            this.interval = interval;
            this.gates = gates;
            StartWorker();
        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
            SegmentTaskQueue.segmentsQueue.Dispose();
            segments.Clear();
            segmentCache.Clear();
        }

        private void StartWorker()
        {
            Task workerTask = Task.Factory.StartNew(
                () => worker.ExecuteTasks(cancelTokenSource.Token), 
                cancelTokenSource.Token);
        }

        public void StartScheduler()
        {
            //Delay first execution until expected time has passed
            Thread.Sleep(interval * 1000);
            Task schedulerTask = PeriodicTaskFactory.Start(
                    () => AddSegmentsToQueue(),
                    intervalInMilliseconds: interval * 1000,
                    cancelToken: cancelTokenSource.Token);
        }

        public override void InitializeSegment(string name)
        {
            var segment = new SelfRefreshingSegment(name, segmentChangeFetcher, gates, segmentCache);
            if (segments.TryAdd(name, segment))
            {
                segment.RegisterSegment();
                SegmentTaskQueue.segmentsQueue.TryAdd(segment);
                Log.Info(String.Format("Segment queued: {0}", segment.name));
            }
        }

        private void AddSegmentsToQueue()
        {
            foreach (SelfRefreshingSegment segment in segments.Values)
            {
                SegmentTaskQueue.segmentsQueue.TryAdd(segment);
                Log.Info(String.Format("Segment queued: {0}", segment.name));
            }
        }
    }
}
