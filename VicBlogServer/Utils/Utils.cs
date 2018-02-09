using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Utils
{
    public static class Utils
    {
        public static long ToUnixUTCTime(this DateTime datetime)
        {
            return (long)((datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
        }

        public static bool IsBetween(this long time,  long min, long max)
        {
            return min <= time && time <= max;
        }

        public static DateTime ToLocalDateTime(this long utcTimestamp)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddMilliseconds(utcTimestamp).ToLocalTime();
        }
    }
}
