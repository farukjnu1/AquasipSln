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
    }
}
