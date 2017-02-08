using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class RedisConfigurationOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }
        public int? Database { get; set; }
        public long? ConnectTimeout { get; set; }
        public long? ConnectRetry { get; set; }
        public long? SyncTimeout { get; set; }
        public string UserPrefix { get; set; }
    }
}
