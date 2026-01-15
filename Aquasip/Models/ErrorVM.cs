using System.Text;

namespace Aquasip.Models
{
    public class ErrorVM
    {
        private readonly IWebHostEnvironment _environment;

        public ErrorVM(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void WriteLog(string? message)
        {
            #region Create File
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "logs");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = ".txt";
            //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // to milliseconds
            DateTime currentDateTime = DateTime.Now;
            string timeStamp = currentDateTime.ToString("yyyyMMdd") + "_" + currentDateTime.ToString("HHmmss") + "_" + currentDateTime.ToString("fff");
            string uniqueFileName = $"{timeStamp}{extension}";

            var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uniqueFileName));

            // Write to the file (append or overwrite)
            using (StreamWriter writer = new StreamWriter(filePath, append: true, Encoding.UTF8))
            {
                writer.WriteLine($"{message}");
            }
            #endregion
        }

    }
}
