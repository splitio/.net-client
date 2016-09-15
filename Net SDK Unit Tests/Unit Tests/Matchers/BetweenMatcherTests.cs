using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Parsing;
using Splitio.Domain;

namespace Splitio_Tests.Unit_Tests
{
    [TestClass]
    public class BetweenMatcherTests
    {
        [TestMethod]
        public void MatchNumberSuccesfully()
        {
            //Arrange
            var matcher = new BetweenMatcher(DataTypeEnum.NUMBER, 1000001, 10540001);

            //Act
            var result1 = matcher.Match("1700000");
            var result2 = matcher.Match("545345");
            var result3 = matcher.Match("98981700000");

            //Assert
            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }

        [TestMethod]
        public void MatchNumberShouldReturnFalseOnInvalidNumber()
        {
            //Arrange
            var matcher = new BetweenMatcher(DataTypeEnum.NUMBER, 1000001, 10540001);

            //Act
            var result = matcher.Match("1aaaaa0");

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchDateSuccesfully()
        {
            //Arrange
            var matcher = new BetweenMatcher(DataTypeEnum.DATETIME, 1470960000000, 1480960000000);

            //Act
            var result = matcher.Match("1470970000000");
            var result1 = matcher.Match("1470910000000");
            var result2 = matcher.Match("1490910000000");

            //Assert
            Assert.IsTrue(result);
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void MatchDateShouldReturnFalseOnInvalidDate()
        {
            //Arrange
            var matcher = new BetweenMatcher(DataTypeEnum.DATETIME, 1470960000000, 1480960000000);

            //Act
            var result = matcher.Match("1aaa0000000");
            
            //Assert
            Assert.IsFalse(result);

        }

        [TestMethod]
        public void MatchShouldReturnFalseOnInvalidDataType()
        {
            //Arrange
            var matcher = new BetweenMatcher(DataTypeEnum.STRING, 1470960000000, 1480960000000);

            //Act
            var result = matcher.Match("abcd");

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfNullOrEmpty()
        {
            //Arrange
            var matcher = new BetweenMatcher(DataTypeEnum.DATETIME, 1470960000000, 1480960000000);

            //Act
            var result = matcher.Match("");
            var result2 = matcher.Match(null);

            //Assert
            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }
    }
}
