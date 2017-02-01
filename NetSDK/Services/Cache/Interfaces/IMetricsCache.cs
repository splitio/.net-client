using Splitio.Services.Metrics.Classes;
using Splitio.Services.Metrics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Interfaces
{
    public interface IMetricsCache
    {
        void SetCount(string name, Counter counter);
        void IncrementCount(Counter counter, long delta);
        Counter GetCount(string name);
        Dictionary<string, Counter> FetchAllCountersAndClear();

        void SetGauge(string name, long gauge);
        long GetGauge(string name);
        Dictionary<string, long> FetchAllGaugesAndClear();

        void SetLatencyBucketCounter(string name, ILatencyTracker latencyTracker);
        void IncrementLatencyBucketCounter(ILatencyTracker latencyTracker, long delta);
        ILatencyTracker GetLatencyTracker(string name);
        Dictionary<string, ILatencyTracker> FetchAllLatencyTrackersAndClear();
    }
}
