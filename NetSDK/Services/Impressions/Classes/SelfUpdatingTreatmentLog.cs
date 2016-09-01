using Newtonsoft.Json;
using Splitio.Domain;
using Splitio.Services.Impressions.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Splitio.Services.Impressions.Classes
{
    public class SelfUpdatingTreatmentLog: InMemoryTreatmentLog, ITreatmentLog
    {
        private ITreatmentSdkApiClient apiClient;
        private int interval;
        private bool stopped;

        public SelfUpdatingTreatmentLog(ITreatmentSdkApiClient apiClient, int interval = 180, ConcurrentDictionary<string, ConcurrentQueue<KeyImpression>> cache = null, int maximumNumberOfKeysToCachePerTest = -1)
        :base(cache, maximumNumberOfKeysToCachePerTest)
        {
            this.apiClient = apiClient;
            this.interval = interval;
            this.stopped = true;
        }

        public void Start()
        {
            Thread thread = new Thread(StartRefreshing);
            thread.Start();
        }

        public void Stop()
        {
            stopped = true;
        }

        private void StartRefreshing()
        {
            if (!stopped)
            {
                return;
            }

            stopped = false;

            while (!stopped)
            {
                SendBulkImpressions();
                Thread.Sleep(interval);
            }
        }

        private void SendBulkImpressions()
        {
            var impressionsByFeature = FetchAllAndClear();

            if (impressionsByFeature.Count > 0)
            {
                try
                {
                    var impressionsJson = ConvertToJson(impressionsByFeature);
                    apiClient.SendBulkImpressions(impressionsJson);
                }
                catch (Exception e)
                {
                    Logger.Error("Exception caught updating impressions.", e);
                }
            }
        }

        protected override void NotifyEviction(string feature, ConcurrentQueue<KeyImpression> impressions) 
        { 
            if (String.IsNullOrEmpty(feature) || impressions == null || impressions.Count == 0)
            {
                return;
            }

            try
            {
                var impressionsJson = ConvertToJson(feature, impressions);
                apiClient.SendBulkImpressions(impressionsJson);
            }
            catch(Exception e)
            {
                Logger.Error("Exception caught updating impressions.", e);
            }
        }

        private string ConvertToJson(string feature, ConcurrentQueue<KeyImpression> impressions)
        {
            return JsonConvert.SerializeObject(impressions.Select(x => new { testName = feature, keyImpressions = x }));
        }

        private string ConvertToJson(ConcurrentDictionary<string, ConcurrentQueue<KeyImpression>> impressionsByFeature)
        {
            return JsonConvert.SerializeObject(impressionsByFeature.Select(x => new { testName = x.Key, keyImpressions = x.Value }));
        }

    }
}
