using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Metrics.Interfaces
{
    public interface IMetricsSdkApiClient
    {
        void SendCountMetrics(string metrics);

        void SendTimeMetrics(string metrics);

        void SendGaugeMetrics(string metrics);
    }
}
