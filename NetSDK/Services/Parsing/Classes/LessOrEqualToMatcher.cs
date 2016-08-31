using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splitio.CommonLibraries;


namespace Splitio.Services.Parsing
{
    public class LessOrEqualToMatcher : CompareMatcher, IMatcher
    {
        public LessOrEqualToMatcher(DataTypeEnum? dataType, long value)
        {
            this.dataType = dataType;
            this.value = value;
        }

        protected override bool MatchNumber(string key)
        {
            long transformedKey;
            if (long.TryParse(key, out transformedKey))
            {
                return transformedKey <= value;
            }
            return false;
        }

        protected override bool MatchDate(string key)
        {
            var date = value.ToDateTime();
            var transformedKey = key.ToDateTime();

            return transformedKey <= date;
        }
    }
}
