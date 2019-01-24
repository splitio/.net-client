using Splitio.Domain;
using Splitio.Services.Shared.Interfaces;

namespace Splitio.Redis.Services.Impressions.Classes
{
    public class RedisTreatmentLog : IListener<KeyImpression>
    {
        private ISimpleCache<KeyImpression> impressionsCache;

        public RedisTreatmentLog(ISimpleCache<KeyImpression> impressionsCache)
        {
            this.impressionsCache = impressionsCache;
        }

        public void Log(KeyImpression items)
        {
            impressionsCache.AddItem(items);
        }
    }
}
