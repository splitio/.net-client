using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.CommonLibraries;
using Splitio.Services.SplitFetcher;
using Splitio.Services.SplitFetcher.Classes;
using Splitio.Services.SegmentFetcher.Classes;
using Splitio.Services.Parsing;
using System.Threading;
using Splitio.Domain;
using Splitio.Services.EngineEvaluator;
using System.Collections.Generic;
using Splitio.Services.Client.Classes;

namespace Splitio_Tests.Integration_Tests
{
    [TestClass]
    public class EngineModuleTests
    {
        JSONFileSplitFetcher splitFetcher;

        [TestInitialize]
        public void Initialize()
        {
            log4net.Config.XmlConfigurator.Configure();
            
            var segmentFetcher = new JSONFileSegmentFetcher(@"segment_payed.json");
            var splitParser = new SplitParser(segmentFetcher);
            splitFetcher = new JSONFileSplitFetcher(@"splits_staging_julian.json", splitParser);
        }

        [DeploymentItem(@"Resources\splits_staging_julian.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw_4SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitFetcher.Fetch("test_jw4");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw4
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 9);
            dict.Add("test", "acdefx");
            var result = engine.GetTreatment("xadcc", split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            dict2.Add("test", "azzdefx");
            var result2 = engine.GetTreatment("xadcscdcc", split, dict2);

            Dictionary<string, object> dict3 = new Dictionary<string, object>();
            dict3.Add("date", 9);
            dict3.Add("test", "azzdefx");
            var result3 = engine.GetTreatment("abcdef", split, dict3);

            //Assert
            Assert.IsTrue(result == "on"); // in whitelist
            Assert.IsTrue(result2 == "off"); // default
            Assert.IsTrue(result3 == "on"); // included user abcdef (whitelist)
        }

        [DeploymentItem(@"Resources\splits_staging_julian.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw3_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitFetcher.Fetch("test_jw3");
      
            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 9);

            //Act
            //get treatment for split test_jw3
            var result = engine.GetTreatment("xadcc", split, dict);

            //Assert
            Assert.IsTrue(result == "off"); //<killed> default
        }

        [DeploymentItem(@"Resources\splits_staging_julian.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitFetcher.Fetch("test_jw");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 1470960000000);
            var result1 = engine.GetTreatment("1f84e5ddb06a3e66145ccfc1aac247", split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            var result2 = engine.GetTreatment("axdzcccczzcce66145ccfc1aac247", split, dict2);

            //Assert
            Assert.IsTrue(result1 == "on"); //date is equal
            Assert.IsTrue(result2 == "off"); //<else> in segment all
        }

        [DeploymentItem(@"Resources\splits_staging_julian.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw2_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitFetcher.Fetch("test_jw2");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw2
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 9);
            var result1 = engine.GetTreatment("abcdz", split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            var result2 = engine.GetTreatment("xadcc", split, dict2);

            //Assert
            Assert.IsTrue(result1 == "on"); //is in segment payed
            Assert.IsTrue(result2 == "off"); // default
        }
    }
}
