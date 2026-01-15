namespace Aquasip.Models
{
    public class RolePermissionVM
    {
        public int RolePermissionId { get; set; }

        public int? RoleId { get; set; }

        public string? Controller { get; set; }

        public bool? IsActive { get; set; }
    }
}
