using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetSDK.CommonLibraries
{
    public static class TypeConverter
    {
        public static DateTime ToDateTime(this long timestamp)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var timestampTruncatedToMinutes = timestamp - timestamp % 60000;
            date = date.AddMilliseconds(timestampTruncatedToMinutes);
            return date;
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

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
            {
                return dateTime;
            }
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }
}
