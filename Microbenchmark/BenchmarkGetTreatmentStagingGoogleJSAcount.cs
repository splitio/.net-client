using System;
using System.Threading.Tasks;
using Splitio.Services.Client.Classes;
﻿using BenchmarkDotNet.Attributes;
using Splitio.Services.Client.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace Microbenchmark
{
    public class BenchmarkGetTreatmentStagingGoogleJSAccount
    {
        ISplitClient client;

        [Setup]
        public void Setup()
        {
            client = ClientSingleInstance.GetInstance("x");
        }

     
        [Benchmark]
        public string GetTreatmentBenchmarkStagingFirstCondition()
        {
            var atributes = new Dictionary<string, object>();
            atributes.Add("atrib", "1474990945");
            var result = client.GetTreatment(Guid.NewGuid().ToString(), "benchmark_jw_1", atributes);
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkStagingSecondCondition()
        {
            var result = client.GetTreatment("abcdefghijklmnopqrxyz123456789ABCDEF", "benchmark_jw_1");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkStagingThirdCondition()
        {
            var atributes = new Dictionary<string, object>();
            atributes.Add("atrib2", "20");
            var result = client.GetTreatment(Guid.NewGuid().ToString(), "benchmark_jw_1", atributes);
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkStaging2FirstCondition()
        {
            var atributes = new Dictionary<string, object>();
            atributes.Add("atrib3", "15");
            var result = client.GetTreatment(Guid.NewGuid().ToString(), "benchmark_jw_2", atributes);
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkStaging2SecondCondition()
        {
            var atributes = new Dictionary<string, object>();
            atributes.Add("atrib4", "abc");
            var result = client.GetTreatment(Guid.NewGuid().ToString(), "benchmark_jw_2", atributes);
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkStaging2ThirdCondition()
        {
            var result = client.GetTreatment(Guid.NewGuid().ToString(), "benchmark_jw_2");
            return result;
        }
    }
}
