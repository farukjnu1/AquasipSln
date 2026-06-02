using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class PurchaseOrderDetail
{
    public long PurchaseOrderDetailId { get; set; }

    public long PurchaseOrderId { get; set; }

    public long ProductId { get; set; }

    public decimal? Qty { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? LineTotal { get; set; }

    public int? StoreId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
