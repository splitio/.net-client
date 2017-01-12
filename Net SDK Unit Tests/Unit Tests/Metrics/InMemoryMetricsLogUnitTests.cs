using Microsoft.VisualStudio.TestTools.UnitTesting;
using Splitio.Services.Metrics.Classes;
using System.Collections.Concurrent;
using Splitio.Services.Metrics.Interfaces;
using System.Linq;
using Moq;

namespace Splitio_Tests.Unit_Tests.Metrics
{
    [TestClass]
    public class InMemoryMetricsLogUnitTests
    {
        [TestMethod]
        public void CountShouldAddNewMetricWhenNotExisting()
        {
            //Arrange
            var counters = new ConcurrentDictionary<string, Counter>();
            var metricsLog = new InMemoryMetricsLog(null, counters);
            
            //Act
            metricsLog.Count("counter_test", 150);

            //Assert
            Counter counter;
            counters.TryGetValue("counter_test", out counter);
            Assert.IsNotNull(counter);
            Assert.AreEqual(1, counter.GetCount());
            Assert.AreEqual(150, counter.GetDelta());
        }

        [TestMethod]
        public void CountShouldUpdateMetricWhenExisting()
        {
            //Arrange
            var counters = new ConcurrentDictionary<string, Counter>();
            var metricsLog = new InMemoryMetricsLog(null, counters);

            //Act
            metricsLog.Count("counter_test", 150);
            metricsLog.Count("counter_test", 10);

            //Assert
            Counter counter;
            counters.TryGetValue("counter_test", out counter);
            Assert.AreEqual(2, counter.GetCount());
            Assert.AreEqual(160, counter.GetDelta());

        }


        [TestMethod]
        public void CountShouldNotUpdateMetricIfDeltaIsLessOrEqualThanZero()
        {
            //Arrange
            var counters = new ConcurrentDictionary<string, Counter>();
            var metricsLog = new InMemoryMetricsLog(null, counters);

            //Act
            metricsLog.Count("counter_test", 0);
            metricsLog.Count("counter_test", -1);

            //Assert
            Counter counter;
            counters.TryGetValue("counter_test", out counter);
            Assert.IsNull(counter);
        }

        [TestMethod]
        public void CountShouldNotAddMetricIfNoNameSpecified()
        {
            //Arrange
            var counters = new ConcurrentDictionary<string, Counter>();
            var metricsLog = new InMemoryMetricsLog(null, counters);

            //Act
            metricsLog.Count("", 1);

            //Assert
            Counter counter;
            counters.TryGetValue("", out counter);
            Assert.IsNull(counter);
        }


        [TestMethod]
        public void TimeShouldAddNewMetricWhenNotExisting()
        {
            //Arrange
            var timers = new ConcurrentDictionary<string, ILatencyTracker>();
            var metricsLog = new InMemoryMetricsLog(null,null, timers);
            
            //Act
            metricsLog.Time("time_test", 1);

            //Assert
            ILatencyTracker timer;
            timers.TryGetValue("time_test",out timer);
            Assert.IsNotNull(timer);
            Assert.AreEqual(1, timer.GetLatency(0));
            long[] latencies = timer.GetLatencies();
            Assert.AreEqual(1, latencies.Sum());
        }

        [TestMethod]
        public void TimeShouldUpdateMetricWhenExisting()
        {
            //Arrange
            var timers = new ConcurrentDictionary<string, ILatencyTracker>();
            var metricsLog = new InMemoryMetricsLog(null, null, timers);

            //Act
            metricsLog.Time("time_test", 1);
            metricsLog.Time("time_test", 9);
            metricsLog.Time("time_test", 8);
          
            //Assert
            ILatencyTracker timer;
            timers.TryGetValue("time_test", out timer);
            Assert.AreEqual(1, timer.GetLatency(0));
            Assert.AreEqual(2, timer.GetLatency(6));
            long[] latencies = timer.GetLatencies();
            Assert.AreEqual(3, latencies.Sum());
        }


        [TestMethod]
        public void TimeShouldNotUpdateMetricIfDeltaIsLessThanZero()
        {
            //Arrange
            var timers = new ConcurrentDictionary<string, ILatencyTracker>();
            var metricsLog = new InMemoryMetricsLog(null, null, timers);

            //Act
            metricsLog.Time("time_test", -1);

            //Assert
            ILatencyTracker timer;
            timers.TryGetValue("time_test", out timer);
            Assert.IsNull(timer);
        }

        public void TimeShouldNotAddMetricIfNoNameSpecified()
        {
            //Arrange
            var timers = new ConcurrentDictionary<string, ILatencyTracker>();
            var metricsLog = new InMemoryMetricsLog(null, null, timers);

            //Act
            metricsLog.Time("", 1000);

            //Assert
            ILatencyTracker timer;
            timers.TryGetValue("", out timer);
            Assert.IsNull(timer);
        }

        [TestMethod]
        public void GaugeShouldAddNewMetricWhenNotExisting()
        {
            //Arrange
            var gauges = new ConcurrentDictionary<string, long>();
            var metricsLog = new InMemoryMetricsLog(null, null, null, gauges);

            //Act
            metricsLog.Gauge("gauge_test", 1234);

            //Assert
            long gauge;
            gauges.TryGetValue("gauge_test", out gauge);
            Assert.IsNotNull(gauge);
            Assert.AreEqual(1234, gauge);
        }

        [TestMethod]
        public void GaugeShouldUpdateMetricWhenExisting()
        {
            //Arrange
            var gauges = new ConcurrentDictionary<string, long>();
            var metricsLog = new InMemoryMetricsLog(null, null, null, gauges);

            //Act
            metricsLog.Gauge("gauge_test", 1234);
            metricsLog.Gauge("gauge_test", 4567);

            //Assert
            long gauge;
            gauges.TryGetValue("gauge_test", out gauge);
            Assert.AreEqual(4567, gauge);
        }

        [TestMethod]
        public void GaugeShouldNotUpdateMetricIfDeltaIsLessThanZero()
        {
            //Arrange
            var gauges = new ConcurrentDictionary<string, long>();
            var metricsLog = new InMemoryMetricsLog(null, null, null, gauges);

            //Act
            metricsLog.Gauge("gauge_test", -1);

            //Assert
            long gauge;
            gauges.TryGetValue("gauge_test", out gauge);
            Assert.AreEqual(0, gauge);
        }

        [TestMethod]
        public void GaugeShouldNotAddMetricIfNoNameSpecified()
        {
            //Arrange
            var gauges = new ConcurrentDictionary<string, long>();
            var metricsLog = new InMemoryMetricsLog(null, null, null, gauges);

            //Act
            metricsLog.Gauge("", 1000);

            //Assert
            long gauge;
            gauges.TryGetValue("", out gauge);
            Assert.AreEqual(0, gauge);
        }


        [TestMethod]
        public void SendCountMetricsSuccessfully()
        {
            //Arrange
            var counters = new ConcurrentDictionary<string, Counter>();
            var metricsApiClientMock = new Mock<IMetricsSdkApiClient>();
            var metricsLog = new InMemoryMetricsLog(metricsApiClientMock.Object, counters, null, null, 1);
            
            //Act
            metricsLog.Count("counter_test", 150);

            //Assert
            metricsApiClientMock.Verify(x => x.SendCountMetrics(It.IsAny<string>()));
        }

        [TestMethod]
        public void SendTimeMetricsSuccessfully()
        {
            //Arrange
            var timers = new ConcurrentDictionary<string, ILatencyTracker>();
            var metricsApiClientMock = new Mock<IMetricsSdkApiClient>();
            var metricsLog = new InMemoryMetricsLog(metricsApiClientMock.Object, null, timers, null, 1, -1);

            //Act
            metricsLog.Time("time_test", 1);

            //Assert
            metricsApiClientMock.Verify(x => x.SendTimeMetrics(It.IsAny<string>()));
        }

        [TestMethod]
        public void SendGaugeMetricsSuccessfully()
        {
            //Arrange
            var gauges = new ConcurrentDictionary<string, long>();
            var metricsApiClientMock = new Mock<IMetricsSdkApiClient>();
            var metricsLog = new InMemoryMetricsLog(metricsApiClientMock.Object, null, null, gauges, 1);

            //Act
            metricsLog.Gauge("gauge_test", 1234);

            //Assert
            metricsApiClientMock.Verify(x => x.SendGaugeMetrics(It.IsAny<string>()));
        }

    }
}
