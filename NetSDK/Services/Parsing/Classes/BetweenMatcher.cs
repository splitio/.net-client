using Splitio.Domain;
using System;
using Splitio.CommonLibraries;
using Splitio.Services.Client.Interfaces;

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

        public override bool Match(long key, ISplitClient splitClient = null)
        {
            return (start <= key) && (key <= end);         
        }

        public override bool Match(DateTime key, ISplitClient splitClient = null)
        {
            var startDate = start.ToDateTime();
            var endDate = end.ToDateTime();
            key = key.Truncate(TimeSpan.FromMinutes(1)); // Truncate to whole minute
            return (startDate <= key) && (key <= endDate);
        }
    }
}
