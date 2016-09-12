using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
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
        private static long HttpConnectionTimeout;
        private static long HttpReadTimeout;
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

        public SelfRefreshingClient(string apiKey, ConfigurationOptions config)
        {
            InitializeLogger();
            ApiKey = apiKey;
            ReadConfig(config);
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

        private void ReadConfig(ConfigurationOptions config)
        {
            BaseUrl = "https://sdk-aws-staging.split.io/api/";
            EventsBaseUrl = "https://events-aws-staging.split.io/api/";
            SplitsRefreshRate = config.FeaturesRefreshRate ?? 30;
            SegmentRefreshRate = config.SegmentsRefreshRate ?? 30;
            HttpEncoding = "gzip";
            HttpConnectionTimeout = config.ConnectionTimeout ?? 15000;
            HttpReadTimeout = config.ReadTimeout ?? 15000;
            SdkVersion = Version.SplitSdkVersion;
            SdkSpecVersion = "net-" + Version.SplitSpecVersion;
            SdkMachineName = config.SdkMachineName;
            SdkMachineIP = config.SdkMachineIP;
            RandomizeRefreshRates = config.RandomizeRefreshRates;
            BlockMilisecondsUntilReady = config.Ready ?? 0;
            ConcurrencyLevel = config.SplitsStorageConcurrencyLevel ?? 4;
            TreatmentLogRefreshRate = config.ImpressionsRefreshRate ?? 60;
            TreatmentLogSize = config.MaxImpressionsLogSize ?? -1;
            MaxCountCalls = config.MaxMetricsCountCallsBeforeFlush ?? 1000;
            MaxTimeBetweenCalls = config.MaxMetricsTimeBetweenCallsBeforeFlush ?? 60;
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
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

            FileAppender fileAppender = new FileAppender();
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new FileAppender.MinimalLock();
            fileAppender.File = @"Logs\split-sdk.log";
            PatternLayout pl = new PatternLayout();
            pl.ConversionPattern = "%date %level %logger - %message%newline";
            pl.ActivateOptions();
            fileAppender.Layout = pl;
            fileAppender.ActivateOptions();

            log4net.Config.BasicConfigurator.Configure(fileAppender);
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
            metricsSdkApiClient = new MetricsSdkApiClient(header, EventsBaseUrl, HttpConnectionTimeout, HttpReadTimeout);
            BuildMetricsLog();
            splitSdkApiClient = new SplitSdkApiClient(header, BaseUrl, HttpConnectionTimeout, HttpReadTimeout, metricsLog);
            segmentSdkApiClient = new SegmentSdkApiClient(header, BaseUrl, HttpConnectionTimeout, HttpReadTimeout, metricsLog);
            treatmentSdkApiClient = new TreatmentSdkApiClient(header, EventsBaseUrl, HttpConnectionTimeout, HttpReadTimeout);
        }

        private void BuildManager()
        {
            manager = new SplitManager(splitFetcher);
        }
    }
}
