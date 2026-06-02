using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class SalesOrderDetail
{
    public long OrderDetailId { get; set; }

    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public int Qty { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public int? StoreId { get; set; }

    public bool? IsActive { get; set; }

    public virtual SalesOrder Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
