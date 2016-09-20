using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitio.Services.SegmentFetcher.Classes
{
    public class SegmentTaskWorker
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SegmentTaskWorker));

        int numberOfParallelTasks;
        int counter;

        public SegmentTaskWorker(int numberOfParallelTasks)
        {
            this.numberOfParallelTasks = numberOfParallelTasks;
            this.counter = 0;
        }

        private void IncrementCounter()
        {
            counter++;
        }

        private void DecrementCounter()
        {
            counter--;
        }

        public void ExecuteTasks()
        {
            while (true)
            {
                if (counter < numberOfParallelTasks)
                {
                    SelfRefreshingSegment segment;
                    //Wait indefinitely until a segment is queued
                    if (SegmentTaskQueue.segmentsQueue.TryTake(out segment, -1))
                    {
                        Log.Info(String.Format("Segment dequeued: {0}", segment.name));
                        IncrementCounter();
                        Task task = new Task(() => segment.RefreshSegment());
                        task.ContinueWith((x) => { DecrementCounter(); }); 
                        task.Start();                   
                    }
                }
            }
        }
    }
}
