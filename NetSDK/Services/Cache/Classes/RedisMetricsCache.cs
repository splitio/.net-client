using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Metrics.Classes;
using Splitio.Services.Metrics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Splitio.Services.Cache.Classes
{
    public class RedisMetricsCache : IMetricsCache
    {
        private IRedisAdapter redisAdapter;
        private ILatencyTracker latencyTracker;
        private const string metricsLatencyKeyPrefix = "SPLITIO.latency.{metricName}.bucket.{bucketNumber}";
        private const string metricsCountKeyPrefix = "SPLITIO.count.";
        private const string metricsGaugeKeyPrefix = "SPLITIO.gauge.";

        public RedisMetricsCache(IRedisAdapter redisAdapter)
        {
            this.redisAdapter = redisAdapter;
            this.latencyTracker = new BinarySearchLatencyTracker();
        }

        public Counter IncrementCount(string name, long delta)
        {
            var key = metricsCountKeyPrefix + name;
            var result = redisAdapter.IcrBy(key, delta);

            var counter = new Counter(); //TODO: counter.count is losing its original value!!
            counter.AddDelta(result);

            return counter;
        }

        public Counter GetCount(string name)
        {
            var key = metricsCountKeyPrefix + name;
            var result = redisAdapter.Get(key);
            var counter = new Counter(); //TODO: counter.count is losing its original value!!
            counter.AddDelta(long.Parse(result));

            return counter;
        }

        public Dictionary<string, Counter> FetchAllCountersAndClear()
        {
            var result = new Dictionary<string, Counter>();
            var pattern = metricsCountKeyPrefix + "*";
            var keys = redisAdapter.Keys(pattern);
            foreach (var count in keys)
            {
                var value = redisAdapter.Get(count);
                var countName = ((string)count).Replace(metricsCountKeyPrefix, "");
                var counterValue = long.Parse(redisAdapter.Get(metricsCountKeyPrefix + countName));
                result.Add(countName, new Counter(counterValue)); //TODO: counter.count is losing its original value!!
                redisAdapter.Del(count);
            }
            return result;
        }

        public void SetGauge(string name, long gauge)
        {
            var key = metricsGaugeKeyPrefix + name;
            redisAdapter.Set(key, gauge.ToString());
        }

        public long GetGauge(string name)
        {
            var key = metricsGaugeKeyPrefix + name;
            string value = redisAdapter.Get(key);

            return long.Parse(value);
        }

        public Dictionary<string, long> FetchAllGaugesAndClear()
        {
            var result = new Dictionary<string, long>();
            var pattern = metricsGaugeKeyPrefix + "*";
            var keys = redisAdapter.Keys(pattern);
            foreach (var gauge in keys)
            {
                var value = redisAdapter.Get(gauge);
                var gaugeName = ((string)gauge).Replace(metricsGaugeKeyPrefix, "");
                result.Add(gaugeName, long.Parse(value));
                redisAdapter.Del(gauge);
            }
            return result;
        }

        public void SetLatency(string name, long value)
        {
            var bucketToIncrement = ((BinarySearchLatencyTracker)latencyTracker).FindIndex(value * 1000);
            var key = metricsLatencyKeyPrefix.Replace("{metricName}", name).Replace("{bucketNumber}", bucketToIncrement.ToString());
            var result = redisAdapter.IcrBy(key, 1);
        }

        public ILatencyTracker GetLatencyTracker(string name)
        {
            var bucketsPattern = metricsLatencyKeyPrefix.Replace("{metricName}", name).Replace("{bucketNumber}", "*");
            var keys = redisAdapter.Keys(bucketsPattern);
            ILatencyTracker result = new BinarySearchLatencyTracker();
            foreach (var key in keys)
            {
               var bucketPrefix = bucketsPattern.Replace("*", "");
               var bucket = ((string)key).Replace(bucketPrefix, "");
               var currentBucketPattern = bucketsPattern.Replace("*", bucket);
               var valueString = redisAdapter.Get(currentBucketPattern);
               long value = long.Parse(valueString);
               result.SetLatencyCount(int.Parse(bucket), value);
            }
            return result;
        }

        public Dictionary<string, ILatencyTracker> FetchAllLatencyTrackersAndClear()
        {
            var result = new Dictionary<string, ILatencyTracker>();
            var pattern = metricsLatencyKeyPrefix.Replace(".{metricName}.bucket.{bucketNumber}", "*");
            var keys = redisAdapter.Keys(pattern);
            foreach(var key in keys)
            {
                var keyParts = ((string)key).Split('.');
                var latencyPosition = Array.IndexOf(keyParts, "latency");
                ILatencyTracker tracker = GetLatencyTracker(keyParts[latencyPosition + 1]);
                result.Add(keyParts[latencyPosition + 1], tracker);
            }
            return result;
        }      
    }
}
