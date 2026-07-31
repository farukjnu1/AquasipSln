namespace Aquasip.Models
{
    public class ContactMessageVM
    {
        public int ContactMessageId { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Subject { get; set; }

        public string? Message { get; set; }

        public bool? IsRead { get; set; }

        public int? ReadBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public bool? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public string MessageType { get; set; } = "";
    }
}
