using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using System.Net;


namespace NetSDK.Tests
{
    [TestClass]
    public class SdkApiClientTests
    {
        [TestMethod]
        public void ExecuteGetSuccessful()
        {
            //Arrange
            var baseUrl = "http://demo7064886.mockable.io";
            var httpHeader = new HTTPHeader()
            {
                AuthorizationApiKey = "ABCD",
                Encoding = "gzip",
                SplitSDKMachineIP = "1.0.0.0",
                SplitSDKMachineName = "localhost",
                SplitSDKVersion = "1",
                SplitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg1");

            //Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsTrue(result.Content.Contains("Hello World"));
        }


        [TestMethod]
        public void ExecuteGetShouldReturnErrorNotAuthorized()
        {
            //Arrange
            var baseUrl = "http://demo7064886.mockable.io";
            var httpHeader = new HTTPHeader()
            {
                Encoding = "gzip",
                SplitSDKMachineIP = "1.0.0.0",
                SplitSDKMachineName = "localhost",
                SplitSDKVersion = "1",
                SplitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg2");

            //Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void ExecuteGetShouldThrowExceptionOnInvalidAddress()
        {
            //Arrange
            var baseUrl = "http://demo706abcd.mockable.io";
            var httpHeader = new HTTPHeader()
            {
                AuthorizationApiKey = "ABCD",
                Encoding = "gzip",
                SplitSDKMachineIP = "1.0.0.0",
                SplitSDKMachineName = "localhost",
                SplitSDKVersion = "1",
                SplitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg2");

            //Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }
    }
}
