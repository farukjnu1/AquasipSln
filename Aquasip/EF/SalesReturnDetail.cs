using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class SalesReturnDetail
{
    public long SalesReturnDetailId { get; set; }

    public long SalesReturnId { get; set; }

    public long ProductId { get; set; }

    public decimal? Qty { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }

    public int? StoreId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual SalesReturn SalesReturn { get; set; } = null!;
}
