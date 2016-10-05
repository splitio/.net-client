using Splitio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Splitio.CommonLibraries;

namespace Splitio.Services.Parsing
{
    public class GreaterOrEqualToMatcher: CompareMatcher, IMatcher
    {
        public GreaterOrEqualToMatcher(DataTypeEnum? dataType, long value)
        {
            this.dataType = dataType;
            this.value = value;
        }

        public override bool Match(long key)
        {
            return key >= value;
        }

        public override bool Match(DateTime key)
        {
            var date = value.ToDateTime();

            return key >= date;
        }
    }
}
