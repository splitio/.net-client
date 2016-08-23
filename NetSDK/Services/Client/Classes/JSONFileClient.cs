using log4net;
using Splitio.Services.EngineEvaluator;
using Splitio.Services.Parsing;
using Splitio.Services.SegmentFetcher.Classes;
using Splitio.Services.SplitFetcher.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Client.Classes
{
    public class JSONFileClient:Client
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JSONFileClient));

        public JSONFileClient(string splitsFilePath, string segmentsFilePath)
        {
            InitializeLogger();
            var segmentFetcher = new JSONFileSegmentFetcher(segmentsFilePath);
            var splitParser = new SplitParser(segmentFetcher);
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
