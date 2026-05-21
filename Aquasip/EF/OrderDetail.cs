using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class OrderDetail
{
    public long OrderDetailId { get; set; }

    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public int Qty { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
