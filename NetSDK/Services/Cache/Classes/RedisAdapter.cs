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

        public RedisAdapter(string host, string port, int databaseNumber = 0, string password = "")
        {
            //TODO: set password
            redis = ConnectionMultiplexer.Connect(String.Format("{0}:{1}", host, port));
            database = redis.GetDatabase(databaseNumber);
            server = redis.GetServer(host, port);
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
