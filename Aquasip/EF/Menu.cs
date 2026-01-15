using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Menu
{
    public int MenuId { get; set; }

    public string Label { get; set; } = null!;

    public int? PageId { get; set; }

    public int? ParentId { get; set; }

    public int Position { get; set; }

    public virtual ICollection<Menu> InverseParent { get; set; } = new List<Menu>();

    public virtual Page? Page { get; set; }

    public virtual Menu? Parent { get; set; }
}
