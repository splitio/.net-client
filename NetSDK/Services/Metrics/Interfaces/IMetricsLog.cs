using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Metrics.Interfaces
{
    public interface IMetricsLog
    {
        void Count(String counter, long delta);
        void Time(String operation, long miliseconds);
        void Gauge(String gauge, long value);
    }
}
