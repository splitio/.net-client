using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.Services.Parsing;

namespace Net_SDK_Unit_Tests.Unit_Tests
{
    [TestClass]
    public class AllKeysMatcherTests
    {
        [TestMethod]
        public void MatchShouldReturnTrueForAnyKey()
        {
            //Arrange
            var matcher = new AllKeysMatcher();

            //Act
            var result = matcher.Match("test");

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfNull()
        {
            //Arrange
            var matcher = new AllKeysMatcher();

            //Act
            var result2 = matcher.Match(null);

            //Assert
            Assert.IsFalse(result2);
        }
    }
}
