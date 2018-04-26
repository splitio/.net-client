﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Common.Logging;
using Splitio.Services.Cache.Interfaces;

namespace Splitio.Services.Client.Classes
{
    public class InMemoryReadinessGatesCache : IReadinessGatesCache
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(InMemoryReadinessGatesCache));
        private readonly CountdownEvent splitsAreReady = new CountdownEvent(1);
        private readonly Dictionary<String, CountdownEvent> segmentsAreReady = new Dictionary<String, CountdownEvent>();

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
                    Log.Debug("Splits are ready");
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
                Log.Debug(segmentName + " segment is ready");
            }
        }

        public bool AreSplitsReady(int milliseconds)
        {
            return splitsAreReady.Wait(milliseconds);
        }

        public bool RegisterSegment(string segmentName)
        {
            if (String.IsNullOrEmpty(segmentName) || AreSplitsReady(0))
            {
                return false;
            }

            try
            {
                segmentsAreReady.Add(segmentName, new CountdownEvent(1));
                Log.Debug("Registered segment: " + segmentName);
            }
            catch (ArgumentException e)
            {
                Log.Warn("Already registered segment: " + segmentName, e);
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
                        return false;
                    }
                }
                else
                {
                    if (!countdown.Wait(0))
                    {
                        return false;
                    }
                }
                timeLeft = timeLeft - (int)clock.ElapsedMilliseconds;
            }

            Log.Debug("Segments are ready");

            return true;
        }
    }
}
