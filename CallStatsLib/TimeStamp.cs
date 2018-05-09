using System;

namespace CallStatsLib
{
    public class TimeStamp
    {
        public static long Now()
        {
            return DateTime.UtcNow.ToUnixTimeStamp();
        }
        public static long ExpireIn(TimeSpan time)
        {
            return DateTime.UtcNow.Add(time).ToUnixTimeStamp();
        }
        public static long ExpireInHours(double hours)
        {
            return DateTime.UtcNow.AddHours(hours).ToUnixTimeStamp();
        }
        public static long ExpireInDays(double days)
        {
            return DateTime.UtcNow.AddDays(days).ToUnixTimeStamp();
        }
        public static long ExpireInYears(int years)
        {
            return DateTime.UtcNow.AddYears(years).ToUnixTimeStamp();
        }
    }

    public static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Convert from .NET DateTime to UnixTimeStamp.
        /// (FYI in -> https://msdn.microsoft.com/en-us/library/system.datetimeoffset.tounixtimeseconds.aspx)
        /// </summary>
        /// <param name="dateTimeUtc">DateTimeUtc</param>
        /// <returns>Unix TimeStamp</returns>
        public static long ToUnixTimeStamp(this DateTime dateTimeUtc)
        {
            return (long)Math.Round((dateTimeUtc.ToUniversalTime() - UnixEpoch).TotalSeconds);
        }
    }
}
