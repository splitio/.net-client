﻿using Splitio.Domain;
using System;
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

        public override bool Match(long key)
        {
            return key <= value;
        }

        public override bool Match(DateTime key)
        {
            var date = value.ToDateTime();
            key = key.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute
            return key <= date;
        }
    }
}
