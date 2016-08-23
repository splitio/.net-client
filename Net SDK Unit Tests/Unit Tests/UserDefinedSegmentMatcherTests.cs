﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Parsing;
using Splitio.Domain;
using System.Collections.Generic;

namespace Splitio_Tests.Unit_Tests
{
    [TestClass]
    public class UserDefinedSegmentMatcherTests
    {
        [TestMethod]
        public void MatchShouldReturnTrueOnMatchingSegment()
        {
            //Arrange
            var keys = new HashSet<string>();
            keys.Add("test1");
            keys.Add("test2");
            var segment = new Segment("test-segment", -1, keys);
            var matcher = new UserDefinedSegmentMatcher(segment);

            //Act
            var result = matcher.Match("test2");

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseOnNonMatchingSegment()
        {
            //Arrange
            var keys = new HashSet<string>();
            keys.Add("test1");
            keys.Add("test2");
            var segment = new Segment("test-segment", -1, keys);
            var matcher = new UserDefinedSegmentMatcher(segment);

            //Act
            var result = matcher.Match("test3");

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchShouldReturnFalseIfSegmentEmpty()
        {
            //Arrange
            var segment = new Segment("test-segment");
            var matcher = new UserDefinedSegmentMatcher(segment);

            //Act
            var result = matcher.Match("test2");

            //Assert
            Assert.IsFalse(result);
        }
    }
}
