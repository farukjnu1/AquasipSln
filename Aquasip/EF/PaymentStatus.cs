using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class PaymentStatus
{
    public int PaymentStateId { get; set; }

    public string PaymentStatus1 { get; set; } = null!;

    public string? Remark { get; set; }

    public int? Sequence { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
