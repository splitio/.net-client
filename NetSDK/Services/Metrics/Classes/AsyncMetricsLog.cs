using Splitio.Services.Metrics.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitio.Services.Metrics.Classes
{
    public class AsyncMetricsLog : IMetricsLog
    {
        IMetricsLog worker;

        public AsyncMetricsLog(IMetricsSdkApiClient apiClient, ConcurrentDictionary<string, Counter> countMetrics = null, ConcurrentDictionary<string, ILatencyTracker> timeMetrics = null, ConcurrentDictionary<string, long> gaugeMetrics = null, int maxCountCall = -1, int maxTimeBetweenCalls = -1)
        {
            worker = new InMemoryMetricsLog(apiClient, countMetrics, timeMetrics, gaugeMetrics, maxCountCall, maxTimeBetweenCalls);
        }

        public void Count(string counter, long delta)
        {
            var task = new Task(() => worker.Count(counter, delta));
            task.Start();
        }

        public void Time(string operation, long miliseconds)
        {
            var task = new Task(() => worker.Time(operation, miliseconds));
            task.Start();
        }

        public void Gauge(string gauge, long value)
        {
            var task = new Task(() => worker.Gauge(gauge, value));
            task.Start();
        }
    }
}
