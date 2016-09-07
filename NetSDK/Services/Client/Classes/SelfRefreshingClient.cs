using Splitio.CommonLibraries;
using Splitio.Domain;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Impressions.Classes;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Services.Metrics.Classes;
using Splitio.Services.Metrics.Interfaces;
using Splitio.Services.Parsing;
using Splitio.Services.SegmentFetcher.Classes;
using Splitio.Services.SplitFetcher;
using Splitio.Services.SplitFetcher.Classes;
using Splitio.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace Splitio.Services.Client.Classes
{
    public class SelfRefreshingClient: SplitClient
    {
        private static string ApiKey;
        private static string BaseUrl;
        private static int SplitsRefreshRate;
        private static int SegmentRefreshRate;
        private static string HttpEncoding;
        private static string HttpConnectionTimeout;
        private static string HttpReadTimeout;
        private static string SdkVersion;
        private static string SdkSpecVersion;
        private static string SdkMachineName;
        private static string SdkMachineIP;
        private static bool RandomizeRefreshRates;
        private static int BlockMilisecondsUntilReady;
        private static int ConcurrencyLevel;
        private static int TreatmentLogRefreshRate;
        private static int TreatmentLogSize;
        private static string EventsBaseUrl;
        private static int MaxCountCalls;
        private static int MaxTimeBetweenCalls;


        /// <summary>
        /// Represents the initial number of buckets for a ConcurrentDictionary. 
        /// Should not be divisible by a small prime number. 
        /// The default capacity is 31. 
        /// More details : https://msdn.microsoft.com/en-us/library/dd287171(v=vs.110).aspx
        /// </summary>
        private const int InitialCapacity = 31;


        private SdkReadinessGates gates;
        private ISplitSdkApiClient splitSdkApiClient;
        private ISegmentSdkApiClient segmentSdkApiClient;
        private ITreatmentSdkApiClient treatmentSdkApiClient;
        private IMetricsSdkApiClient metricsSdkApiClient;

        public SelfRefreshingClient(string apiKey)
        {
            InitializeLogger();
            ApiKey = apiKey;
            ReadConfig();
            BuildSdkReadinessGates();
            BuildSdkApiClients();
            BuildSplitFetcher();
            BuildTreatmentLog();
            BuildSplitter();
            BuildEngine();
            BuildManager();
            Start();
            if (BlockMilisecondsUntilReady > 0)
            {
                BlockUntilReady(BlockMilisecondsUntilReady);
            }
        }

        private void ReadConfig()
        {
            BaseUrl = ConfigurationManager.AppSettings["BASE_URL"];
            SplitsRefreshRate = int.Parse(ConfigurationManager.AppSettings["SPLITS_REFRESH_RATE"]);
            SegmentRefreshRate = int.Parse(ConfigurationManager.AppSettings["SEGMENT_REFRESH_RATE"]);
            HttpEncoding = ConfigurationManager.AppSettings["HTTP_ENCODING"];
            HttpConnectionTimeout = ConfigurationManager.AppSettings["HTTP_CONNECTION_TIMEOUT"];
            HttpReadTimeout = ConfigurationManager.AppSettings["HTTP_READ_TIMEOUT"];
            SdkVersion = ConfigurationManager.AppSettings["SPLIT_SDK_VERSION"];
            SdkSpecVersion = "net-" + ConfigurationManager.AppSettings["SPLIT_SDK_SPEC_VERSION"];
            SdkMachineName = ConfigurationManager.AppSettings["SPLIT_SDK_MACHINE_NAME"];
            SdkMachineIP = ConfigurationManager.AppSettings["SPLIT_SDK_MACHINE_IP"];
            RandomizeRefreshRates = bool.Parse(ConfigurationManager.AppSettings["RANDOMIZE_REFRESH_RATE"]);
            BlockMilisecondsUntilReady = int.Parse(ConfigurationManager.AppSettings["BLOCK_MILISECONDS_UNTIL_READY"]);
            ConcurrencyLevel = int.Parse(ConfigurationManager.AppSettings["SPLITS_STORAGE_CONCURRENCY_LEVEL"]);
            TreatmentLogRefreshRate = int.Parse(ConfigurationManager.AppSettings["TEST_IMPRESSIONS_REFRESH_RATE"]);
            TreatmentLogSize = int.Parse(ConfigurationManager.AppSettings["TEST_IMPRESSIONS_QUEUE_SIZE"]);
            EventsBaseUrl = ConfigurationManager.AppSettings["EVENTS_BASE_URL"];
            MaxCountCalls = int.Parse(ConfigurationManager.AppSettings["METRICS_MAX_COUNT_CALLS"]);
            MaxTimeBetweenCalls = int.Parse(ConfigurationManager.AppSettings["METRICS_MAX_TIME_BETWEEN_CALLS"]);
        }

        private void BlockUntilReady(int BlockMilisecondsUntilReady)
        {
            if (!gates.IsSDKReady(BlockMilisecondsUntilReady))
            {
                throw new TimeoutException(String.Format("SDK was not ready in {0} miliseconds", BlockMilisecondsUntilReady));
            }
        }

        public void Start()
        {
            ((SelfUpdatingTreatmentLog)treatmentLog).Start();
            ((SelfRefreshingSplitFetcher)splitFetcher).Start();
        }

        public void Stop()
        {
            ((SelfRefreshingSplitFetcher)splitFetcher).Stop();
            ((SelfUpdatingTreatmentLog)treatmentLog).Stop();
        }

        private void InitializeLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private void BuildSplitter()
        {
            splitter = new Splitter();
        }

        private void BuildEngine()
        {
            engine = new Engine(splitter);
        }

        private void BuildSdkReadinessGates()
        {
            gates = new SdkReadinessGates();
        }

        private void BuildSplitFetcher()
        {
            var segmentRefreshRate = RandomizeRefreshRates ? Random(SegmentRefreshRate) : SegmentRefreshRate;
            var splitsRefreshRate = RandomizeRefreshRates ? Random(SplitsRefreshRate) : SplitsRefreshRate;

            var segmentChangeFetcher = new ApiSegmentChangeFetcher(segmentSdkApiClient);
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(segmentChangeFetcher, gates, new ConcurrentDictionary<string, SelfRefreshingSegment>(ConcurrencyLevel, InitialCapacity), segmentRefreshRate);
            var splitChangeFetcher = new ApiSplitChangeFetcher(splitSdkApiClient);
            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            splitFetcher = new SelfRefreshingSplitFetcher(splitChangeFetcher, splitParser, gates, splitsRefreshRate, -1, new ConcurrentDictionary<string, Domain.ParsedSplit>(ConcurrencyLevel, InitialCapacity));
        }

        private void BuildTreatmentLog()
        {
            treatmentLog = new SelfUpdatingTreatmentLog(treatmentSdkApiClient, TreatmentLogRefreshRate, new BlockingQueue<KeyImpression>(TreatmentLogSize));
        }


        private void BuildMetricsLog()
        {
            metricsLog = new AsyncMetricsLog(metricsSdkApiClient, new ConcurrentDictionary<string, Counter>(), new ConcurrentDictionary<string, ILatencyTracker>(), new ConcurrentDictionary<string, long>(), MaxCountCalls, MaxTimeBetweenCalls);
        }

        private int Random(int refreshRate)
        {
            Random random = new Random();
            return Math.Max(5, random.Next(refreshRate/2, refreshRate));
        }

        private void BuildSdkApiClients()
        {
            var header = new HTTPHeader();
            header.authorizationApiKey = ApiKey;
            header.encoding = HttpEncoding;
            header.splitSDKVersion = SdkVersion;
            header.splitSDKSpecVersion = SdkSpecVersion;
            header.splitSDKMachineName = SdkMachineName;
            header.splitSDKMachineIP = SdkMachineIP;
            var connectionTimeout = long.Parse(HttpConnectionTimeout);
            var readTimeout = long.Parse(HttpReadTimeout);
            metricsSdkApiClient = new MetricsSdkApiClient(header, EventsBaseUrl, connectionTimeout, readTimeout);
            BuildMetricsLog();
            splitSdkApiClient = new SplitSdkApiClient(header, BaseUrl, connectionTimeout, readTimeout, metricsLog);
            segmentSdkApiClient = new SegmentSdkApiClient(header, BaseUrl, connectionTimeout, readTimeout, metricsLog);
            treatmentSdkApiClient = new TreatmentSdkApiClient(header, EventsBaseUrl, connectionTimeout, readTimeout);
        }

        private void BuildManager()
        {
            manager = new SplitManager(splitFetcher);
        }
    }
}
