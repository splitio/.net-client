﻿using BenchmarkDotNet.Running;
using Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microbenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Metric.Config
            .WithHttpEndpoint("http://localhost:2345/")
            .WithAllCounters();

            var metricsProcessor = new MetricsProcessor();
            metricsProcessor.ProcessRequest(Int32.Parse(args[0]), Int32.Parse(args[1]));
            /*var switcher = new BenchmarkSwitcher(new[] {
                typeof(BenchmarkGetTreatmentLocalhost),
                typeof(BenchmarkGetTreatmentStagingTestingAccount),
                typeof(BenchmarkGetTreatmentStagingGoogleJSAccount)
            });
            switcher.Run(args);
            Console.Read();*/
        }
    }
}