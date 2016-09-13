using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.CommonLibraries;
using System.Net;
using Splitio.Services.SplitFetcher.Classes;
using Splitio.Services.SplitFetcher;
using System.Threading;
using Splitio.Domain;
using Splitio.Services.Parsing;
using Splitio.Services.SegmentFetcher.Classes;
using System.Collections.Generic;
using log4net;
using Splitio.Services.Client.Classes;

namespace Splitio_Tests.Integration_Tests
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
        [DeploymentItem(@"Resources\splits_staging.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]

        public void ExecuteGetSuccessfulWithResultsFromJSONFile()
        {
            //Arrange
            var splitParser = new SplitParser(new JSONFileSegmentFetcher("segment_payed.json"));
            var splitFetcher = new JSONFileSplitFetcher("splits_staging.json", splitParser);

            //Act           
            ParsedSplit result = splitFetcher.Fetch("Pato_Test_1");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.name == "Pato_Test_1");
            Assert.IsTrue(result.conditions.Count > 0);
        }

        [TestMethod]
        [Ignore] 
        public void ExecuteGetSuccessfulWithResults()
        {
            //Arrange
            var baseUrl = "https://sdk-aws-staging.split.io/api/";
            //var baseUrl = "http://localhost:3000/api/";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "///PUT API KEY HERE///",
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
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, gates, 30, null);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, splitParser, gates, 30);
            selfRefreshingSplitFetcher.Start();

            //Act           
            gates.IsSDKReady(1000);
            selfRefreshingSplitFetcher.Stop();
            ParsedSplit result  = selfRefreshingSplitFetcher.Fetch("Pato_Test_1");
            ParsedSplit result2 = selfRefreshingSplitFetcher.Fetch("Manu_Test_1");
            //Assert
            Assert.IsNotNull(result);
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
                authorizationApiKey = "0",
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
            var selfRefreshingSegmentFetcher = new SelfRefreshingSegmentFetcher(apiSegmentChangeFetcher, gates, 30, null);

            var splitParser = new SplitParser(selfRefreshingSegmentFetcher);
            var selfRefreshingSplitFetcher = new SelfRefreshingSplitFetcher(apiSplitChangeFetcher, splitParser, gates, 30);
            selfRefreshingSplitFetcher.Start();

            //Act
            gates.IsSDKReady(10);

            var result = selfRefreshingSplitFetcher.Fetch("condition_and");

            //Assert
            Assert.IsNull(result);
        
        }


    }
}
