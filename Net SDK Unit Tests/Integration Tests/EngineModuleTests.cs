using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using NetSDK.Services.SplitFetcher;
using NetSDK.Services.SplitFetcher.Classes;
using NetSDK.Services.SegmentFetcher.Classes;
using NetSDK.Services.Parsing;
using System.Threading;
using NetSDK.Domain;
using NetSDK.Services.EngineEvaluator;
using System.Collections.Generic;
using NetSDK.Services.Client.Classes;

namespace Net_SDK_Unit_Tests.Integration_Tests
{
    [TestClass]
    public class EngineModuleTests
    {
        SelfRefreshingSplitFetcher selfRefreshingSplitFetcher;

        [TestInitialize]
        public void Initialize()
        {
            log4net.Config.XmlConfigurator.Configure();
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            //var baseUrl = "http://localhost:3000/api/";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "5p2c0r4so20ill66lm35i45h6pkvrd2skmib",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "net-0.0.0",
                splitSDKSpecVersion = "1.2",
                encoding = "gzip"
            };
            var sdkApiClient = new SplitSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSplitChangeFetcher = new ApiSplitChangeFetcher(sdkApiClient);
            var sdkSegmentApiClient = new SegmentSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSegmentChangeFetcher = new ApiSegmentChangeFetcher(sdkSegmentApiClient);
            var gates = new SdkReadinessGates();
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, gates, null, 30);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, splitParser, gates, 30, -1);
            selfRefreshingSplitFetcher.Start();

            while(!gates.IsSDKReady(1000))
            {
                Thread.Sleep(10);
            }
        }
        [TestMethod]
        public void ExecuteGetTreatment_Test_jw_4SuccessfulWithResults()
        {         
            ParsedSplit split = selfRefreshingSplitFetcher.Fetch("test_jw4");

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

        [TestMethod]
        public void ExecuteGetTreatment_Test_jw3_SuccessfulWithResults()
        {
            ParsedSplit split = selfRefreshingSplitFetcher.Fetch("test_jw3");
      
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


        [TestMethod]
        public void ExecuteGetTreatment_Test_jw_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = selfRefreshingSplitFetcher.Fetch("test_jw");

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


        [TestMethod]
        public void ExecuteGetTreatment_Test_jw2_SuccessfulWithResults()
        {
            //Arrange
            ParsedSplit split = selfRefreshingSplitFetcher.Fetch("test_jw2");

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
