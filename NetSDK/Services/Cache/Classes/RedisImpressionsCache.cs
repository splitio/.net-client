using Newtonsoft.Json;
using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Classes
{
    public class RedisImpressionsCache : IImpressionsCache
    {
        private IRedisAdapter redisAdapter;
        private const string impressionKeyPrefix = "SPLITIO.impressions.";

        public RedisImpressionsCache(IRedisAdapter redisAdapter)
        {
            this.redisAdapter = redisAdapter;
        }

        public void AddImpression(KeyImpression impression)
        {
            var key = impressionKeyPrefix + impression.feature;
            var impressionToSend = new{ keyName = impression.keyName, treatment = impression.treatment, time = impression.time };
            var impressionJson = JsonConvert.SerializeObject(impressionToSend);
            redisAdapter.SAdd(key, impressionJson);
        }

        public List<KeyImpression> FetchAllAndClear()
        {
            var impressions = redisAdapter.SMembers(impressionKeyPrefix + "*");
            var result = impressions.Select(x => JsonConvert.DeserializeObject<KeyImpression>(x)).ToList();
            var features = result.Select(x => x.feature).Distinct();
            foreach (var feature in features)
            {
                var key = impressionKeyPrefix + feature;
                redisAdapter.Del(key);
            }
            return result;
        }
    }
}
