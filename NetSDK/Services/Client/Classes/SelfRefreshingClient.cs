using NetSDK.CommonLibraries;
using NetSDK.Services.Client.Interfaces;
using NetSDK.Services.EngineEvaluator;
using NetSDK.Services.Parsing;
using NetSDK.Services.SegmentFetcher.Classes;
using NetSDK.Services.SplitFetcher;
using NetSDK.Services.SplitFetcher.Classes;
using NetSDK.Services.SplitFetcher.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetSDK.Services.Client.Classes
{
    public class SelfRefreshingClient: Client
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

        private SdkReadinessGates gates;
        private ISplitSdkApiClient splitSdkApiClient;
        private ISegmentSdkApiClient segmentSdkApiClient;

        public SelfRefreshingClient(string apiKey)
        {
            InitializeLogger();
            ApiKey = apiKey;
            ReadConfig();
            BuildSdkReadinessGates();
            BuildSdkApiClients();
            BuildSplitFetcher();
            BuildSplitter();
            BuildEngine();
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
            SdkSpecVersion = ConfigurationManager.AppSettings["SPLIT_SDK_SPEC_VERSION"];
            SdkMachineName = ConfigurationManager.AppSettings["SPLIT_SDK_MACHINE_NAME"];
            SdkMachineIP = ConfigurationManager.AppSettings["SPLIT_SDK_MACHINE_IP"];
            RandomizeRefreshRates = bool.Parse(ConfigurationManager.AppSettings["RANDOMIZE_REFRESH_RATE"]);
            BlockMilisecondsUntilReady = int.Parse(ConfigurationManager.AppSettings["BLOCK_MILISECONDS_UNTIL_READY"]);
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
            ((SelfRefreshingSplitFetcher)splitFetcher).Start();
        }

        public void Stop()
        {
            ((SelfRefreshingSplitFetcher)splitFetcher).Stop();
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
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(segmentChangeFetcher, gates,  interval: segmentRefreshRate);
            var splitChangeFetcher = new ApiSplitChangeFetcher(splitSdkApiClient);
            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            splitFetcher = new SelfRefreshingSplitFetcher(splitChangeFetcher, splitParser, gates, splitsRefreshRate);
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
            splitSdkApiClient = new SplitSdkApiClient(header, BaseUrl, connectionTimeout, readTimeout);
            segmentSdkApiClient = new SegmentSdkApiClient(header, BaseUrl, connectionTimeout, readTimeout);
        }
    }
}
