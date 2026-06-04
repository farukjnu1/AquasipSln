namespace Aquasip.Utilities
{
    public class TimeConversion
    {
        // Convert DateTime to Unix timestamp (seconds)
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            // Ensure the DateTime is in UTC
            DateTime utcDateTime = dateTime.ToUniversalTime();

            // Unix epoch start
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Calculate total seconds since epoch
            return (long)(utcDateTime - epoch).TotalSeconds;
        }

        public static DateTime UnixTimestampToDateTime(long unixSeconds)
        {
            // Convert to UTC DateTime
            // DateTime utcDateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;

            // Convert to Local System DateTime
            DateTime localDateTime = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime;

            return localDateTime;
        }

        public static double DateDifferenceInMinutes(DateTime startDate, DateTime endDate)
        {
            // Calculate the absolute difference in minutes
            return Math.Abs((endDate - startDate).TotalMinutes);
        }

    }
}
