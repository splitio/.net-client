using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.CommonLibraries;
using System.Net;


namespace NetSDK.Tests
{
    [TestClass]
    public class SdkApiClientTests
    {
        [TestInitialize]
        public void Initialization()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [TestMethod]
        public void ExecuteGetSuccessful()
        {
            //Arrange
            var baseUrl = "http://demo7064886.mockable.io";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "ABCD",
                encoding = "gzip",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "1",
                splitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg1");

            //Assert
            Assert.AreEqual(result.statusCode, HttpStatusCode.OK);
            Assert.IsTrue(result.content.Contains("Hello World"));
        }


        [TestMethod]
        public void ExecuteGetShouldReturnErrorNotAuthorized()
        {
            //Arrange
            var baseUrl = "http://demo7064886.mockable.io";
            var httpHeader = new HTTPHeader()
            {
                encoding = "gzip",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "1",
                splitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg2");

            //Assert
            Assert.AreEqual(result.statusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void ExecuteGetShouldReturnNotFoundOnInvalidRequest()
        {
            //Arrange
            var baseUrl = "http://demo706abcd.mockable.io";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "ABCD",
                encoding = "gzip",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "1",
                splitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg2");

            //Assert
            Assert.AreEqual(result.statusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ExecuteGetShouldReturnEmptyResponseOnInvalidURL()
        {
            //Arrange
            var baseUrl = "http://demo70e.iio";
            var httpHeader = new HTTPHeader()
            {
                authorizationApiKey = "ABCD",
                encoding = "gzip",
                splitSDKMachineIP = "1.0.0.0",
                splitSDKMachineName = "localhost",
                splitSDKVersion = "1",
                splitSDKSpecVersion = "2"
            };
            var SdkApiClient = new SdkApiClient(httpHeader, baseUrl, 10000, 10000);

            //Act
            var result = SdkApiClient.ExecuteGet("/messages?item=msg2");

            //Assert
            Assert.IsTrue(result.content == null);
            Assert.IsTrue(result != null);
        }
    }
}
