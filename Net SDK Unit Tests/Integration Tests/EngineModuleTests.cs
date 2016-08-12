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

namespace Net_SDK_Unit_Tests.Integration_Tests
{
    [TestClass]
    public class EngineModuleTests
    {
        [TestMethod]
        public void ExecuteGetTreatmentSuccessfulWithResults()
        {
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
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, new HashSet<SelfRefreshingSegment>(), 30);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, splitParser, 30, -1);
            selfRefreshingSplitFetcher.Start();

            while (!selfRefreshingSplitFetcher.initialized)
            {
                Thread.Sleep(10);
            }
            selfRefreshingSplitFetcher.Stop();
            ParsedSplit split = selfRefreshingSplitFetcher.Fetch("test_jw");
            ParsedSplit split2 = selfRefreshingSplitFetcher.Fetch("test_jw2");
            ParsedSplit split3 = selfRefreshingSplitFetcher.Fetch("test_jw3");
            ParsedSplit split4 = selfRefreshingSplitFetcher.Fetch("test_jw4");

            Splitter splitter = new Splitter();
            Engine engine = new Engine(splitter);

            //Act
            //get treatment for split test_jw
            Dictionary<string, object> dict = new Dictionary<string,object>();
            dict.Add("date", 1470960000000);
            var result1 = engine.GetTreatment("1f84e5ddb06a3e66145ccfc1aac247", split, dict);

            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            dict2.Add("date", 9);
            var result2 = engine.GetTreatment("axdzcccczzcce66145ccfc1aac247", split, dict2);

            //get treatment for split test_jw2
            Dictionary<string, object> dict3 = new Dictionary<string, object>();
            dict3.Add("date", 9);
            var result3 = engine.GetTreatment("abcdz", split2, dict3);

            Dictionary<string, object> dict4 = new Dictionary<string, object>();
            dict4.Add("date", 9);
            var result4 = engine.GetTreatment("xadcc", split2, dict4);

            //get treatment for split test_jw3
            var result5 = engine.GetTreatment("xadcc", split3, dict4);


            //get treatment for split test_jw4
            Dictionary<string, object> dict5 = new Dictionary<string, object>();
            dict5.Add("date", 9);
            dict5.Add("test", "acdefx");
            var result6 = engine.GetTreatment("xadcc", split4, dict5);

            Dictionary<string, object> dict6 = new Dictionary<string, object>();
            dict6.Add("date", 9);
            dict6.Add("test", "azzdefx");
            var result7 = engine.GetTreatment("xadcscdcc", split4, dict6);

            Dictionary<string, object> dict7 = new Dictionary<string, object>();
            dict7.Add("date", 9);
            dict7.Add("test", "azzdefx");
            var result8 = engine.GetTreatment("abcdef", split4, dict7);

            //Assert
            Assert.IsTrue(result1 == "on"); //date is equal
            Assert.IsTrue(result2 == "off"); //<else> in segment all
            Assert.IsTrue(result3 == "on"); //is in segment payed
            Assert.IsTrue(result4 == "off"); // default
            Assert.IsTrue(result5 == "off"); //<killed> default
            Assert.IsTrue(result6 == "on"); // in whitelist
            Assert.IsTrue(result7 == "off"); // default
            Assert.IsTrue(result8 == "on"); // included user abcdef (whitelist)
        }
    }
}
