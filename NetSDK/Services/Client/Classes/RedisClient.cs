using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Splitio.CommonLibraries;
using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Impressions.Classes;
using Splitio.Services.Metrics.Classes;
using Splitio.Services.Parsing;
using Splitio.Services.Parsing.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class RedisClient : SplitClient
    {
        private RedisSplitParser splitParser;
        private RedisAdapter redisAdapter; 

        private static string SdkVersion;
        private static string SdkSpecVersion;
        private static string SdkMachineName;
        private static string SdkMachineIP;
        private static string RedisHost;
        private static string RedisPort;
        private static string RedisPassword;
        private static int RedisDatabase;
        private static int RedisConnectTimeout;
        private static int RedisConnectRetry;
        private static int RedisSyncTimeout;
        private static string RedisUserPrefix;


        public RedisClient(ConfigurationOptions config)
        {
            InitializeLogger();
            ReadConfig(config);
            BuildRedisCache();
            BuildTreatmentLog(config); 
            BuildMetricsLog();
            BuildSplitter();
            BuildManager();
            BuildParser();
        }


        private void InitializeLogger()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            if (hierarchy.Root.Appenders.Count == 0)
            {
                RollingFileAppender fileAppender = new RollingFileAppender();
                fileAppender.AppendToFile = true;
                fileAppender.LockingModel = new FileAppender.MinimalLock();
                fileAppender.File = @"Logs\split-sdk.log";
                fileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
                fileAppender.DatePattern = "yyyyMMdd";
                PatternLayout pl = new PatternLayout();
                pl.ConversionPattern = "%date %level %logger - %message%newline";
                pl.ActivateOptions();
                fileAppender.Layout = pl;
                fileAppender.ActivateOptions();

                log4net.Config.BasicConfigurator.Configure(fileAppender);
            }
        }
        private void ReadConfig(ConfigurationOptions config)
        {
            SdkVersion = ".NET-" + Version.SplitSdkVersion;
            SdkSpecVersion = ".NET-" + Version.SplitSpecVersion;
            SdkMachineName = config.SdkMachineName ?? Environment.MachineName;
            SdkMachineIP = config.SdkMachineIP ?? Dns.GetHostAddresses(Environment.MachineName).Where(x => x.AddressFamily == AddressFamily.InterNetwork && x.IsIPv6LinkLocal == false).Last().ToString();
            RedisHost = config.CacheAdapterConfig.Host;
            RedisPort = config.CacheAdapterConfig.Port;
            RedisPassword = config.CacheAdapterConfig.Password;
            RedisDatabase = config.CacheAdapterConfig.Database ?? 0;
            RedisConnectTimeout = config.CacheAdapterConfig.ConnectTimeout ?? 0;
            RedisSyncTimeout = config.CacheAdapterConfig.SyncTimeout ?? 0;
            RedisConnectRetry = config.CacheAdapterConfig.ConnectRetry ?? 0;
            RedisUserPrefix = config.CacheAdapterConfig.UserPrefix;
            LabelsEnabled = config.LabelsEnabled ?? true;
        }

        private void BuildRedisCache()
        {
            redisAdapter = new RedisAdapter(RedisHost, RedisPort, RedisPassword, RedisDatabase, RedisConnectTimeout, RedisConnectRetry, RedisSyncTimeout);
            splitCache = new RedisSplitCache(redisAdapter, RedisUserPrefix);
            segmentCache = new RedisSegmentCache(redisAdapter, RedisUserPrefix);
            metricsCache = new RedisMetricsCache(redisAdapter, SdkMachineIP, SdkVersion, RedisUserPrefix);
            impressionsCache = new RedisImpressionsCache(redisAdapter, SdkMachineIP, SdkVersion, RedisUserPrefix);
        }

        private void BuildTreatmentLog(ConfigurationOptions config)
        {
            var treatmentLog = new RedisTreatmentLog(impressionsCache);
            impressionListener = new AsynchronousImpressionListener();
            ((AsynchronousImpressionListener)impressionListener).AddListener(treatmentLog);
            ((AsynchronousImpressionListener)impressionListener).AddListener(config.ImpressionListener);
        }

        private void BuildMetricsLog()
        {
            metricsLog = new RedisMetricsLog(metricsCache);
        }

        private void BuildSplitter()
        {
            splitter = new Splitter();
        }

        private void BuildManager()
        {
            manager = new RedisSplitManager(splitCache);
        }

        private void BuildParser()
        {
            splitParser = new RedisSplitParser(segmentCache);
        }

        protected override string GetTreatmentForFeature(Key key, string feature, Dictionary<string, object> attributes)
        {
            long start = CurrentTimeHelper.CurrentTimeMillis();
            var clock = new Stopwatch();
            clock.Start();

            try
            {
                var split = splitCache.GetSplit(feature);

                if (split == null)
                {
                    //if split definition was not found, impression label = "rules not found"
                    RecordStats(key, feature, null, LabelSplitNotFound, start, Control, SdkGetTreatment, clock);

                    Log.Warn(String.Format("Unknown or invalid feature: {0}", feature));
                    return Control;
                }

                ParsedSplit parsedSplit = splitParser.Parse((Split)split);

                var treatment = GetTreatment(key, parsedSplit, attributes, start, clock);

                return treatment;
            }
            catch (Exception e)
            {
                //if there was an exception, impression label = "exception"
                RecordStats(key, feature, null, LabelException, start, Control, SdkGetTreatment, clock);

                Log.Error(String.Format("Exception caught getting treatment for feature: {0}", feature), e);
                return Control;
            }
        }      
    }
}
