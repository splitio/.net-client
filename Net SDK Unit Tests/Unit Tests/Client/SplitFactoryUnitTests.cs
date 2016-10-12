using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Client.Classes;

namespace Splitio_Tests.Unit_Tests.Client
{
    [TestClass]
    public class SplitFactoryUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(TimeoutException), "SDK was not ready in 1 miliseconds")]
        public void BuildSplitClientShouldReturnExceptionIfSdkNotReady()
        {
            //Arrange            
            var options = new ConfigurationOptions() { Ready = 1 };
            var factory = new SplitFactory("any", options);

            //Act         
            var client = factory.BuildSplitClient();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "API Key should be set to initialize Split SDK.")]
        public void BuildSplitClientWithEmptyApiKeyShouldReturnException()
        {
            //Arrange
            var factory = new SplitFactory(null, null);

            //Act         
            var client = factory.BuildSplitClient();
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.splits")]
        public void BuildSplitClientWithLocalhostApiKeyShouldReturnLocalhostClient()
        {
            //Arrange
            var options = new ConfigurationOptions() { LocalhostFilePath = "test.splits" };
            var factory = new SplitFactory("localhost", options);

            //Act         
            var client = factory.BuildSplitClient();

            //Assert
            Assert.AreEqual(typeof(LocalhostClient), client.GetType());
        }

        [TestMethod]
        public void BuildSplitClientWithApiKeyShouldReturnSelfRefreshingSplitClient()
        {
            //Arrange
            var factory = new SplitFactory("any", null);

            //Act         
            var client = factory.BuildSplitClient();

            //Assert
            Assert.AreEqual(typeof(SelfRefreshingClient), client.GetType());
        }
    }
}
