using Splitio.Domain;
using Splitio.Services.Cache.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public bool Match(string key)
        {
            return segmentsCache.IsInSegment(segmentName, key);
        }
    }
}
