using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Metrics.Interfaces
{
    public interface ILatencyTracker
    {

        void AddLatencyMillis(long millis);

        void AddLatencyMicros(long micros);

        long[] GetLatencies();

        long GetLatency(int index);

        void Clear();

        long GetBucketForLatencyMillis(long latency);

        long GetBucketForLatencyMicros(long latency);

    }
}
