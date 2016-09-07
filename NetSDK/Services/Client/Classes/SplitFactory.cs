using Splitio.Services.Client.Classes;
using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class SplitFactory
    {
        private ISplitClient client;
        private ISplitManager manager;

        public ISplitClient BuildSplitClient(string apiKey, ConfigurationOptions options = null)
        {
            if (client == null)
            {
                if (apiKey == "localhost")
                {
                    client = new LocalhostClient(options.LocalhostFilePath);
                }
                else
                {
                    client = new SelfRefreshingClient(apiKey);
                }
            }
            return client;
        }

        public ISplitManager GetSplitManager()
        {
            if (client != null)
            {
                manager = client.GetSplitManager();
            }

            return manager;
        }
    }
}
