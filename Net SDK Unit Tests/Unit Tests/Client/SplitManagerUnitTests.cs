﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Cache.Classes;
using Splitio.Domain;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Splitio.Services.Client.Classes;

namespace Splitio_Tests.Unit_Tests.Client
{
    [TestClass]
    public class SplitManagerUnitTests
    {
        [TestMethod]
        public void SplitsReturnSuccessfully()
        {
            //Arrange
            var conditionsWithLogic = new List<ConditionWithLogic>();
            var conditionWithLogic = new ConditionWithLogic()
            {
                partitions  = new List<PartitionDefinition>()
                {
                    new PartitionDefinition(){size = 100, treatment = "on"}
                }
            };
            conditionsWithLogic.Add(conditionWithLogic);
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            splitCache.AddSplit("test1", new ParsedSplit() { name = "test1", changeNumber = 10000, killed = false, trafficTypeName = "user", seed = -1, conditions = conditionsWithLogic});
            splitCache.AddSplit("test2", new ParsedSplit() { name = "test2", conditions = conditionsWithLogic });
            splitCache.AddSplit("test3", new ParsedSplit() { name = "test3", conditions = conditionsWithLogic });
            splitCache.AddSplit("test4", new ParsedSplit() { name = "test4", conditions = conditionsWithLogic });
            splitCache.AddSplit("test5", new ParsedSplit() { name = "test5", conditions = conditionsWithLogic });
            splitCache.AddSplit("test6", new ParsedSplit() { name = "test6", conditions = conditionsWithLogic });

            var manager = new SplitManager(splitCache);
            
            //Act
            var result = manager.Splits();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Count);
            var firstResult = result.Find(x=>x.name == "test1");
            Assert.AreEqual(firstResult.name, "test1");
            Assert.AreEqual(firstResult.changeNumber, 10000);
            Assert.AreEqual(firstResult.killed, false);
            Assert.AreEqual(firstResult.trafficType, "user");
            Assert.AreEqual(firstResult.treatments.Count, 1);
            var firstTreatment = firstResult.treatments[0];
            Assert.AreEqual(firstTreatment, "on");
        }

        [TestMethod]
        public void SplitsWhenCacheIsEmptyShouldReturnEmptyList()
        {
            //Arrange
            var splitCache = new InMemorySplitCache(new ConcurrentDictionary<string, ParsedSplit>());
            var manager = new SplitManager(splitCache);

            //Act
            var result = manager.Splits();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void SplitsWhenCacheIsNotInstancedShouldReturnNull()
        {
            //Arrange
            var manager = new SplitManager(null);

            //Act
            var result = manager.Splits();

            //Assert
            Assert.IsNull(result);
        }
    }
}
