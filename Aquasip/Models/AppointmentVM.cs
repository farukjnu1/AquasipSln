using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aquasip.Models
{
    public class AppointmentVM
    {
        public int AppointmentId { get; set; }

        public int? SpecialistId { get; set; }

        public DateTime? AppointmentDate { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        public int? Gender { get; set; }
        [Required]
        public decimal? Age { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Address { get; set; }

        public string? ProblemDetail { get; set; }
        public bool? IsRead { get; set; }

        public int? ReadBy { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateAt { get; set; }
        public int? CreateBy { get; set; }
        public IEnumerable<SelectListItem>? SpecialistOptions { get; set; }
        public IEnumerable<SelectListItem>? DepartmentOptions { get; set; }

        

        
    }
}
