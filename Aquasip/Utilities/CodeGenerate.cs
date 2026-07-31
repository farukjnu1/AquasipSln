using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Aquasip.Utilities
{
    public class CodeGenerate
    {
        public static string SalesOrderNum(DateTime currentDate)
        {
            var startDate = new DateTime(2026, 1, 1, 1, 1, 1);
            // Calculate total seconds since start-date
            string timestamp = TimeConversion.DateDifferenceInSeconds(currentDate, startDate).ToString("F0");
            // Combine last 2 digits of phone number with timestamp
            return "SO" + timestamp;
        }

        public static string SalesReturnNum(DateTime currentDate)
        {
            var startDate = new DateTime(2026, 1, 1, 1, 1, 1);
            // Calculate total seconds since start-date
            string timestamp = TimeConversion.DateDifferenceInSeconds(currentDate, startDate).ToString("F0");
            return "SR" + timestamp;
        }

        public static string PurchaseOrderNum(DateTime currentDate)
        {
            var startDate = new DateTime(2026, 1, 1, 1, 1, 1);
            // Calculate total seconds since start-date
            string timestamp = "PO" + TimeConversion.DateDifferenceInSeconds(currentDate, startDate).ToString("F0");
            return timestamp;
        }

        public static string PurchaseReturnNum(DateTime currentDate)
        {
            var startDate = new DateTime(2026, 1, 1, 1, 1, 1);
            // Calculate total seconds since start-date
            string timestamp = "PR" + TimeConversion.DateDifferenceInSeconds(currentDate, startDate).ToString("F0");
            return timestamp;
        }

        public static string CustomerNum(DateTime currentDate)
        {
            var startDate = new DateTime(2026, 1, 1, 1, 1, 1);
            // Calculate total seconds since start-date
            string timestamp = "C" + TimeConversion.DateDifferenceInSeconds(currentDate, startDate).ToString("F0");
            return timestamp;
        }

        public static string ContactMessageNum(DateTime currentDate)
        {
            var startDate = new DateTime(2026, 1, 1, 1, 1, 1);
            // Calculate total seconds since start-date
            string timestamp = "CM" + TimeConversion.DateDifferenceInSeconds(currentDate, startDate).ToString("F0");
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
