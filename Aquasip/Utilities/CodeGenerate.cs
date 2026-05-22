namespace Aquasip.Utilities
{
    public class CodeGenerate
    {
        public static string SalesOrderNumber(DateTime dateTime, string phone)
        {
            // Calculate total seconds since epoch
            string timestamp = TimeConversion.DateTimeToUnixTimestamp(dateTime).ToString();

            // Get last 2 digits of phone number
            string last2Digits = phone.Substring(phone.Length - 2);

            // Combine last 2 digits of phone number with timestamp
            return last2Digits + timestamp;
        }
    }
}
