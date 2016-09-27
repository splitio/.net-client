using Splitio.Services.Client.Classes;
using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microbenchmark
{
    public static class ClientSingleInstance
    {
        private static ISplitClient instance;

        public static ISplitClient GetInstance(string apikey)
        {
            if (instance == null)
            {
                var config = new ConfigurationOptions();
                config.FeaturesRefreshRate = 30;
                config.SegmentsRefreshRate = 30;
                config.Endpoint = "https://sdk-aws-staging.split.io";
                config.EventsEndpoint = "https://events-aws-staging.split.io";
                config.ReadTimeoutInMs = 30000;
                config.ConnectionTimeOutInMs = 30000;
                config.Ready = 240000;
                config.NumberOfParalellSegmentTasks = 10;
                instance = new SelfRefreshingClient(apikey, config);
                ((SelfRefreshingClient)instance).Stop();
            }
            return instance;
        }
    }
}
