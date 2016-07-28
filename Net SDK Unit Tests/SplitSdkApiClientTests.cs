using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using System.Net;
using NetSDK.Services.SplitFetcher;


namespace NetSDK.Tests
{
    [TestClass]
    public class SplitSdkApiClientTests
    {
        [TestMethod]
        public void ExecuteFetchSplitChangesSuccessful()
        {
            //Arrange
            var baseUrl = "https://sdk.aws.staging.split.io/api";
            var httpHeader = new HTTPHeader()
            {
                AuthorizationApiKey = "43sdqmuqt5tvbjtl3e3t2i8ps4",
                Encoding = "gzip",
                SplitSDKMachineIP = "1.0.0.0",
                SplitSDKMachineName = "localhost",
                SplitSDKVersion = "net-0.0.0",
                SplitSDKSpecVersion = "1.2"
            };
            var SplitSdkApiClient = new SplitSdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SplitSdkApiClient.FetchSplitChanges("-1");

            //Assert
            Assert.IsTrue(result.Contains("splits"));
        }


        [TestMethod]
        public void ExecuteGetShouldReturnEmptyIfNotAuthorized()
        {
            //Arrange
            var baseUrl = "https://sdk.aws.staging.split.io/api";
            var httpHeader = new HTTPHeader()
            {
                Encoding = "gzip",
                SplitSDKMachineIP = "1.0.0.0",
                SplitSDKMachineName = "localhost",
                SplitSDKVersion = "net-0.0.0",
                SplitSDKSpecVersion = "1.2"
            };
            var SplitSdkApiClient = new SplitSdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SplitSdkApiClient.FetchSplitChanges("-1");

            //Assert
            Assert.IsTrue(result == String.Empty);
        }
    }
}
