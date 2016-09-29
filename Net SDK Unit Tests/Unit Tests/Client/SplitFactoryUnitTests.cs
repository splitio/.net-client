using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Client.Classes;

namespace Splitio_Tests.Unit_Tests.Client
{
    [TestClass]
    public class SplitFactoryUnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception), "API Key should be set to initialize Split SDK.")]
        public void BuildSplitClientWithEmptyApiKeyShouldReturnException()
        {
            //Arrange
            var factory = new SplitFactory();

            //Act         
            var client = factory.BuildSplitClient(null, null);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.splits")]
        public void BuildSplitClientWithLocalhostApiKeyShouldReturnLocalhostClient()
        {
            //Arrange
            var factory = new SplitFactory();

            //Act         
            var options = new ConfigurationOptions() { LocalhostFilePath = "test.splits" };

            var client = factory.BuildSplitClient("localhost", options);

            //Assert
            Assert.AreEqual(typeof(LocalhostClient), client.GetType());
        }

        [TestMethod]
        public void BuildSplitClientWithApiKeyShouldReturnSelfRefreshingSplitClient()
        {
            //Arrange
            var factory = new SplitFactory();

            //Act         
            var client = factory.BuildSplitClient("any", null);

            //Assert
            Assert.AreEqual(typeof(SelfRefreshingClient), client.GetType());
        }
    }
}
