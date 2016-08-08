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
            //var splitParser = new SplitParser(new SelfRefreshingSegmentFetcher(new ApiSegmentChangeFetcher()));
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, null, 30, -1);
            selfRefreshingSplitFetcher.Start();

            //Act
            Split result = null;
            while (!selfRefreshingSplitFetcher.initialized)
            {
                Thread.Sleep(10);
            }
            selfRefreshingSplitFetcher.Stop();
            result = selfRefreshingSplitFetcher.Fetch("condition_and");

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.name == "condition_and");

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
            //var splitParser = new SplitParser(new SelfRefreshingSegmentFetcher(new SegmentChangeFetcher()));
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, null, 30, 1);
            selfRefreshingSplitFetcher.Start();

            //Act
            Split result = null;
            while (!selfRefreshingSplitFetcher.initialized)
            {
                Thread.Sleep(10);
            }
            selfRefreshingSplitFetcher.Stop();
            result = selfRefreshingSplitFetcher.Fetch("condition_and");


            //Assert
            Assert.IsTrue(result == null);
        
        }


    }
}
