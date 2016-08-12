using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public class UserDefinedSegmentMatcher: IMatcher
    {
        private Segment segment;

        public UserDefinedSegmentMatcher(Segment segment)
        {
            //TODO: no permitir null
            this.segment = segment;
        }
        public bool Match(string key)
        {
            if (segment == null)
            { 
                return false;
            }

            return segment.Contains(key);
        }
    }
}
