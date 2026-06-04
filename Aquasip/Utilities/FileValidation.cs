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
    }
}
