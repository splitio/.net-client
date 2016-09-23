using System;
using System.Threading.Tasks;
using Splitio.Services.Client.Classes;
﻿using BenchmarkDotNet.Attributes;
using Splitio.Services.Client.Interfaces;

namespace Microbenchmark
{
    public class Benchmark
    {
        ISplitClient localhostClient;
        ISplitClient jsonClient;

        public Benchmark()
        {
            String apikey = "localhost";

            var factory = new SplitFactory();
            var configurations = new ConfigurationOptions();
            configurations.LocalhostFilePath = @"C:\Making Sense\net-client\Microbenchmark\bin\Release\Resources\test.splits";
            localhostClient = factory.BuildSplitClient(apikey, configurations);

            jsonClient = new JSONFileClient(@"C:\Making Sense\net-client\Microbenchmark\bin\Release\Resources\splits_staging.json", @"C:\Making Sense\net-client\Microbenchmark\bin\Release\Resources\segment_payed.json");
        }

        [Benchmark]
        public string GetTreatmentBenchmarkLocalhostA()
        {
            var result = localhostClient.GetTreatment("test", "double_writes_to_cassandra");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkLocalhostB()
        {
            var result = localhostClient.GetTreatment("test", "other_test_feature");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClientA()
        {
            var result = jsonClient.GetTreatment("test", "Marcio_Test_1");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkJsonClientB()
        {
            var result = jsonClient.GetTreatment("abcdfghijklm9878979", "Marcio_Test_1");
            return result;
        }
    }
}
