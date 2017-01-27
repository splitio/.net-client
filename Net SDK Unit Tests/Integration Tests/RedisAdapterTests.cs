using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Cache.Classes;
using Splitio.Services.Cache.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio_Tests.Integration_Tests
{
    [TestClass]
    [Ignore]
    public class RedisAdapterTests
    {
        IRedisAdapter adapter;

        [TestInitialize]
        public void Initialization()
        {
            adapter = new RedisAdapter("localhost", "6379");
        }

        [TestMethod]
        public void ExecuteSetAndGetSuccessful()
        {
            //Arrange
            var isSet = adapter.Set("test_key", "test_value");

            //Act
            var result = adapter.Get("test_key");

            //Assert
            Assert.IsTrue(isSet);
            Assert.AreEqual("test_value", result);
        }

        [TestMethod]
        public void ExecuteMultipleSetAndMultipleGetSuccessful()
        {
            //Arrange
            var isSet1 = adapter.Set("test_key", "test_value");
            var isSet2 = adapter.Set("test_key2", "test_value2");
            var isSet3 = adapter.Set("test_key3", "test_value3");

            //Act
            var result = adapter.Get(new RedisKey[]{"test_key", "test_key2", "test_key3"});

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(isSet1 & isSet2 & isSet3);
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains("test_value"));
            Assert.IsTrue(result.Contains("test_value2"));
            Assert.IsTrue(result.Contains("test_value3"));
        }

        [TestMethod]
        public void ExecuteMultipleSetAndGetAllKeysWithFilterSuccessful()
        {
            //Arrange
            var isSet1 = adapter.Set("test.test_key", "test_value");
            var isSet2 = adapter.Set("test.test_key2", "test_value2");
            var isSet3 = adapter.Set("test.test_key3", "test_value3");

            //Act
            var result = adapter.Keys("test.*");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(isSet1 & isSet2 & isSet3);
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains("test.test_key"));
            Assert.IsTrue(result.Contains("test.test_key2"));
            Assert.IsTrue(result.Contains("test.test_key3"));
        }

        [TestMethod]
        public void ExecuteSetAndDelSuccessful()
        {
            //Arrange
            var isSet1 = adapter.Set("testdel.test_key", "test_value");

            //Act
            var isDel = adapter.Del("testdel.test_key");
            var result = adapter.Get("testdel.test_key");

            //Assert
            Assert.IsTrue(isSet1);
            Assert.IsTrue(isDel);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExecuteSetAndFlushSuccessful()
        {
            //Arrange
            var isSet1 = adapter.Set("testflush.test_key", "test_value");

            //Act
            adapter.Flush();
            var result = adapter.Keys("test.*");

            //Assert
            Assert.IsTrue(isSet1);
            Assert.AreEqual(0, result.Count());
        }

    }
}
