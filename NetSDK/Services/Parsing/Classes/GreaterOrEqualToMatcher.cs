using NetSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetSDK.CommonLibraries;

namespace NetSDK.Services.Parsing
{
    public class GreaterOrEqualToMatcher: CompareMatcher, IMatcher
    {
        public GreaterOrEqualToMatcher(DataTypeEnum? dataType, long value)
        {
            this.dataType = dataType;
            this.value = value;
        }

        protected override bool MatchNumber(string key)
        {
            long transformedKey;
            if (long.TryParse(key, out transformedKey))
            {
                return transformedKey >= value;
            }
            return false;
        }

        protected override bool MatchDate(string key)
        {
            var date = value.ToDateTime();
            var transformedKey = key.ToDateTime();

            return transformedKey >= date;
        }
    }
}
