using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class PurchaseOrderState
{
    public int PurchaseStateId { get; set; }

    public string PurchaseStatus { get; set; } = null!;

    public string? Remark { get; set; }

    public int? Sequence { get; set; }

    public string? ColorCode { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
