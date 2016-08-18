using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using log4net;

namespace NetSDK.Services.Client
{
    public class SdkReadinessGates
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SdkReadinessGates));
        private CountdownEvent splitsAreReady = new CountdownEvent(1);
        private Dictionary<String, CountdownEvent> segmentsAreReady = new Dictionary<String, CountdownEvent>();

        public bool IsSDKReady(int milliseconds)
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();

            if (!AreSplitsReady(milliseconds))
            {
                return false;
            }

            int timeLeft = milliseconds - (int)clock.ElapsedMilliseconds;

            return AreSegmentsReady(timeLeft);
        }


        public void SplitsAreReady()
        {
            if (!splitsAreReady.IsSet)
            {
                splitsAreReady.Signal();
                if (splitsAreReady.IsSet)
                {
                    Log.Info("Splits are ready");
                }
            }
        }
        
        public void SegmentIsReady(String segmentName)
        {
            CountdownEvent countDown;
            segmentsAreReady.TryGetValue(segmentName, out countDown);

            if ((countDown == null) || (countDown.IsSet))
            {
                return;
            }

            countDown.Signal();

            if (countDown.IsSet)
            {
                Log.Info(segmentName + " segment is ready");
            }
        }

        public bool AreSplitsReady(int milliseconds)
        {
            return splitsAreReady.Wait(milliseconds);
        }

        public bool RegisterSegments(List<String> segmentNames)
        {
            if (segmentNames == null || AreSplitsReady(0))
            {
                return false;
            }

            foreach (var segmentName in segmentNames)
            {
                try
                {
                    segmentsAreReady.Add(segmentName, new CountdownEvent(1));
                    Log.Info("Registered segment: " + segmentName);
                }
                catch(ArgumentException e)
                {
                    Log.Info("Already registered segment: " + segmentName, e);
                }
            }

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

                if (timeLeft >= 0)
                {
                    if (!countdown.Wait(timeLeft))
                    {
                        Log.Error(segmentName + " is not ready yet");
                        return false;
                    }
                }
                else
                {
                    if (!countdown.Wait(0))
                    {
                        Log.Error(segmentName + " is not ready yet");
                        return false;
                    }
                }
                timeLeft = timeLeft - (int)clock.ElapsedMilliseconds;
            }

            return true;
        }
    }
}
