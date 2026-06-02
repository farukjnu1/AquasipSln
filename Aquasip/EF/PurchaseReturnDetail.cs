using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class PurchaseReturnDetail
{
    public long PurchaseReturnDetailId { get; set; }

    public long PurchaseReturnId { get; set; }

    public long ProductId { get; set; }

    public decimal? Qty { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? LineTotal { get; set; }

    public int? StoreId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseReturn PurchaseReturn { get; set; } = null!;
}
