namespace Aquasip.Models
{
    public class RoleVM
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }

        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4, GetPermissionByRole = 5
        }
    }
}
