using NetSDK.Services.Client.Classes;
using NetSDK.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class ClientFactory
    {
        public IClient BuildApiClient(string apiKey, ConfigurationOptions options)
        {
            if (apiKey == "localhost")
            {
                return new LocalhostClient(options.LocalhostFilePath);
            }
            return new SelfRefreshingClient(apiKey);
        }
    }
}
