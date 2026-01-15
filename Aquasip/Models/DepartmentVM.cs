namespace Aquasip.Models
{
    public class DepartmentVM
    {
        public int DepartmentId { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
