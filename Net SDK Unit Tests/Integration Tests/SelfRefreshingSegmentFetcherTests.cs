using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.CommonLibraries;
using System.Net;
using Splitio.Services.SplitFetcher.Classes;
using Splitio.Services.SplitFetcher;
using System.Threading;
using Splitio.Domain;
using Splitio.Services.SegmentFetcher.Classes;
using System.Collections.Generic;
using Splitio.Services.Parsing;
using Splitio.Services.Client.Classes;

namespace Splitio_Tests.Integration_Tests
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
        [DeploymentItem(@"Resources\segment_payed.json")]
        public void ExecuteGetSuccessfulWithResultsFromJSONFile()
        {
            //Arrange
            var segmentFetcher = new JSONFileSegmentFetcher("segment_payed.json");

            //Act
            var result = segmentFetcher.Fetch("payed");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.name == "payed");
            Assert.IsTrue(result.Contains("abcdz"));
        }


        [TestMethod]
        [Ignore] 
        public void ExecuteGetSuccessfulWithResults()
        {
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "///PUT API KEY HERE///",
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
            var result = (SelfRefreshingSegment)selfRefreshingSegmentFetcher.Fetch("payed");
            var segmentsToRegister = new List<string>();
            segmentsToRegister.Add(result.name);
            gates.RegisterSegments(segmentsToRegister);
            result.Start();

            while(!gates.AreSegmentsReady(1000))
            {
                Thread.Sleep(10);
            }

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.name == "payed");
            Assert.IsTrue(result.Contains("abcdz"));

        }

    }
}
