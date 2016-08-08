using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.Services.Parsing
{
    public class BetweenMatcher: Matcher
    {
        private DataTypeEnum? dataType;
        private long start;
        private long end;

        public BetweenMatcher(DataTypeEnum? dataType, long start, long end)
        {
            // TODO: Complete member initialization
            this.dataType = dataType;
            this.start = start;
            this.end = end;
        }

        public override bool Match(string key)
        {
            throw new NotImplementedException();
        }
    }
}
