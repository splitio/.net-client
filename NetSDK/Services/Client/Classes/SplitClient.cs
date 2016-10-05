using log4net;
using Splitio.CommonLibraries;
using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Services.Metrics.Interfaces;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public abstract class SplitClient: ISplitClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplitClient));
        private const string Control = "control";
        private const string SdkGetTreatment = "sdk.getTreatment";

        protected Splitter splitter;
        protected ITreatmentLog treatmentLog;
        protected IMetricsLog metricsLog;
        protected Engine engine;
        protected ISplitManager manager;
        protected ISplitCache splitCache;
        protected ISegmentCache segmentCache;

        public ISplitManager GetSplitManager()
        {
            return manager;
        }

        public string GetTreatment(string key, string feature, Dictionary<string, object> attributes = null)
        {
            Key keys = new Key(key, key);
            return GetTreatmentForFeature(keys, feature, attributes);
        }

        public string GetTreatment(Key key, string feature, Dictionary<string, object> attributes = null)
        {
            return GetTreatmentForFeature(key, feature, attributes);
        }


        private void RecordStats(Key key, string feature, long start, string treatment, string operation, Stopwatch clock)
        {
            if (metricsLog != null)
            {
                metricsLog.Time(SdkGetTreatment, clock.ElapsedMilliseconds);
            }

            if (treatmentLog != null)
            {
                treatmentLog.Log(key.matchingKey, feature, treatment, start, key.bucketingKey);
            }
        }

        private string GetTreatmentForFeature(Key key, string feature, Dictionary<string, object> attributes)
        {
            try
            {
                var split = splitCache.GetSplit(feature);

                if (split == null)
                {
                    Log.Warn(String.Format("Unknown or invalid feature: {0}", feature));
                    return Control;
                }

                long start = CurrentTimeHelper.CurrentTimeMillis();
                var clock = new Stopwatch();
                clock.Start();
                
                var treatment = engine.GetTreatment(key, split, attributes);
                
                RecordStats(key, feature, start, treatment, SdkGetTreatment, clock);

                return treatment;
            }
            catch (Exception e)
            {
                Log.Error(String.Format("Exception caught getting treatment for feature: {0}", feature), e);
                return Control;
            }
        }
    }
}
