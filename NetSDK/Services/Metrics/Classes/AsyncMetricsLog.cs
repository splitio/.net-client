using log4net;
using Splitio.Services.Metrics.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Splitio.Services.Metrics.Classes
{
    public class AsyncMetricsLog : IMetricsLog
    {
        IMetricsLog worker;

        protected static readonly ILog Logger = LogManager.GetLogger(typeof(AsyncMetricsLog));

        public AsyncMetricsLog(IMetricsSdkApiClient apiClient, ConcurrentDictionary<string, Counter> countMetrics = null, ConcurrentDictionary<string, ILatencyTracker> timeMetrics = null, ConcurrentDictionary<string, long> gaugeMetrics = null, int maxCountCalls = -1, int maxTimeBetweenCalls = -1)
        {
            worker = new InMemoryMetricsLog(apiClient, countMetrics, timeMetrics, gaugeMetrics, maxCountCalls, maxTimeBetweenCalls);
        }

        public void Count(string counter, long delta)
        {
            try
            {
                var task = new Task(() => worker.Count(counter, delta));
                task.Start();
            }
            catch(Exception e)
            {
                Logger.Error("Exception running count metrics task", e);
            }
        }

        public void Time(string operation, long miliseconds)
        {
            try
            {
                var task = new Task(() => worker.Time(operation, miliseconds));
                task.Start();
            }
            catch(Exception e)
            {
                Logger.Error("Exception running time metrics task", e);
            }
        }

        public void Gauge(string gauge, long value)
        {
            try
            {
                var task = new Task(() => worker.Gauge(gauge, value));
                task.Start();
            }
            catch(Exception e)
            {
                Logger.Error("Exception running gauge metrics task", e);
            }
        }
    }
}
