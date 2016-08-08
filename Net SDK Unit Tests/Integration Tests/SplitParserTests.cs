using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using NetSDK.Services.SplitFetcher;
using NetSDK.Services.SplitFetcher.Classes;
using NetSDK.Domain;
using System.Threading;
using NetSDK.Services.Parsing;
using NetSDK.Services.SegmentFetcher.Classes;
using System.Collections.Generic;

namespace Net_SDK_Unit_Tests.Integration_Tests
{
    [TestClass]
    public class SplitParserTests
    {
        [TestMethod]
        public void ParseSplitSuccessfully()
        {
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "43sdqmuqt5tvbjtl3e3t2i8ps4",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "net-0.0.0",
                splitSDKSpecVersion = "1.2",
                encoding = "gzip"
            };
            var sdkApiClient = new SplitSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSplitChangeFetcher = new ApiSplitChangeFetcher(sdkApiClient);
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, null, 30, -1);
            selfRefreshingSplitFetcher.Start();

            Split split = null;
            while (!selfRefreshingSplitFetcher.initialized)
            {
                Thread.Sleep(10);
            }
            selfRefreshingSplitFetcher.Stop();
            split = selfRefreshingSplitFetcher.Fetch("recentlyViewedKeys_aurora");

            var sdkSegmentApiClient = new SegmentSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSegmentChangeFetcher = new ApiSegmentChangeFetcher(sdkSegmentApiClient);
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, new HashSet<SelfRefreshingSegment>(), 30);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);

            //Act
            var result = splitParser.Parse(split);

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.name == "recentlyViewedKeys_aurora");
            Assert.IsTrue(result.conditions.Count > 0);
        }

        [TestMethod]
        public void ParseSplitShouldReturnNullOnError()
        {
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "43sdqmuqt5tvbjtl3e3t2i8ps4",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "net-0.0.0",
                splitSDKSpecVersion = "1.2",
                encoding = "gzip"
            };
            var sdkApiClient = new SplitSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSplitChangeFetcher = new ApiSplitChangeFetcher(sdkApiClient);
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, null, 30, -1);
            selfRefreshingSplitFetcher.Start();

            Split split = null;
            while (!selfRefreshingSplitFetcher.initialized)
            {
                Thread.Sleep(10);
            }
            selfRefreshingSplitFetcher.Stop();
            split = selfRefreshingSplitFetcher.Fetch("recently");

            var sdkSegmentApiClient = new SegmentSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSegmentChangeFetcher = new ApiSegmentChangeFetcher(sdkSegmentApiClient);
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, new HashSet<SelfRefreshingSegment>(), 30);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);

            //Act
            var result = splitParser.Parse(split);

            //Assert
            Assert.IsTrue(result == null);
        }
    }
}
