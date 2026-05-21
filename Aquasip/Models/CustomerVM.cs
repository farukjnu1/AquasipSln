using Aquasip.EF;

namespace Aquasip.Models
{
    public class CustomerVM
    {
        public long CustomerId { get; set; }

        public string CustomerCode { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

       
    }
}
