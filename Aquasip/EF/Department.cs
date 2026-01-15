using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
