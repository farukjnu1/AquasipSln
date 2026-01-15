using Aquasip.EF;

namespace Aquasip.Models
{
    public class MenuVM:PageVM
    {
        public int MenuId { get; set; }

        public string Label { get; set; } = null!;

        public int? PageId { get; set; }

        public int? ParentId { get; set; }

        public int Position { get; set; }

        public virtual List<MenuVM> InverseParent { get; set; } = new List<MenuVM>();
    }
}
