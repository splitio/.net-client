using log4net;
using NetSDK.Services.EngineEvaluator;
using NetSDK.Services.Parsing;
using NetSDK.Services.SegmentFetcher.Classes;
using NetSDK.Services.SplitFetcher.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Client.Classes
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
