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
            var config = String.Format("{0}:{1}, allowAdmin = true", host, port);
            redis = ConnectionMultiplexer.Connect(config);
            database = redis.GetDatabase(databaseNumber);
            server = redis.GetServer(String.Format("{0}:{1}", host, port));
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

        public bool SAdd(string key, RedisValue value)
        {
            return database.SetAdd(key, value);
        }

        public long SAdd(string key, RedisValue[] values)
        {
            return database.SetAdd(key, values);
        }

        public long SRem(string key, RedisValue[] values)
        {
            return database.SetRemove(key, values);
        }

        public bool SIsMember(string key, string value)
        {
            return database.SetContains(key, value);
        }

        public RedisValue[] SMembers(string key)
        {
            return database.SetMembers(key);
        }

        public void Flush()
        {
            server.FlushDatabase();
        }
    }
}
