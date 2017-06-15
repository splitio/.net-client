using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Metrics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Metrics.Classes
{
    public class RedisMetricsLog : IMetricsLog
    {
        IMetricsCache metricsCache;

        public RedisMetricsLog(IMetricsCache metricsCache)
        {
            this.metricsCache = metricsCache;
        }

        public void Count(string counterName, long delta)
        {
            if (String.IsNullOrEmpty(counterName) || delta <= 0)
            {
                return;
            }

            metricsCache.IncrementCount(counterName, delta);
        }

        public void Time(string operation, long miliseconds)
        {
            if (String.IsNullOrEmpty(operation) || miliseconds < 0)
            {
                return;
            }

            metricsCache.SetLatency(operation, miliseconds);
        }

        public void Gauge(string gauge, long value)
        {
            if (String.IsNullOrEmpty(gauge) || value < 0)
            {
                return;
            }

            metricsCache.SetGauge(gauge, value);
        }


        public void Clear()
        {
            //TODO: implement this
            throw new NotImplementedException();
        }
    }
}
