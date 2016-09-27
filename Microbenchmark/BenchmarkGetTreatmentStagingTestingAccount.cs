using System;
using System.Threading.Tasks;
using Splitio.Services.Client.Classes;
﻿using BenchmarkDotNet.Attributes;
using Splitio.Services.Client.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace Microbenchmark
{
    public class BenchmarkGetTreatmentStagingTestingAccount
    {
        ISplitClient client;

        [Setup]
        public void Setup()
        {
            client = ClientSingleInstance.GetInstance("x");
        }

     
        [Benchmark]
        public string GetTreatmentBenchmarkStaging()
        {
            var result = client.GetTreatment("user_for_testing_do_no_erase", "DJANGO_addCondition");
            return result;
        }

        [Benchmark]
        public string GetTreatmentBenchmarkStaging2()
        {
            var result = client.GetTreatment(Guid.NewGuid().ToString(), "DJANGO_addCondition");
            return result;
        }
    }
}
