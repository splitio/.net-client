using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetSDK.Services.Client.Classes;

namespace Net_SDK_Unit_Tests.Integration_Tests
{
    [TestClass]
    public class LocalhostClientTests
    {
        [DeploymentItem(@"Resources\test.splits")]
        [TestMethod]
        public void GetTreatmentSuccessfully()
        {
            //Arrange
            var client = new LocalhostClient("test.splits");

            //Act
            var result1 = client.GetTreatment(null, "double_writes_to_cassandra", null);
            var result2 = client.GetTreatment("id", "double_writes_to_cassandra", null);
            var result3 = client.GetTreatment(null, "other_test_feature", null);
            var result4 = client.GetTreatment("id", "other_test_feature", null);

            //Asert
            Assert.IsTrue(result1 == "off"); //default treatment
            Assert.IsTrue(result2 == "off"); //matched 
            Assert.IsTrue(result3 == "on"); //default treatment
            Assert.IsTrue(result4 == "on"); //matched 
        }
    }
}
