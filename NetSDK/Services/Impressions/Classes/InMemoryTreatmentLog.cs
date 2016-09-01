using log4net;
using Splitio.Domain;
using Splitio.Services.Impressions.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Impressions.Classes
{
    public class InMemoryTreatmentLog : ITreatmentLog
    {
        private ConcurrentDictionary<String, ConcurrentQueue<KeyImpression>> cache;
        private int maximumNumberOfKeysToCachePerTest;
        private object lockObject = new Object();
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(InMemoryTreatmentLog));

        public InMemoryTreatmentLog(ConcurrentDictionary<string, ConcurrentQueue<KeyImpression>> cache = null, int maximumNumberOfKeysToCachePerTest = -1)
        {
            this.maximumNumberOfKeysToCachePerTest = maximumNumberOfKeysToCachePerTest;
            this.cache = cache ?? new ConcurrentDictionary<string, ConcurrentQueue<KeyImpression>>();
        }

        protected virtual void NotifyEviction(string feature, ConcurrentQueue<KeyImpression> impressions) { }


        public ConcurrentDictionary<String, ConcurrentQueue<KeyImpression>> FetchAllAndClear()
        {
            lock (lockObject)
            {
                var existingImpressions = new ConcurrentDictionary<String, ConcurrentQueue<KeyImpression>>(cache);
                cache = new ConcurrentDictionary<String, ConcurrentQueue<KeyImpression>>();
                return existingImpressions;
            }
        }

        public void Log(string id, string feature, string treatment, long time)
        {
            if (id == null || feature == null || treatment == null || time < 0)
            {
                return;
            }

            lock (lockObject)
            {
                ConcurrentQueue<KeyImpression> impressions;

                cache.TryGetValue(feature, out impressions);

                if (impressions == null)
                {
                    impressions = new ConcurrentQueue<KeyImpression>();
                    impressions.Enqueue(new KeyImpression { keyName = id, treatment = treatment, time = time });
                    cache.TryAdd(feature, impressions);
                }
                else
                {
                    if (impressions.Count >= maximumNumberOfKeysToCachePerTest)
                    {
                        Logger.Warn("Count limit for feature treatment log reached. Clearing impressions for feature.");
                        NotifyEviction(feature, impressions);
                        impressions = new ConcurrentQueue<KeyImpression>();
                        impressions.Enqueue(new KeyImpression { keyName = id, treatment = treatment, time = time });
                        cache.TryAdd(feature, impressions);
                    }
                    else
                    {
                        impressions.Enqueue(new KeyImpression { keyName = id, treatment = treatment, time = time });
                    }
                }

                return;
            }
        }
    }
}
