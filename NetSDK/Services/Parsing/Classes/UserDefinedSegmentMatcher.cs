using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.Services.Parsing
{
    public class UserDefinedSegmentMatcher: IMatcher
    {
        private Segment segment;

        public UserDefinedSegmentMatcher(Segment segment)
        {
            if (segment == null)
            {
                throw new Exception("Segment cannot be null");
            }

            this.segment = segment;
        }
        public bool Match(string key)
        {
            return segment.Contains(key);
        }
    }
}
