﻿using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splitio.CommonLibraries;

namespace Splitio.Services.Parsing
{
    public class BetweenMatcher : CompareMatcher, IMatcher
    {

        public BetweenMatcher(DataTypeEnum? dataType, long start, long end)
        {
            this.dataType = dataType;
            this.start = start;
            this.end = end;
        }

        public override bool Match(long key)
        {
            return (start <= key) && (key <= end);         
        }

        public override bool Match(DateTime key)
        {
            var startDate = start.ToDateTime();
            var endDate = end.ToDateTime();

            return (startDate <= key) && (key <= endDate);
        }
    }
}
