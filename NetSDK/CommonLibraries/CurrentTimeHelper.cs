using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.CommonLibraries
{
    public static class CurrentTimeHelper
    {
        public static long CurrentTimeMillis()
        {
            var Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }
}
