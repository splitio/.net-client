using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace NetSDK.Services.Client
{
    public class SdkReadinessGates
    {
        private CountdownEvent splitsAreReady = new CountdownEvent(1);
        private Dictionary<String, CountdownEvent> segmentsAreReady = new Dictionary<String, CountdownEvent>();

        public bool IsSDKReady(int milliseconds)
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            int timeLeft = milliseconds;

            bool splits = AreSplitsReady(timeLeft);
            if (!splits)
            {
                return false;
            }

            timeLeft = timeLeft - (int)clock.ElapsedMilliseconds;

            return AreSegmentsReady(timeLeft);
        }


        public void SplitsAreReady()
        {
            splitsAreReady.Signal();
            if (splitsAreReady.IsSet)
            {
                //_log.info("splits are ready");
            }
        }
        
        public void SegmentIsReady(String segmentName)
        {
            CountdownEvent countDown;
            segmentsAreReady.TryGetValue(segmentName, out countDown);

            if (countDown == null)
            {
                return;
            }

            countDown.Signal();

            if (countDown.IsSet)
            {
               // _log.info(segmentName + " segment is ready");
            }
        }

        public bool AreSplitsReady(int milliseconds)
        {
            return splitsAreReady.Wait(milliseconds);
        }

        public bool RegisterSegments(HashSet<String> segmentNames)
        {
            if (segmentNames == null || AreSplitsReady(0))
            {
                return false;
            }

            foreach (var segmentName in segmentNames)
            {
                segmentsAreReady.Add(segmentName, new CountdownEvent(1));
            }

            //_log.info("Registered segments: " + segments);

            return true;
        }

        public bool AreSegmentsReady(int milliseconds)
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            int timeLeft = milliseconds;

            foreach (var entry in segmentsAreReady)
            {
                var segmentName = entry.Key;
                var countdown = entry.Value;

                if (!countdown.Wait(timeLeft))
                {
                    //_log.error(segmentName + " is not ready yet");
                    return false;
                }

                timeLeft = timeLeft - (int)clock.ElapsedMilliseconds;
            }

            return true;
        }
    }
}
