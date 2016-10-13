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
        private string apiKey;
        private ConfigurationOptions options;

        public SplitFactory(string apiKey, ConfigurationOptions options = null)
        {
            this.apiKey = apiKey;
            this.options = options;
        }

        public ISplitClient Client()
        {
            if (client == null)
            {
                BuildSplitClient();
            }
            return client;
        }

        private void BuildSplitClient()
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new Exception("API Key should be set to initialize Split SDK.");
            }

            if (options == null)
            {
                options = new ConfigurationOptions();
            }

            if (apiKey == "localhost")
            {
                client = new LocalhostClient(options.LocalhostFilePath);
            }
            else
            {
                client = new SelfRefreshingClient(apiKey, options);
            }
        }

        public ISplitManager Manager()
        {
            if (client == null)
            {
                BuildSplitClient();
            }
           
            manager = client.GetSplitManager();

            return manager;
        }
    }
}
