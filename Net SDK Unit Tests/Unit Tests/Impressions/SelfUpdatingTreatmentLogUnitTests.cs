using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Impressions.Classes;
using Splitio.Domain;
using System.Threading;
using Moq;
using Splitio.Services.Impressions.Interfaces;

namespace Splitio_Tests.Unit_Tests.Impressions
{
    [TestClass]
    public class SelfUpdatingTreatmentLogUnitTests
    {
        [TestMethod]
        public void LogSuccessfully()
        {
            //Arrange
            var queue = new BlockingQueue<KeyImpression>(10);
            var treatmentLog = new SelfUpdatingTreatmentLog(null, 1000, queue, 10);

            //Act
            treatmentLog.Log("GetTreatment", "test", "on", 7000);

            //Assert
            Thread.Sleep(5000);
            var element = queue.Dequeue();
            Assert.IsNotNull(element);
            Assert.AreEqual("GetTreatment", element.keyName);
            Assert.AreEqual("test", element.feature);
            Assert.AreEqual("on", element.treatment);
            Assert.AreEqual(7000, element.time);
        }

        [TestMethod]
        public void LogSuccessfullyAndSendImpressions()
        {
            //Arrange
            var apiClientMock = new Mock<ITreatmentSdkApiClient>();
            var queue = new BlockingQueue<KeyImpression>(10);
            var treatmentLog = new SelfUpdatingTreatmentLog(apiClientMock.Object, 1, queue, 10);

            //Act
            treatmentLog.Start();
            treatmentLog.Log("GetTreatment", "test", "on", 7000);

            //Assert
            Thread.Sleep(5000);
            apiClientMock.Verify(x => x.SendBulkImpressions(It.IsAny<string>()));
        }
    }
}
