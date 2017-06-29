using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using Splitio.Services.Client.Interfaces;
using System;
using System.Collections.Generic;

namespace Splitio.Services.Parsing
{
    public class UserDefinedSegmentMatcher: IMatcher
    {
        private string segmentName;
        private ISegmentCache segmentsCache;

        public UserDefinedSegmentMatcher(string segmentName, ISegmentCache segmentsCache)
        {
            this.segmentName = segmentName;
            this.segmentsCache = segmentsCache;
        }

        public bool Match(string key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return segmentsCache.IsInSegment(segmentName, key);
        }

        public bool Match(Key key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return segmentsCache.IsInSegment(segmentName, key.matchingKey);
        }

        public bool Match(DateTime key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(long key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

        public bool Match(List<string> key, Dictionary<string, object> attributes = null, ISplitClient splitClient = null)
        {
            return false;
        }

    }
}
