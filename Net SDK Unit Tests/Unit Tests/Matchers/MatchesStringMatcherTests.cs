using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Parsing;
using Splitio.Services.Parsing.Classes;
using System;
using System.Collections.Generic;

namespace Splitio_Tests.Unit_Tests
{
    [TestClass]
    public class MatchesStringMatcherTests
    {
        [TestMethod]
        public void MatchShouldReturnTrueOnMatchingKey()
        {
            //Arrange
            var matcher = new MatchesStringMatcher("^a");

            //Act
            var result = matcher.Match("arrive");

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseOnNonMatchingKey()
        {
            //Arrange
            var matcher = new MatchesStringMatcher("^a");

            //Act
            var result = matcher.Match("split");

            //Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingLong()
        {
            //Arrange
            var matcher = new MatchesStringMatcher("^a");

            //Act
            var result = matcher.Match(123);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingDate()
        {
            //Arrange
            var matcher = new MatchesStringMatcher("^a");

            //Act
            var result = matcher.Match(DateTime.UtcNow);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingBoolean()
        {
            //Arrange
            var matcher = new MatchesStringMatcher("^a");

            //Act
            var result = matcher.Match(true);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfMatchingSet()
        {
            //Arrange
            var matcher = new MatchesStringMatcher("^a");

            //Act
            var keys = new List<string>();
            keys.Add("test1");
            keys.Add("test3");

            var result = matcher.Match(keys);

            //Assert
            Assert.IsFalse(result);
        }

    }
}
