using log4net;
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
        private int maxCountCalls;
        private int maxTimeBetweenCalls;
        private DateTime utcNowTimestamp = DateTime.UtcNow;
        private DateTime countLastCall;
        private DateTime timeLastCall;
        private DateTime gaugeLastCall;
        private object countMetricsLockObject = new Object();
        private object timeMetricsLockObject = new Object();
        private object gaugeMetricsLockObject = new Object();
        private Boolean sendingCountMetrics = false;
        private Boolean sendingTimeMetrics = false;
        private Boolean sendingGaugeMetrics = false;
        private int gaugeCallCount = 0;


        protected static readonly ILog Logger = LogManager.GetLogger(typeof(InMemoryMetricsLog));

        public InMemoryMetricsLog(IMetricsSdkApiClient apiClient, ConcurrentDictionary<string, Counter> countMetrics = null, ConcurrentDictionary<string, ILatencyTracker> timeMetrics = null, ConcurrentDictionary<string, long> gaugeMetrics = null, int maxCountCalls = 1000, int maxTimeBetweenCalls = 60)
        {
            this.apiClient = apiClient;
            this.countMetrics = countMetrics ?? new ConcurrentDictionary<string, Counter>();
            this.timeMetrics = timeMetrics ?? new ConcurrentDictionary<string, ILatencyTracker>();
            this.gaugeMetrics = gaugeMetrics ?? new ConcurrentDictionary<string, long>();
            this.maxCountCalls = maxCountCalls;
            this.maxTimeBetweenCalls = maxTimeBetweenCalls * 1000;
            this.countLastCall = utcNowTimestamp;
            this.timeLastCall = utcNowTimestamp;
            this.gaugeLastCall = utcNowTimestamp;
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

            Counter counter;

            if (!countMetrics.TryGetValue(counterName, out counter))
            {
                counter = new Counter();
                countMetrics.TryAdd(counterName, counter);
            }

            counter.AddDelta(delta);

            var oldLastCall = countLastCall;
            countLastCall = DateTime.UtcNow;
            if (counter.GetCount() >= maxCountCalls || ((countLastCall - oldLastCall).TotalMilliseconds > maxTimeBetweenCalls))
            {
                SendCountMetrics();
            }

        }

        public void Time(string operation, long miliseconds)
        {
            if (String.IsNullOrEmpty(operation) || miliseconds < 0)
            {
                return;
            }

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
            }
        }

        public void Gauge(string gauge, long value)
        {
            if (String.IsNullOrEmpty(gauge) || value < 0)
            {
                return;
            }

            if (!gaugeMetrics.ContainsKey(gauge))
            {
                gaugeMetrics.TryAdd(gauge, 0);
            }

            gaugeMetrics[gauge] = value;
            gaugeCallCount++;

            var oldLastCall = gaugeLastCall;
            gaugeLastCall = DateTime.UtcNow;
            if (gaugeCallCount >= maxCountCalls || (gaugeLastCall - oldLastCall).TotalMilliseconds > maxTimeBetweenCalls)
            {
                SendGaugeMetrics();
            }
        }


        private void SendCountMetrics()
        {
            lock (countMetricsLockObject)
            {
                if (sendingCountMetrics)
                {
                    return;
                }
                sendingCountMetrics = true;
            }

            var countMetricsJson = ConvertCountMetricsToJson(countMetrics);
            countMetrics.Clear();
            if (countMetricsJson != String.Empty)
            {
                apiClient.SendCountMetrics(countMetricsJson);
            }
            sendingCountMetrics = false;
        }

        private string ConvertCountMetricsToJson(ConcurrentDictionary<string, Counter> countMetrics)
        {
            try
            {
                return JsonConvert.SerializeObject(countMetrics.Select(x => new { name = x.Key, delta = x.Value.GetDelta() }));
            }
            catch(Exception e)
            {
                Logger.Error("Exception ocurred serializing count metrics", e);

                return String.Empty;
            }
        }
        private void SendTimeMetrics()
        {
            lock (timeMetricsLockObject)
            {
                if (sendingTimeMetrics)
                {
                    return;
                }
                sendingTimeMetrics = true;
            }
            var timeMetricsJson = ConvertTimeMetricsToJson(timeMetrics);
            timeMetrics.Clear();
            if (timeMetricsJson != String.Empty)
            {
                apiClient.SendTimeMetrics(timeMetricsJson);
            }
            sendingTimeMetrics = false;
        }

        private string ConvertTimeMetricsToJson(ConcurrentDictionary<string, ILatencyTracker> timeMetrics)
        {
            try
            {
                return JsonConvert.SerializeObject(timeMetrics.Select(x => new { name = x.Key, latencies = x.Value.GetLatencies()}));
            }
            catch (Exception e)
            {
                Logger.Error("Exception ocurred serializing time metrics", e);

                return String.Empty;
            }
        }

        private void SendGaugeMetrics()
        {
            lock (gaugeMetricsLockObject)
            {
                if (sendingGaugeMetrics)
                {
                    return;
                }
                sendingGaugeMetrics = true;
            }

            var gaugeMetricsJson = ConvertGaugeMetricsToJson(gaugeMetrics);
            gaugeMetrics.Clear();
            gaugeCallCount = 0;
            if (gaugeMetricsJson != String.Empty)
            {
                apiClient.SendGaugeMetrics(gaugeMetricsJson);
            }
            sendingGaugeMetrics = false;
        }

        private string ConvertGaugeMetricsToJson(ConcurrentDictionary<string, long> gaugeMetrics)
        {
            try
            {
                return JsonConvert.SerializeObject(gaugeMetrics.Select(x => new { name = x.Key, value = x.Value }));
            }
            catch (Exception e)
            {
                Logger.Error("Exception ocurred serializing gauge metrics", e);

                return String.Empty;
            }
        }
    }
}
