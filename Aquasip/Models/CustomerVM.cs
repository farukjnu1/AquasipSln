using Aquasip.EF;
using System.ComponentModel.DataAnnotations;

namespace Aquasip.Models
{
    public class CustomerVM:ResponseVM
    {
        public long CustomerId { get; set; }

        public string CustomerCode { get; set; } = null!;

        public string? FullName { get; set; }
        public string? ShortName { get; set; }

        public string? PhoneNumber { get; set; }
        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; } = false;

        [StringLength(255)]
        [Compare("PasswordHash")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4, UpdateEmailVerify = 5, Signin = 6, GetByEmail = 7
        }
    }
}
