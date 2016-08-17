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
using NetSDK.Services.Client;
using NetSDK.Services.Parsing;

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
            var gates = new SdkReadinessGates();
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, gates, null, 30);

            //Act
           

            var result = (SelfRefreshingSegment)selfRefreshingSegmentFetcher.Fetch("adil_segment");
            var segmentsToRegister = new HashSet<string>();
            segmentsToRegister.Add(result.name);
            gates.RegisterSegments(segmentsToRegister);
            result.Start();

            gates.AreSegmentsReady(1000);

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.name == "adil_segment");
            Assert.IsTrue(result.Contains("eNXnLmFZfweB5i1Z5NF5"));

        }

    }
}
