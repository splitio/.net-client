using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using System.Net;
using NetSDK.Services.SplitFetcher.Classes;
using NetSDK.Services.SplitFetcher;
using System.Threading;
using NetSDK.Domain;
using NetSDK.Services.Parsing;
using NetSDK.Services.SegmentFetcher.Classes;
using System.Collections.Generic;
using log4net;

namespace NetSDK.Tests
{
    [TestClass]
    public class SelfRefreshingSplitFetcherTests
    {
        [TestInitialize]
        public void Initialization()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [TestMethod]
        public void ExecuteGetSuccessfulWithResults()
        {
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            //var baseUrl = "http://localhost:3000/api/";
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
            var sdkSegmentApiClient = new SegmentSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSegmentChangeFetcher = new ApiSegmentChangeFetcher(sdkSegmentApiClient);
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, new HashSet<SelfRefreshingSegment>(), 30);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, splitParser, 30, -1);
            selfRefreshingSplitFetcher.Start();

            //Act           
            while (!selfRefreshingSplitFetcher.initialized)
            {
                Thread.Sleep(10);
            }
            selfRefreshingSplitFetcher.Stop();
            ParsedSplit result  = selfRefreshingSplitFetcher.Fetch("Pato_Test_1");
            ParsedSplit result2 = selfRefreshingSplitFetcher.Fetch("Manu_Test_1");
            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.initialized);
            Assert.IsTrue(result2.initialized);
            Assert.IsTrue(result.name == "Pato_Test_1");
            Assert.IsTrue(result.conditions.Count > 0);

        }

        [TestMethod]
        public void ExecuteGetWithoutResults()
        {
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "43sdqmuqt5tvbjtl3e3t2i8ps4x",
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
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, splitParser, 30, 1);
            selfRefreshingSplitFetcher.Start();

            //Act
            ParsedSplit result = null;
            int i = 0;
            while (!selfRefreshingSplitFetcher.initialized && i<20)
            {
                Thread.Sleep(10);
                i++;
            }
            selfRefreshingSplitFetcher.Stop();
            result = selfRefreshingSplitFetcher.Fetch("condition_and");


            //Assert
            Assert.IsTrue(result == null);
        
        }


    }
}
