using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Splitio.Services.SplitFetcher.Interfaces;
using Splitio.Services.SplitFetcher.Classes;

namespace Splitio_Tests.Unit_Tests
{
    [TestClass]
    public class ApiSplitChangeFetcherTests
    {
        [TestMethod]
        [Description("Test a Json that changes its structure and is deserialized without exception. Contains: a field renamed, a field removed and a field added.")]
        public void ExecuteJsonDeserializeSuccessfulWithChangeInJsonFormat()
        {
            //Arrange
            Mock<ISplitSdkApiClient> apiMock = new Mock<ISplitSdkApiClient>();
            apiMock
                .Setup(x => x.FetchSplitChanges(-1))
                .Returns("{\"splits\": [ { \"trafficType\": \"user\", \"name\": \"Reset_Seed_UI\", \"seed\": 1552577712, \"status\": \"ACTIVE\", \"defaultTreatment\": \"off\", \"changeNumber\": 1469827821322, \"conditions\": [ { \"matcherGroup\": { \"combiner\": \"AND\", \"matchers\": [ { \"keySelector\": { \"trafficType\": \"user\", \"attribute\": null }, \"matcherType\": \"ALL_KEYS\", \"negate\": false, \"userDefinedSegmentMatcherData\": null, \"whitelistMatcherData\": null, \"unaryNumericMatcherData\": null, \"betweenMatcherData\": null } ] }, \"partitions\": [ { \"treatment\": \"on\", \"size\": 100 }, { \"treatment\": \"off\", \"size\": 0, \"addedField\": \"test\"  } ] } ] } ], \"since\": 1469817846929, \"till\": 1469827821322 }\r\n");

            ApiSplitChangeFetcher apiSplitChangeFetcher = new ApiSplitChangeFetcher(apiMock.Object);

            //Act
            var result = apiSplitChangeFetcher.Fetch(-1);

            //Assert
            Assert.IsTrue(result != null);

        }
    }
}
