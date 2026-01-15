namespace Aquasip.Models
{
    public class UserRoleVM
    {
        public int UserRoleId { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
