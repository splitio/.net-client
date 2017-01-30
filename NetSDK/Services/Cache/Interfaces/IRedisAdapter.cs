using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Cache.Interfaces
{
    public interface IRedisAdapter
    {
        bool Set(string key, string value);

        string Get(string key);

        RedisValue[] Get(RedisKey[] keys);

        RedisKey[] Keys(string pattern);

        bool Del(string key);

        long Del(RedisKey[] keys);

        long SAdd(string key, RedisValue[] values);

        long SRem(string key, RedisValue[] values);

        bool SIsMember(string key, string value);

        RedisValue[] SMembers(string key);

        void Flush();
    }
}
