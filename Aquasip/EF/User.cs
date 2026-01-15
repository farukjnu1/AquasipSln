using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public int? CreateBy { get; set; }

    public virtual ICollection<Medium> Media { get; set; } = new List<Medium>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
