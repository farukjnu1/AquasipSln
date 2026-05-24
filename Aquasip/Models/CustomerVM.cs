using Aquasip.EF;
using System.ComponentModel.DataAnnotations;

namespace Aquasip.Models
{
    public class CustomerVM
    {
        public long CustomerId { get; set; }

        public string CustomerCode { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }
        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        [StringLength(255)]
        [Compare("PasswordHash")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
