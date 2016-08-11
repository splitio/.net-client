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
            date = date.AddMilliseconds(timestamp);
            date = date.Truncate(TimeSpan.FromSeconds(date.Second));
            date = date.Truncate(TimeSpan.FromMilliseconds(date.Millisecond));
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
