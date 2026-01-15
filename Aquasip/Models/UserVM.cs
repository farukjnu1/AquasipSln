using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aquasip.Models
{
    public class UserVM
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]

        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Matches SQL default
        public bool IsActive { get; set; } = false;
        public int? CreateBy { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> RoleOptions { get; set; } = new List<SelectListItem>();
        public List<UserRoleVM> UserRoles { get; set; } = new List<UserRoleVM>();
        public QueryType QueryTypes { get;set; }
        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4, UpdateEmail = 5, UpdateUsername = 6, UpdatePassword = 7, Login = 8
        }
    }
}
