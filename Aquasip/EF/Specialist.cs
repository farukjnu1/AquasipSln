using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Specialist
{
    public int SpecialistId { get; set; }

    public string? FullName { get; set; }

    public string? Designation { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}
