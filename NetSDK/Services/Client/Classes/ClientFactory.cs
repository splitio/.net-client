using Splitio.Services.Client.Classes;
using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class ClientFactory
    {
        public IClient BuildApiClient(string apiKey, ConfigurationOptions options = null)
        {
            if (apiKey == "localhost")
            {
                return new LocalhostClient(options.LocalhostFilePath);
            }
            return new SelfRefreshingClient(apiKey);
        }
    }
}
