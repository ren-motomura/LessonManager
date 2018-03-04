using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonManager.Utils
{
    class Time
    {
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime TimestampToDateTime(long timestamp)
        {
            return epoch.Add(TimeSpan.FromSeconds(timestamp)).ToLocalTime();
        }
        public static long DateTimeToTimestamp(DateTime dt)
        {
            return Convert.ToInt64(TimeZoneInfo.ConvertTimeToUtc(dt).Subtract(epoch).TotalSeconds);
        }
    }
}
