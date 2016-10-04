using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Client.Classes;
using Splitio.Services.EngineEvaluator;
using Moq;
using System.Collections.Generic;
using Splitio.Domain;

namespace Splitio_Tests.Unit_Tests.Client
{
    [TestClass]
    public class SplitClientUnitTests
    {
        
        [TestMethod]
        [DeploymentItem(@"Resources\test.splits")]
        public void GetTreatmentShouldReturnControlIfSplitNotFound()
        {
            //Arrange
            var splitClient = new LocalhostClient("test.splits");
            
            //Act
            var result = splitClient.GetTreatment("test", "test");

            //Assert
            Assert.AreEqual("control", result);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.splits")]
        public void GetTreatmentShouldReturnControlIfExceptionIsThrown()
        {
           //Arrange
           Mock<Engine> engineMock = new Mock<Engine>();
            engineMock
                .Setup(x => x.GetTreatment(It.IsAny<string>(), It.IsAny<ParsedSplit>(), null))
                .Throws(new Exception());
            var splitClient = new LocalhostClient("test.splits", engineMock.Object);

            //Act
            var result = splitClient.GetTreatment("test", "other_test_feature");

            //Assert
            Assert.AreEqual("control", result);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.splits")]
        public void GetTreatmentShouldRunSuccessfullyOnEngineValidResponse()
        {
            //Arrange
            Mock<Engine> engineMock = new Mock<Engine>();
            engineMock
                .Setup(x => x.GetTreatment(It.IsAny<string>(), It.IsAny<ParsedSplit>(), null))
                .Returns("off");
            var splitClient = new LocalhostClient("test.splits", engineMock.Object);

            //Act
            var result = splitClient.GetTreatment("test", "other_test_feature");

            //Assert
            Assert.AreEqual("off", result);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\test.splits")]
        public void GetTreatmentShouldRunSuccessfullyOnEngineValidResponseUsingBucketingKey()
        {
            //Arrange
            Mock<Engine> engineMock = new Mock<Engine>();
            engineMock
                .Setup(x => x.GetTreatment(It.IsAny<Key>(), It.IsAny<ParsedSplit>(), null))
                .Returns("off");
            var splitClient = new LocalhostClient("test.splits", engineMock.Object);

            //Act
            Key key = new Key() { matchingKey = "test", bucketingKey ="a" };
            var result = splitClient.GetTreatment(key, "other_test_feature");

            //Assert
            Assert.AreEqual("off", result);
        }
    }
}
