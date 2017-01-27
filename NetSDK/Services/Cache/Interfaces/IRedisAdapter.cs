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
    }
}
