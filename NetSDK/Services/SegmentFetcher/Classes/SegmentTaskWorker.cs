﻿using Common.Logging;
using System;
using System.Threading;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SegmentTaskWorker
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentTaskWorker));

        private readonly int numberOfParallelTasks;
        private int counter;

        //Worker is always one task, so when it is signaled, after the
        //task stops its wait, this variable is auto-reseted
        private AutoResetEvent waitForExecution = new AutoResetEvent(false);

        public SegmentTaskWorker(int numberOfParallelTasks)
        {
            this.numberOfParallelTasks = numberOfParallelTasks;
            this.counter = 0;
        }

        private void IncrementCounter()
        {
            Interlocked.Increment(ref counter);
        }

        private void DecrementCounter()
        {
            Interlocked.Decrement(ref counter);
            waitForExecution.Set();
        }

        public void ExecuteTasks(CancellationToken token)
        {
            while (true)
            {
                if (counter < numberOfParallelTasks)
                {
                    SelfRefreshingSegment segment;
                    //Wait indefinitely until a segment is queued
                    if (SegmentTaskQueue.segmentsQueue.TryTake(out segment, -1))
                    {
                        if (Log.IsDebugEnabled)
                        {
                            Log.Debug(String.Format("Segment dequeued: {0}", segment.name));
                        }

                        IncrementCounter();
                        segment.RefreshSegment(token).ContinueWith((x) => { DecrementCounter(); });
                    }
                }
                else
                {
                    waitForExecution.WaitOne();
                }
            }
        }
    }
}
