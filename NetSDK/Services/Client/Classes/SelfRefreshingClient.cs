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
        private static string ApiKey = ConfigurationManager.AppSettings["API_KEY"];
        private static string BaseUrl = ConfigurationManager.AppSettings["BASE_URL"];
        private static string SplitsRefreshRate = ConfigurationManager.AppSettings["SPLITS_REFRESH_RATE"];
        private static string SegmentRefreshRate = ConfigurationManager.AppSettings["SEGMENT_REFRESH_RATE"];
        private static string HttpEncoding = ConfigurationManager.AppSettings["HTTP_ENCODING"];
        private static string HttpConnectionTimeout = ConfigurationManager.AppSettings["HTTP_CONNECTION_TIMEOUT"];
        private static string HttpReadTimeout = ConfigurationManager.AppSettings["HTTP_READ_TIMEOUT"];
        private static string SdkVersion = ConfigurationManager.AppSettings["SPLIT_SDK_VERSION"];
        private static string SdkSpecVersion = ConfigurationManager.AppSettings["SPLIT_SDK_SPEC_VERSION"];
        private static string SdkMachineName = ConfigurationManager.AppSettings["SPLIT_SDK_MACHINE_NAME"];
        private static string SdkMachineIP = ConfigurationManager.AppSettings["SPLIT_SDK_MACHINE_IP"];
        private static string RandomizeRefreshRates = ConfigurationManager.AppSettings["RANDOMIZE_REFRESH_RATE"];

        public SelfRefreshingClient()
        {
            InitializeLogger();
            BuildSdkReadinessGates();
            BuildSdkApiClients();
            BuildSplitFetcher();
            BuildSplitter();
            BuildEngine();
            Start();
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
            var randomizeRefreshRates = bool.Parse(RandomizeRefreshRates);
            var segmentRefreshRateValue = int.Parse(SegmentRefreshRate);
            var segmentRefreshRate = randomizeRefreshRates ? Random(segmentRefreshRateValue) : segmentRefreshRateValue;
            var splitsRefreshRateValue = int.Parse(SplitsRefreshRate);
            var splitsRefreshRate = randomizeRefreshRates ? Random(splitsRefreshRateValue) : splitsRefreshRateValue;


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
