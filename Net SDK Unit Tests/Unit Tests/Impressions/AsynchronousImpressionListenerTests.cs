using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Splitio.Domain;
using Splitio.Services.Shared.Classes;
using Splitio.Services.Shared.Interfaces;
using System;
using System.Threading;

namespace Splitio_Tests.Unit_Tests.Impressions
{
    [TestClass]
    public class AsynchronousImpressionListenerTests
    {
        [TestMethod]
        public void AddListenerAndPerformLogSuccessfully()
        {
            //Arrange
            var asyncListener = new AsynchronousListener<KeyImpression>();
            var listenerMock = new Mock<IListener<KeyImpression>>();
            asyncListener.AddListener(listenerMock.Object);

            //Act
            asyncListener.Log(new KeyImpression() { feature = "test", changeNumber = 100, keyName = "date", label = "testdate", time = 10000000, treatment = "on", bucketingKey = "any" });
            Thread.Sleep(1000);

            //Assert
            listenerMock.Verify(x => x.Log(It.Is<KeyImpression>(p => p.keyName == "date" && p.feature == "test" && p.treatment == "on" && p.time == 10000000 && p.changeNumber == 100 && p.label == "testdate" && p.bucketingKey == "any")));
        }

        [TestMethod]
        public void AddTwoListenersAndPerformLogSuccessfully()
        {
            //Arrange
            var asyncListener = new AsynchronousListener<KeyImpression>();
            var listenerMock1 = new Mock<IListener<KeyImpression>>();
            var listenerMock2 = new Mock<IListener<KeyImpression>>();
            asyncListener.AddListener(listenerMock1.Object);
            asyncListener.AddListener(listenerMock2.Object);


            //Act
            asyncListener.Log(new KeyImpression() { feature = "test", changeNumber = 100, keyName = "date", label = "testdate", time = 10000000, treatment = "on", bucketingKey = "any" });
            Thread.Sleep(1000);

            //Assert
            listenerMock1.Verify(x => x.Log(It.Is<KeyImpression>(p => p.keyName == "date" && p.feature == "test" && p.treatment == "on" && p.time == 10000000 && p.changeNumber == 100 && p.label == "testdate" && p.bucketingKey == "any")), Times.Once());
            listenerMock2.Verify(x => x.Log(It.Is<KeyImpression>(p => p.keyName == "date" && p.feature == "test" && p.treatment == "on" && p.time == 10000000 && p.changeNumber == 100 && p.label == "testdate" && p.bucketingKey == "any")), Times.Once());
        }

        [TestMethod]
        public void AddTwoListenersAndPerformLogSuccessfullyWhenOneListenerFails()
        {
            //Arrange
            var asyncListener = new AsynchronousListener<KeyImpression>();
            var listenerMock1 = new Mock<IListener<KeyImpression>>();
            listenerMock1.Setup(x => x.Log(It.IsAny<KeyImpression>())).Throws(new Exception());
            var listenerMock2 = new Mock<IListener<KeyImpression>>();
            asyncListener.AddListener(listenerMock1.Object);
            asyncListener.AddListener(listenerMock2.Object);


            //Act
            asyncListener.Log(new KeyImpression() { feature = "test", changeNumber = 100, keyName = "date", label = "testdate", time = 10000000, treatment = "on", bucketingKey = "any" });
            Thread.Sleep(1000);

            //Assert
            listenerMock2.Verify(x => x.Log(It.Is<KeyImpression>(p => p.keyName == "date" && p.feature == "test" && p.treatment == "on" && p.time == 10000000 && p.changeNumber == 100 && p.label == "testdate" && p.bucketingKey == "any")), Times.Once());
        }
    }
}
