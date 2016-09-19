using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Cache.Classes;
using System.Collections.Concurrent;
using Splitio.Domain;

namespace Splitio_Tests.Unit_Tests.Cache
{
    [TestClass]
    public class SplitCacheTests
    {
        [TestMethod]
        public void AddAndGetSplitTest()
        {
            //Arrange
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            var splitName = "test1";

            //Act
            splitCache.AddSplit(splitName, new ParsedSplit() { name = splitName });
            var result = splitCache.GetSplit(splitName);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddDuplicateSplitTest()
        {
            //Arrange
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            var splitName = "test1";

            //Act
            splitCache.AddSplit(splitName, new ParsedSplit() { name = splitName });
            splitCache.AddSplit(splitName, new ParsedSplit() { name = splitName });
            var result = splitCache.GetAllSplits();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetInexistentSplitTest()
        {
            //Arrange
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            var splitName = "test1";

            //Act
            var result = splitCache.GetSplit(splitName);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveSplitTest()
        {
            //Arrange
            var splitName = "test1";
            var splits = new ConcurrentDictionary<string, ParsedSplit>();
            splits.TryAdd(splitName, new ParsedSplit() { name = splitName });
            var splitCache = new InMemorySplitCache(splits);
            
            //Act
            splitCache.RemoveSplit(splitName);
            var result = splitCache.GetSplit(splitName);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SetAndGetChangeNumberTest()
        {
            //Arrange
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            var changeNumber = 1234;

            //Act
            splitCache.SetChangeNumber(changeNumber);
            var result = splitCache.GetChangeNumber();

            //Assert
            Assert.AreEqual(changeNumber, result);
        }

        [TestMethod]
        public void GetAllSplitsTest()
        {
            //Arrange
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            var splitName = "test1";
            var splitName2 = "test2";

            //Act
            splitCache.AddSplit(splitName, new ParsedSplit() { name = splitName });
            splitCache.AddSplit(splitName2, new ParsedSplit() { name = splitName2 });

            var result = splitCache.GetAllSplits();

            //Assert
            Assert.AreEqual(2, result.Count);
        }
    }
}
