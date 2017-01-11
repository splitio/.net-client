using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Metrics.Classes;
using System.Collections.Concurrent;
using System.Threading;
using Splitio.Services.Metrics.Interfaces;
using System.Linq;

namespace Splitio_Tests.Unit_Tests.Metrics
{
    [TestClass]
    public class AsyncMetricsLogUnitTests
    {
        [TestMethod]
        public void CountSucessfully()
        {
            //Arrange
            var counters = new ConcurrentDictionary<string, Counter>();
            var metricsLog = new AsyncMetricsLog(null, counters, null, null, 10, 3000);

            //Act
            metricsLog.Count("counter_test", 150);

            //Assert
            Thread.Sleep(2000);
            Counter counter;
            counters.TryGetValue("counter_test", out counter);
            Assert.IsNotNull(counter);
            Assert.AreEqual(1, counter.GetCount());
            Assert.AreEqual(150, counter.GetDelta());
        }


        [TestMethod]
        public void TimeSucessfully()
        {
            //Arrange
            var timers = new ConcurrentDictionary<string, ILatencyTracker>();
            var metricsLog = new AsyncMetricsLog(null, null, timers, null, 10, 3000);

            //Act
            metricsLog.Time("time_test", 1);

            //Assert
            Thread.Sleep(2000);
            ILatencyTracker timer;
            timers.TryGetValue("time_test", out timer);
            Assert.IsNotNull(timer);
            Assert.AreEqual(1, timer.GetLatency(0));
            long[] latencies = timer.GetLatencies();
            Assert.AreEqual(1, latencies.Sum());
        }


        [TestMethod]
        public void GaugeSucessfully()
        {
            //Arrange
            var gauges = new ConcurrentDictionary<string, long>();
            var metricsLog = new AsyncMetricsLog(null, null, null, gauges, 10, 3000);

            //Act
            metricsLog.Gauge("gauge_test", 1234);

            //Assert
            Thread.Sleep(2000);
            long gauge;
            gauges.TryGetValue("gauge_test", out gauge);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(1234, gauge);
        }
    }
}
