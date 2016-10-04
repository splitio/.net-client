using log4net;
using Newtonsoft.Json;
using Splitio.CommonLibraries;
using Splitio.Domain;
using Splitio.Services.Impressions.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Splitio.Services.Impressions.Classes
{
    public class SelfUpdatingTreatmentLog: ITreatmentLog
    {
        private ITreatmentSdkApiClient apiClient;
        private int interval;
        private bool stopped;
        private BlockingQueue<KeyImpression> queue;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        protected static readonly ILog Logger = LogManager.GetLogger(typeof(SelfUpdatingTreatmentLog));

        public SelfUpdatingTreatmentLog(ITreatmentSdkApiClient apiClient, int interval, BlockingQueue<KeyImpression> queue = null, int maximumNumberOfKeysToCache = -1)
        {
            this.queue = queue ?? new BlockingQueue<KeyImpression>(maximumNumberOfKeysToCache);
            this.apiClient = apiClient;
            this.interval = interval;
        }

        public void Start()
        {
            PeriodicTaskFactory.Start(() => { SendBulkImpressions(); }, interval * 1000, cancellationTokenSource.Token);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }


        private void SendBulkImpressions()
        {
            if(queue.HasReachedMaxSize())
            {
                Logger.Warn("Split SDK impressions queue is full. Impressions may have been dropped. Consider increasing capacity.");
            }

            var impressions = queue.FetchAllAndClear();

            if (impressions.Count > 0)
            {
                try
                {
                    var impressionsJson = ConvertToJson(impressions);
                    apiClient.SendBulkImpressions(impressionsJson);
                }
                catch (Exception e)
                {
                    Logger.Error("Exception caught updating impressions.", e);
                }
            }
        }

        private string ConvertToJson(ConcurrentQueue<KeyImpression> impressions)
        {
            var impressionsPerFeature = 
                impressions
                .GroupBy(item => item.feature)
                .Select(group => new { testName = group.Key, keyImpressions = group.Select(x => new { keyName = x.keyName, treatment = x.treatment, time = x.time }) });
            return JsonConvert.SerializeObject(impressionsPerFeature);
        }


        public void Log(string id, string feature, string treatment, long time)
        {
            KeyImpression impression = new KeyImpression() { feature = feature, keyName = id, treatment = treatment, time = time };
            var enqueueTask = new Task(() => queue.Enqueue(impression));
            enqueueTask.Start();
        }


        public void Log(Key key, string feature, string treatment, long time)
        {
            KeyImpression impression = new KeyImpression() { feature = feature, keyName = key.matchingKey, treatment = treatment, time = time, bucketingKey = key.bucketingKey };
            var enqueueTask = new Task(() => queue.Enqueue(impression));
            enqueueTask.Start();
        }
    }
}
