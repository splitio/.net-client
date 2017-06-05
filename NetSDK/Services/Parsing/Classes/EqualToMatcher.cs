using Splitio.Domain;
using System;
using Splitio.CommonLibraries;
using Splitio.Services.Client.Interfaces;

namespace Splitio.Services.Parsing
{
    public class EqualToMatcher : CompareMatcher, IMatcher
    {
        public EqualToMatcher(DataTypeEnum? dataType, long value)
        {
            this.dataType = dataType;
            this.value = value;
        }

        public override bool Match(long key, ISplitClient splitClient = null)
        {
            return value == key;
        }

        public override bool Match(DateTime key, ISplitClient splitClient = null)
        {
            var date = value.ToDateTime();

            return date.Date == key.Date; // Compare just date part
        }
    }
}
