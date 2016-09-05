using Newtonsoft.Json;
using Splitio.Services.Metrics.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Metrics.Classes
{
    public class InMemoryMetricsLog : IMetricsLog
    {
        IMetricsSdkApiClient apiClient;
        private ConcurrentDictionary<string, Counter> countMetrics;
        private ConcurrentDictionary<string, ILatencyTracker> timeMetrics;
        private ConcurrentDictionary<string, long> gaugeMetrics;
        private int maxCountCall;
        private int maxTimeBetweenCalls;
        private DateTime utcNowTimestamp = DateTime.UtcNow;
        private DateTime countLastCall;
        private DateTime timeLastCall;
        private DateTime gaugeLastCall;
        private object countMetricsLockObject = new Object();
        private object timeMetricsLockObject = new Object();
        private object gaugeMetricsLockObject = new Object();


        public InMemoryMetricsLog(IMetricsSdkApiClient apiClient, ConcurrentDictionary<string, Counter> countMetrics = null, ConcurrentDictionary<string, ILatencyTracker> timeMetrics = null, ConcurrentDictionary<string, long> gaugeMetrics = null, int maxCountCall = -1, int maxTimeBetweenCalls = -1)
        {
            this.apiClient = apiClient;
            this.countMetrics = countMetrics ?? new ConcurrentDictionary<string, Counter>();
            this.timeMetrics = timeMetrics ?? new ConcurrentDictionary<string, ILatencyTracker>();
            this.gaugeMetrics = gaugeMetrics ?? new ConcurrentDictionary<string, long>();
            this.maxCountCall = maxCountCall;
            this.maxTimeBetweenCalls = maxTimeBetweenCalls;
            this.countLastCall = utcNowTimestamp;
            this.timeLastCall = utcNowTimestamp;
            this.gaugeLastCall = utcNowTimestamp;
        }

        public ConcurrentDictionary<string, Counter> FetchCountMetricsAndClear()
        {
            lock (countMetricsLockObject)
            {
                var existingCountMetrics = new ConcurrentDictionary<string, Counter>(countMetrics);
                countMetrics = new ConcurrentDictionary<string, Counter>();
                return existingCountMetrics;
            }
        }

        public ConcurrentDictionary<string, ILatencyTracker> FetchTimeMetricsAndClear()
        {
            lock (timeMetricsLockObject)
            {
                var existingTimeMetrics = new ConcurrentDictionary<string, ILatencyTracker>(timeMetrics);
                timeMetrics = new ConcurrentDictionary<string, ILatencyTracker>();
                return existingTimeMetrics;
            }
        }

        public ConcurrentDictionary<string, long> FetchGaugeMetricsAndClear()
        {
            lock (gaugeMetricsLockObject)
            {
                var existingGaugeMetrics = new ConcurrentDictionary<string, long>(gaugeMetrics);
                gaugeMetrics = new ConcurrentDictionary<string, long>();
                return existingGaugeMetrics;
            }
        }

        public void Count(string counterName, long delta)
        {
            if (delta <= 0)
            {
                return;
            }

            if (String.IsNullOrEmpty(counterName))
            {
                return;
            }

            lock (countMetricsLockObject)
            {
                Counter counter;

                if (!countMetrics.TryGetValue(counterName, out counter))
                {
                    counter = new Counter();
                    countMetrics.TryAdd(counterName, counter);
                }

                counter.AddDelta(delta);

                var oldLastCall = countLastCall;
                countLastCall = DateTime.UtcNow;
                if (counter.GetCount() > maxCountCall || ((countLastCall - oldLastCall).TotalMilliseconds > maxTimeBetweenCalls))
                {
                    SendCountMetrics();
                    counter.Clear();
                }
            }
        }

        public void Time(string operation, long miliseconds)
        {
            if (String.IsNullOrEmpty(operation) || miliseconds < 0)
            {
                return;
            }
            lock (timeMetricsLockObject)
            {
                ILatencyTracker tracker;

                if (!timeMetrics.TryGetValue(operation, out tracker))
                {
                    tracker = new BinarySearchLatencyTracker();
                    timeMetrics.TryAdd(operation, tracker);
                }

                tracker.AddLatencyMillis((int)miliseconds);

                var oldLastCall = timeLastCall;
                timeLastCall = DateTime.UtcNow;
                if ((timeLastCall - oldLastCall).TotalMilliseconds > maxTimeBetweenCalls)
                {
                    SendTimeMetrics();
                    tracker.Clear();
                }
            }
        }

        public void Gauge(string gauge, long value)
        {
            throw new NotImplementedException();
        }

        private void SendCountMetrics()
        {
            var countMetrics = FetchCountMetricsAndClear();
            var counterMetricsJson = ConvertCountMetricsToJson(countMetrics);
            apiClient.SendCountMetrics(counterMetricsJson);
        }

        private string ConvertCountMetricsToJson(ConcurrentDictionary<string, Counter> countMetrics)
        {
            return JsonConvert.SerializeObject(countMetrics.Select(x => new { name = x.Key, delta = x.Value.GetDelta() }));
        }
        private void SendTimeMetrics()
        {
            var timeMetrics = FetchTimeMetricsAndClear();
            var counterMetricsJson = ConvertTimeMetricsToJson(timeMetrics);
            apiClient.SendTimeMetrics(counterMetricsJson);
        }

        private string ConvertTimeMetricsToJson(ConcurrentDictionary<string, ILatencyTracker> timeMetrics)
        {
            return JsonConvert.SerializeObject(timeMetrics.Select(x => new { name = x.Key, latencies = x.Value.GetLatencies()}));
        }
    }
}
