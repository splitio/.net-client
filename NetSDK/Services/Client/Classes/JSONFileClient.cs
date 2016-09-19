using log4net;
using Splitio.Domain;
using Splitio.Services.Cache.Classes;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Parsing;
using Splitio.Services.SegmentFetcher.Classes;
using Splitio.Services.SplitFetcher.Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class JSONFileClient:SplitClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JSONFileClient));

        public JSONFileClient(string splitsFilePath, string segmentsFilePath)
        {
            InitializeLogger();
            var segmentCache = new InMemorySegmentCache(new ConcurrentDictionary<string, Segment>());
            var segmentFetcher = new JSONFileSegmentFetcher(segmentsFilePath, segmentCache);
            var splitParser = new SplitParser(segmentFetcher, segmentCache);
            splitFetcher = new JSONFileSplitFetcher(splitsFilePath, splitParser);
            splitter = new Splitter();
            engine = new Engine(splitter);
        }

        private void InitializeLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
