using System;
using System.Threading.Tasks;
using Splitio.Services.Client.Classes;
﻿using BenchmarkDotNet.Attributes;
using Splitio.Services.Client.Interfaces;
using System.Collections.Generic;

namespace Microbenchmark
{
    public class Benchmark
    {
        ISplitClient localhostClient;
        ISplitClient jsonClient;
        private Dictionary<string, object> attributes;
        public Benchmark()
        {
            String apikey = "localhost";

            var factory = new SplitFactory();
            var configurations = new ConfigurationOptions();
            configurations.LocalhostFilePath = @"C:\Making Sense\net-client\Microbenchmark\bin\Release\Resources\test.splits";
            localhostClient = factory.BuildSplitClient(apikey, configurations);

            attributes = new Dictionary<string, object>();
            attributes.Add("attr", "18");

            jsonClient = new JSONFileClient(@"C:\Making Sense\net-client\Microbenchmark\bin\Release\Resources\splits_staging.json", @"C:\Making Sense\net-client\Microbenchmark\bin\Release\Resources\segment_payed.json");
        }

        [Benchmark]
        public string GetTreatmentBenchmarkLocalhost_Off()
        {
            var result = localhostClient.GetTreatment("test", "double_writes_to_cassandra");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkLocalhost_On()
        {
            var result = localhostClient.GetTreatment("test", "other_test_feature");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClient_AllKeys_ShortKey()
        {
            var result = jsonClient.GetTreatment("test", "Marcio_Test_1");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClient_AllKeys_LongKey()
        {
            var result = jsonClient.GetTreatment("abcdfghijklm9878979fdsfdsfdsAAssdsXXdsds", "Marcio_Test_1");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClient_MultipleMatchers_ShortKey()
        {
            var result = jsonClient.GetTreatment("test", "Manu_Test_1");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClient_MultipleMatchers_LongKey()
        {
            var result = jsonClient.GetTreatment("abcdfghijklm9878979fdsfdsfdsAAssdsXXdsds", "Manu_Test_1");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClient_MultipleMatchers_Attribute()
        {
            var result = jsonClient.GetTreatment("test", "Manu_Test_1", attributes);
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClient_MultipleMatchers_InSegment()
        {
            var result = jsonClient.GetTreatment("abc123", "Manu_Test_1");
            return result;
        }
    }
}
