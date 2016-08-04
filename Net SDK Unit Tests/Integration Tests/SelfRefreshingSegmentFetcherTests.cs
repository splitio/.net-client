using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using System.Net;
using NetSDK.Services.SplitFetcher.Classes;
using NetSDK.Services.SplitFetcher;
using System.Threading;
using NetSDK.Domain;
using NetSDK.Services.SegmentFetcher.Classes;
using System.Collections.Generic;

namespace NetSDK.Tests
{
    [TestClass]
    public class SelfRefreshingSegmentFetcherTests
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
            var sdkApiClient = new SegmentSdkApiClient(httpHeader, baseUrl, 10000, 10000);
            var apiSegmentChangeFetcher = new ApiSegmentChangeFetcher(sdkApiClient);
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, new HashSet<SelfRefreshingSegment>(), 30);

            //Act
            SelfRefreshingSegment result = null;
            int i = 0;
            while (result == null && i < 10)
            {
                result = (SelfRefreshingSegment)selfRefreshingSegmentFetcher.Fetch("adil_segment");
                Thread.Sleep(100);
                i++;
            }

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.name == "adil_segment");
            Thread.Sleep(1000);
            Assert.IsTrue(result.Contains("eNXnLmFZfweB5i1Z5NF5"));

        }

    }
}
