using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class PurchaseOrder
{
    public long PurchaseOrderId { get; set; }

    public string? Ponumber { get; set; }

    public DateTime Podate { get; set; }

    public int SupplierId { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; } = new List<PurchaseReturn>();

    public virtual Supplier Supplier { get; set; } = null!;
}
