﻿using System;
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
using Splitio.Services.Cache.Classes;
using System.Collections.Concurrent;
using System.Linq;

namespace Splitio_Tests.Integration_Tests
{
    [TestClass]
    public class EngineModuleTests
    {
        InMemorySplitCache splitCache;

        [TestInitialize]
        public void Initialize()
        {
            log4net.Config.XmlConfigurator.Configure();

            var segmentCache = new InMemorySegmentCache(new ConcurrentDictionary<string, Segment>());
            var segmentFetcher = new JSONFileSegmentFetcher(@"segment_payed.json", segmentCache);
            var splitParser = new SplitParser(segmentFetcher, segmentCache);
            var splitChangeFetcher = new JSONFileSplitChangeFetcher("splits_staging_2.json");
            var splitChangesResult = splitChangeFetcher.Fetch(-1);
            splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>(
                splitChangesResult.splits.Select(x => new KeyValuePair<string, ParsedSplit>(x.name, splitParser.Parse(x)))
            )); 
        }

        [DeploymentItem(@"Resources\splits_staging_2.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw_4SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitCache.GetSplit("test_jw4");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw4
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 9);
            dict.Add("test", "acdefx");
            var result = engine.GetTreatment(new Key("xadcc", null), split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            dict2.Add("test", "azzdefx");
            var result2 = engine.GetTreatment(new Key("xadcscdcc", null), split, dict2);

            Dictionary<string, object> dict3 = new Dictionary<string, object>();
            dict3.Add("date", 9);
            dict3.Add("test", "azzdefx");
            var result3 = engine.GetTreatment(new Key("abcdef", null), split, dict3);

            //Assert
            Assert.IsTrue(result == "on"); // in whitelist
            Assert.IsTrue(result2 == "off"); // default
            Assert.IsTrue(result3 == "on"); // included user abcdef (whitelist)
        }

        [DeploymentItem(@"Resources\splits_staging_2.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw3_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitCache.GetSplit("test_jw3");
      
            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 9);

            //Act
            //get treatment for split test_jw3
            var result = engine.GetTreatment(new Key("xadcc", null), split, dict);

            //Assert
            Assert.IsTrue(result == "off"); //<killed> default
        }

        [DeploymentItem(@"Resources\splits_staging_2.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitCache.GetSplit("test_jw");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 1470960000000);
            var result1 = engine.GetTreatment(new Key("1f84e5ddb06a3e66145ccfc1aac247", null), split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            var result2 = engine.GetTreatment(new Key("axdzcccczzcce66145ccfc1aac247", null), split, dict2);

            //Assert
            Assert.IsTrue(result1 == "on"); //date is equal
            Assert.IsTrue(result2 == "off"); //<else> in segment all
        }

        [DeploymentItem(@"Resources\splits_staging_2.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw2_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = splitCache.GetSplit("test_jw2");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw2
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("date", 9);
            var result1 = engine.GetTreatment(new Key("abcdz", null), split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            var result2 = engine.GetTreatment(new Key("xadcc", null), split, dict2);

            //Assert
            Assert.IsTrue(result1 == "on"); //is in segment payed
            Assert.IsTrue(result2 == "off"); // default
        }

        [DeploymentItem(@"Resources\splits_staging_2.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw2_SuccessfullyUsingBucketingKey()
        {
            //Arrange
            ParsedSplit split = splitCache.GetSplit("test_jw2_b");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw2_b
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Key key = new Key(bucketingKey:"ab", matchingKey:"abcdz");
            var result1 = engine.GetTreatment(key, split, dict);

            key = new Key(bucketingKey:"abcdzsdsadasd345", matchingKey:"abcdz");
            var result2 = engine.GetTreatment(key, split, dict);

            //Assert
            Assert.IsTrue(result1 == "off"); //is in segment payed and bucketing key matches partition off
            Assert.IsTrue(result2 == "on"); //is in segment payed and bucketing key matches partition on
        }
    }
}
