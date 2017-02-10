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
        public int? ConnectTimeout { get; set; }
        public int? ConnectRetry { get; set; }
        public int? SyncTimeout { get; set; }
        public string UserPrefix { get; set; }
    }
}
