using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using Splitio.Services.EngineEvaluator;
using System;
using System.Collections.Generic;

namespace Splitio_Tests.Unit_Tests
{
    [TestClass]
    public class SplitterTests
    {
        [DeploymentItem(@"Resources\legacy-sample-data.csv")]
        [TestMethod]
        public void VerifyHashAndBucketSampleData()
        {
            VerifyTestFile("legacy-sample-data.csv", new string[] { "\r\n" });
        }

        [DeploymentItem(@"Resources\legacy-sample-data-non-alpha-numeric.csv")]
        [TestMethod]
        public void VerifyHashAndBucketSampleDataNonAlphanumeric()
        {
            VerifyTestFile("legacy-sample-data-non-alpha-numeric.csv", new string[] { "\n" });
        }


        [DeploymentItem(@"Resources\murmur3-sample-data.csv")]
        [TestMethod]
        public void VerifyMurmur3HashAndBucketSampleData()
        {
            VerifyTestFile("murmur3-sample-data.csv", new string[] { "\r\n" }, false);
        }


        [DeploymentItem(@"Resources\murmur3-sample-data-non-alpha-numeric.csv")]
        [TestMethod]
        public void VerifyMurmur3HashAndBucketSampleDataNonAlphanumeric()
        {
            VerifyTestFile("murmur3-sample-data-non-alpha-numeric.csv", new string[] { "\n" }, false);
        }


        private void VerifyTestFile(string file, string[] sepparator, bool legacy = true)
        {
            //Arrange
            var fileContent = File.ReadAllText(file);
            var contents = fileContent.Split(sepparator, StringSplitOptions.None);
            var csv = from line in contents
                      select line.Split(',').ToArray();

            var splitter = new Splitter();
            bool first = true;

            var results = new List<string>();
            //Act
            foreach (string[] item in csv)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (item.Length == 4)
                    {
                        var hash = legacy ? splitter.LegacyHash(item[1], int.Parse(item[0])) : splitter.Hash(item[1], int.Parse(item[0]));
                        var bucket = legacy ? splitter.LegacyBucket(item[1], int.Parse(item[0])) : splitter.Bucket(item[1], int.Parse(item[0]));

                        //Assert
                        Assert.AreEqual(hash, int.Parse(item[2]));
                        Assert.AreEqual(bucket, int.Parse(item[3]));
                    }
                }
            }
        }
    }
}
