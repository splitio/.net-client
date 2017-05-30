using NLog;
using NLog.Config;
using NLog.Targets;
using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Services.Parsing;
using Splitio.Services.Parsing.Classes;
using Splitio.Services.SegmentFetcher.Classes;
using Splitio.Services.SplitFetcher.Classes;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Splitio.Services.Client.Classes
{
    public class JSONFileClient:SplitClient
    {
        private static readonly Logger Log = LogManager.GetLogger(typeof(JSONFileClient).ToString());
        public JSONFileClient(string splitsFilePath, string segmentsFilePath, ISegmentCache segmentCacheInstance = null, ISplitCache splitCacheInstance = null, IImpressionListener treatmentLogInstance = null, bool isLabelsEnabled = true)
        {
            InitializeLogger();
            segmentCache = segmentCacheInstance ?? new InMemorySegmentCache(new ConcurrentDictionary<string, Segment>());
            var segmentFetcher = new JSONFileSegmentFetcher(segmentsFilePath, segmentCache);
            var splitParser = new InMemorySplitParser(segmentFetcher, segmentCache);
            var splitChangeFetcher = new JSONFileSplitChangeFetcher(splitsFilePath);
            var splitChangesResult = splitChangeFetcher.Fetch(-1);
            var parsedSplits = new ConcurrentDictionary<string, ParsedSplit>();
            foreach (Split split in splitChangesResult.splits)
                parsedSplits.TryAdd(split.name, splitParser.Parse(split));         
            splitCache = splitCacheInstance ?? new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>(parsedSplits));
            impressionListener = treatmentLogInstance;
            splitter = new Splitter();
            LabelsEnabled = isLabelsEnabled;
        }

        private void InitializeLogger()
        {
            var fileTarget = new FileTarget();
            fileTarget.Name = "splitio";
            fileTarget.FileName = @".\Logs\splitio.log";
            fileTarget.ArchiveFileName = @".\Logs\splitio.{#}.log";
            fileTarget.LineEnding = LineEndingMode.CRLF;
            fileTarget.Layout = "${longdate} ${level: uppercase = true} ${logger} - ${message} - ${exception:format=tostring}";
            fileTarget.ConcurrentWrites = true;
            fileTarget.CreateDirs = true;
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.DateAndSequence;
            fileTarget.ArchiveAboveSize = 200000000;
            fileTarget.ArchiveDateFormat = "yyyyMMdd";
            fileTarget.MaxArchiveFiles = 30;
            var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);

            if (LogManager.Configuration == null)
            {
                var config = new LoggingConfiguration();
                config.AddTarget("splitio", fileTarget);
                config.LoggingRules.Add(rule);
                LogManager.Configuration = config;
            }
            else
            {
                if (LogManager.Configuration.ConfiguredNamedTargets.Where(x => x.Name == "splitio").FirstOrDefault() == null)
                {
                    LogManager.Configuration.AddTarget("splitio", fileTarget);
                    LogManager.Configuration.LoggingRules.Add(rule);
                }
            }
        }

        public void RemoveSplitFromCache(string splitName)
        {
            splitCache.RemoveSplit(splitName);
        }

        public void RemoveKeyFromSegmentCache(string segmentName, List<string> keys)
        {
            segmentCache.RemoveFromSegment(segmentName, keys);
        }
    }
}
