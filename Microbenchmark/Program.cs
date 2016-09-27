using BenchmarkDotNet.Running;
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
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(BenchmarkGetTreatmentLocalhost),
                typeof(BenchmarkGetTreatmentStagingTestingAccount),
                typeof(BenchmarkGetTreatmentStagingGoogleJSAccount)
            });
            switcher.Run(args);
            Console.Read();
        }
    }
}
