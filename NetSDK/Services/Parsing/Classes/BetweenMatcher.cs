using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetSDK.CommonLibraries;

namespace NetSDK.Services.Parsing
{
    public class BetweenMatcher : CompareMatcher, IMatcher
    {

        public BetweenMatcher(DataTypeEnum? dataType, long start, long end)
        {
            this.dataType = dataType;
            this.start = start;
            this.end = end;
        }

        protected override bool MatchNumber(string key)
        {
            long transformedKey;
            if (long.TryParse(key, out transformedKey))
            {
                return (start <= transformedKey) && (transformedKey <= end);
            }
            return false;            
        }

        protected override bool MatchDate(string key)
        {
            var startDate = start.ToDateTime();
            var endDate = end.ToDateTime();
            var transformedKey = key.ToDateTime();

            return (startDate <= transformedKey) && (transformedKey <= endDate);
        }       
    }
}
