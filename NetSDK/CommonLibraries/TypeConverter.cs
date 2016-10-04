using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitio.CommonLibraries
{
    public static class TypeConverter
    {
        private static readonly DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        public static DateTime ToDateTime(this long timestamp)
        {
            var timestampTruncatedToMinutes = timestamp - timestamp % 60000;
            return date.AddMilliseconds(timestampTruncatedToMinutes);
        }

        public static DateTime? ToDateTime(this string timestampString)
        {
            long timestamp;
            if (long.TryParse(timestampString, out timestamp))
            {
                return timestamp.ToDateTime();
            }
            return null;
        }
    }
}
