using System.Text;
using static System.Net.Mime.MediaTypeNames;

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

        public static string PurchaseOrderNumber(DateTime dateTime)
        {
            // Calculate total seconds since epoch
            string timestamp = TimeConversion.DateTimeToUnixTimestamp(dateTime).ToString();

            return timestamp;
        }

        public static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string TextToHex(string text)
        {
            //string text = "Hello";
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            string hex = Convert.ToHexString(bytes);
            // 48656C6C6F
            return hex;
        }

        public static string HexToText(string hex)
        {
            //string hex = "48656C6C6F";
            byte[] bytes = Convert.FromHexString(hex);
            string text = Encoding.UTF8.GetString(bytes);
            // Hello
            return text;
        }

    }
}
