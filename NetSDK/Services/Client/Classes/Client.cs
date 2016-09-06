using log4net;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Services.Metrics.Interfaces;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class Client: IClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Client));
        private const string Control = "control";
        private const string SdkGetTreatment = "sdk.getTreatment";

        protected Splitter splitter;
        protected ISplitFetcher splitFetcher;
        protected ITreatmentLog treatmentLog;
        protected IMetricsLog metricsLog;
        protected Engine engine;

        public string GetTreatment(string key, string feature, Dictionary<string, object> attributes)
        {
            try
            {
                var split = splitFetcher.Fetch(feature);

                if (split == null)
                {
                    Log.Warn(String.Format("Unknown or invalid feature: {0}", feature));
                    return Control;
                }

                long start = CurrentTimeMillis();

                var treatment = engine.GetTreatment(key, split, attributes);

                RecordStats(key, feature, start, treatment, SdkGetTreatment);

                return treatment;
            }
            catch(Exception e)
            {
                Log.Error(String.Format("Exception caught getting treatment for feature: {0}", feature), e);
                return Control;
            }
        }

        private long CurrentTimeMillis()
        {
            var Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        private void RecordStats(string key, string feature, long start, string treatment, string operation)
        {
            var end = CurrentTimeMillis();

            if (metricsLog != null)
            {
                metricsLog.Time(SdkGetTreatment, end - start);
            }

            if (treatmentLog != null)
            {
                treatmentLog.Log(key, feature, treatment, start);
            }
        }
    }
}
