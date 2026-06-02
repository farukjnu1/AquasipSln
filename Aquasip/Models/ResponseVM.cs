namespace Aquasip.Models
{
    public class ResponseVM
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string CallbackAction { get; set; } = null!;
        public string CallbackController { get; set; } = null!;
    }
}
