using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? SpecialistId { get; set; }

    public DateTime? AppointmentDate { get; set; }

    public string? FullName { get; set; }

    public int? Gender { get; set; }

    public decimal? Age { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? ProblemDetail { get; set; }

    public bool? IsRead { get; set; }

    public int? ReadBy { get; set; }

    public int? DepartmentId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public int? CreateBy { get; set; }
}
