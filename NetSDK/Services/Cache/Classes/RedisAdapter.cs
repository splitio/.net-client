using Splitio.Services.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Classes
{
    public class RedisAdapter : IRedisAdapter
    {
        private ConnectionMultiplexer redis;
        private IDatabase database;
        private IServer server;

        public RedisAdapter()
        {
            //TODO: configs as input
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            database = redis.GetDatabase();
            server = redis.GetServer("localhost", 6379);
        }

        public bool Set(string key, string value)
        {
            return database.StringSet(key, value);
        }

        public string Get(string key)
        {
            return database.StringGet(key);
        }

        public RedisValue[] Get(RedisKey[] keys)
        {
            return database.StringGet(keys);
        }

        public RedisKey[] Keys(string pattern)
        {
            var keys = server.Keys(pattern : pattern);
            return keys.ToArray();
        }

        public bool Del(string key)
        {
            return database.KeyDelete(key);
        }

        public long Del(RedisKey[] keys)
        {
            return database.KeyDelete(keys);
        }

        public void Flush()
        {
            server.FlushDatabase();
        }
    }
}
