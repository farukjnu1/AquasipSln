namespace Aquasip.Utilities
{
    public class FileValidation
    {
        public static bool IsValidFileForReview(string fileName)
        {
            string[] allowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif"
            };

            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        public static string GetFileNameFromURL(string url)
        {
            var nDirectories = url.Split('/').Length;
            var fileName = url.Split('/')[nDirectories - 1];
            return fileName;
        }

    }
}
