﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Client.Classes;
using Splitio.Domain;
using System.Collections.Generic;
using Splitio.Services.Impressions.Classes;
using Moq;
using Splitio.Services.Impressions.Interfaces;
using Splitio.Services.Cache.Interfaces;

namespace Splitio_Tests.Integration_Tests
{
    [TestClass]
    public class JSONFileClientTests
    {
        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentOnFailedParsingSplitShouldReturnCONTROL()
        {
            //Arrange
            var client = new JSONFileClient("splits_staging_3.json", "");

            //Act           
            var result = client.GetTreatment("test","fail", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("control", result);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentOnFailedParsingSplitShouldNotAffectOtherSplits()
        {
            //Arrange
            var client = new JSONFileClient("splits_staging_3.json", "");

            //Act           
            var result = client.GetTreatment("test", "asd", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("off", result);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentOnDeletedSplitShouldReturnControl()
        {
            //Arrange
            var client = new JSONFileClient("splits_staging_3.json", "");

            //Act           
            var result = client.GetTreatment("test", "asd", null);
            client.RemoveSplitFromCache("asd");
            var result2 = client.GetTreatment("test", "asd", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("off", result);
            Assert.IsNotNull(result2);
            Assert.AreEqual("control", result2);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        [DeploymentItem(@"Resources\segment_payed.json")]
        public void ExecuteGetTreatmentOnRemovedUserFromSegmentShouldReturnOff()
        {
            //Arrange
            var client = new JSONFileClient("splits_staging_3.json", "segment_payed.json");

            //Act           
            var result = client.GetTreatment("abcdz", "test_jw2", null);
            client.RemoveKeyFromSegmentCache("payed", new List<string>() { "abcdz" });
            var result2 = client.GetTreatment("abcdz", "test_jw2", null);


            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("on", result);
            Assert.IsNotNull(result2);
            Assert.AreEqual("off", result2);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_4.json")]
        public void ExecuteGetTreatmentOnSplitWithOnOffOnPartition()
        {
            //Arrange
            var client = new JSONFileClient("splits_staging_4.json", "");

            //Act           
            var result = client.GetTreatment("01", "Test_on_off_on", null);
            var result2 = client.GetTreatment("ab", "Test_on_off_on", null);
            var result3 = client.GetTreatment("00b0", "Test_on_off_on", null);


            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("on", result);
            Assert.IsNotNull(result2);
            Assert.AreEqual("off", result2);
            Assert.IsNotNull(result3);
            Assert.AreEqual("on", result3);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentOnSplitWithSegmentNotInitialized()
        {
            //Arrange
            var client = new JSONFileClient("splits_staging_3.json", "");

            //Act           
            //feature test_jw2 has UserDefinedSegmentMatcher 
            //on "payed" segment, and it is not initialized.
            var result = client.GetTreatment("abcdz", "test_jw2", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("off", result);
        }


        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentAndLogLabelKilled()
        {
            //Arrange
            var treatmentLogMock = new Mock<ITreatmentLog>();
            var client = new JSONFileClient("splits_staging_3.json", "", null, null, treatmentLogMock.Object);

            //Act           
            var result = client.GetTreatment("test", "test_jw3", null);

            //Assert
            treatmentLogMock.Verify(x => x.Log("test", "test_jw3", "off", It.IsAny<long>(), 1470947806420, "killed", "test"));
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentAndLogLabelNoConditionMatched()
        {
            //Arrange
            var treatmentLogMock = new Mock<ITreatmentLog>();
            var client = new JSONFileClient("splits_staging_3.json", "", null, null, treatmentLogMock.Object);

            //Act           
            var result = client.GetTreatment("test", "whitelisting_elements", null);

            //Assert
            treatmentLogMock.Verify(x => x.Log("test", "whitelisting_elements", "off", It.IsAny<long>(), 1471368078203, "no rule matched", "test"));
       
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentAndLogLabelSplitNotFound()
        {
            //Arrange
            var treatmentLogMock = new Mock<ITreatmentLog>();
            var client = new JSONFileClient("splits_staging_3.json", "", null, null, treatmentLogMock.Object);

            //Act           
            client.RemoveSplitFromCache("asd");
            var result = client.GetTreatment("test", "asd", null);

            //Assert
            treatmentLogMock.Verify(x => x.Log("test", "asd", "control", It.IsAny<long>(), null, "rules not found", "test"));
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentAndLogLabelException()
        {
            //Arrange
            var treatmentLogMock = new Mock<ITreatmentLog>();
            var splitCacheMock = new Mock<ISplitCache>();
            splitCacheMock.Setup(x => x.GetSplit(It.IsAny<string>())).Throws<Exception>();
            var client = new JSONFileClient("splits_staging_3.json", "", null, splitCacheMock.Object, treatmentLogMock.Object);

            //Act           
            var result = client.GetTreatment("test", "asd", null);

            //Assert
            treatmentLogMock.Verify(x => x.Log("test", "asd", "control", It.IsAny<long>(), null, "exception", "test"));
        }

        [TestMethod]
        [DeploymentItem(@"Resources\splits_staging_3.json")]
        public void ExecuteGetTreatmentAndLogLabelForTreatment()
        {
            //Arrange
            var treatmentLogMock = new Mock<ITreatmentLog>();
            var client = new JSONFileClient("splits_staging_3.json", "", null, null, treatmentLogMock.Object);

            //Act           
            var result = client.GetTreatment("db765170-e9f2-11e5-885c-c2f58c3a47a7", "Segments_Restructuring_UI", null);

            //Assert
            treatmentLogMock.Verify(x => x.Log("db765170-e9f2-11e5-885c-c2f58c3a47a7", "Segments_Restructuring_UI", "on", It.IsAny<long>(), 1484084207827, "explicitly included", "db765170-e9f2-11e5-885c-c2f58c3a47a7"));
  
        }

    }
}
