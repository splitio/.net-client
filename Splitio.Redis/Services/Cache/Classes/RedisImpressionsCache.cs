﻿using Newtonsoft.Json;
using Splitio.Domain;
using Splitio.Redis.Services.Cache.Interfaces;
using Splitio.Services.Shared.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitio.Redis.Services.Cache.Classes
{
    public class RedisImpressionsCache : RedisCacheBase, ISimpleCache<IList<KeyImpression>>
    {
        private const string impressionKeyPrefix = "impressions.";

        public RedisImpressionsCache(IRedisAdapter redisAdapter, string machineIP, string sdkVersion, string userPrefix = null)
            : base(redisAdapter, machineIP, sdkVersion, userPrefix) 
        {}

        public void AddItem(IList<KeyImpression> items)
        {
            var key = string.Format("{0}SPLITIO.impressions", string.IsNullOrEmpty(UserPrefix) ? string.Empty : $"{UserPrefix}.");

            var impressions = items.Select(item => JsonConvert.SerializeObject(new
            {
                m = new { s = SdkVersion, i = MachineIp, n = Environment.MachineName },
                i = new { k = item.keyName, b = item.bucketingKey, f = item.feature, t = item.treatment, r = item.label, c = item.changeNumber, m = item.time }
            }));

            var lengthRedis = redisAdapter.ListRightPush(key, impressions.Select(i => (RedisValue)i).ToArray());

            if (lengthRedis == items.Count)
            {
                redisAdapter.KeyExpire(key, new TimeSpan(0, 0, 3600));
            }
        }
    }
}
