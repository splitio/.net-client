using Newtonsoft.Json;
using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Splitio.Services.Cache.Classes
{
    public class RedisSplitCache : ISplitCache
    {
        private IRedisAdapter redisAdapter;
        private const string splitKeyPrefix = "SPLITIO.split.";
        private const string splitsKeyPrefix = "SPLITIO.splits.";

        public RedisSplitCache(IRedisAdapter redisAdapter)
        {
            this.redisAdapter = redisAdapter;
        }
         
        public void AddSplit(string splitName, SplitBase split)
        {
            var splitJson = JsonConvert.SerializeObject(split);
            redisAdapter.Set(splitKeyPrefix + splitName, splitJson);
        }

        public bool RemoveSplit(string splitName)
        {
            return redisAdapter.Del(splitKeyPrefix + splitName);
        }

        public void SetChangeNumber(long changeNumber)
        {
            redisAdapter.Set(splitsKeyPrefix + "till", changeNumber.ToString());
        }

        public long GetChangeNumber()
        {
            string changeNumberString = redisAdapter.Get(splitsKeyPrefix + "till");
            long changeNumberParsed;
            var result = long.TryParse(changeNumberString, out changeNumberParsed);
            
            return result ? changeNumberParsed : -1;
        }

        public SplitBase GetSplit(string splitName)
        {
            var splitJson = redisAdapter.Get(splitKeyPrefix + splitName);
            return splitJson != null ? JsonConvert.DeserializeObject<Split>(splitJson) : null;
        }

        public List<SplitBase> GetAllSplits()
        {
            var splitKeys = redisAdapter.Keys(splitKeyPrefix + "*");
            var splitValues = redisAdapter.Get(splitKeys);
            var splits = splitValues.Select(x=> JsonConvert.DeserializeObject<Split>(x));
            return splits.Cast<SplitBase>().ToList();
        }
    }
}
