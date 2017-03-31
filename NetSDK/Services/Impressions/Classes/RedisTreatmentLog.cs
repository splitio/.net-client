using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Impressions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitio.Services.Impressions.Classes
{
    public class RedisTreatmentLog : IImpressionListener
    {
        private IImpressionsCache impressionsCache;

        public RedisTreatmentLog(IImpressionsCache impressionsCache)
        {
            this.impressionsCache = impressionsCache;
        }

        public void Log(KeyImpression impression)
        {
            var enqueueTask = new Task(() => impressionsCache.AddImpression(impression));
            enqueueTask.Start();
        }
    }
}
