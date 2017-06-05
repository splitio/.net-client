using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.Parsing;
using Splitio.Services.Parsing.Classes;
using System;
using System.Collections.Generic;
using Moq;

namespace Splitio_Tests.Unit_Tests
{
    [TestClass]
    public class DependencyMatcherTests
    {
        [TestMethod]
        public void MatchShouldReturnTrueOnMatchingKey()
        {
            //Arrange
            var treatments = new List<string>(){"on"};
            var matcher = new DependencyMatcher("test1", treatments);
            var splitClientMock = new Mock<ISplitClient>();
            splitClientMock.Setup(x => x.GetTreatment("test", "test1", null, false)).Returns("on");
            //Act
            var result = matcher.Match("test", splitClientMock.Object);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseOnNonMatchingKey()
        {
            //Arrange
            var treatments = new List<string>() { "off" };
            var matcher = new DependencyMatcher("test1", treatments);
            var splitClientMock = new Mock<ISplitClient>();
            splitClientMock.Setup(x => x.GetTreatment("test", "test1", null, false)).Returns("on");
            //Act
            var result = matcher.Match("test", splitClientMock.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfNullSplitClient()
        {
            //Arrange
            var treatments = new List<string>();
            var matcher = new DependencyMatcher("test1", treatments);
            ISplitClient splitClient = null;

            //Act
            var result = matcher.Match("test2", splitClient);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfEmptyTreatmentList()
        {
            //Arrange
            var treatments = new List<string>();
            var matcher = new DependencyMatcher("test1", treatments);
            var splitClientMock = new Mock<ISplitClient>();
            splitClientMock.Setup(x => x.GetTreatment("test", "test1", null, false)).Returns("on");

            //Act
            var result = matcher.Match("test2", splitClientMock.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingLong()
        {
            //Arrange
            var treatments = new List<string>() { "on" };
            var matcher = new DependencyMatcher("test1", treatments);
            var splitClientMock = new Mock<ISplitClient>();
            splitClientMock.Setup(x => x.GetTreatment("test", "test1", null, false)).Returns("on");

            //Act
            var result = matcher.Match(123, splitClientMock.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingDate()
        {
            //Arrange
            var treatments = new List<string>() { "on" };
            var matcher = new DependencyMatcher("test1", treatments);
            var splitClientMock = new Mock<ISplitClient>();
            splitClientMock.Setup(x => x.GetTreatment("test", "test1", null, false)).Returns("on");

            //Act
            var result = matcher.Match(DateTime.UtcNow, splitClientMock.Object);

            //Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingList()
        {
            //Arrange
            var treatments = new List<string>() { "on" };
            var matcher = new DependencyMatcher("test1", treatments);
            var splitClientMock = new Mock<ISplitClient>();
            splitClientMock.Setup(x => x.GetTreatment("test", "test1", null, false)).Returns("on");

            //Act
            var result = matcher.Match(DateTime.UtcNow, splitClientMock.Object);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
