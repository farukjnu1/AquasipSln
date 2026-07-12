using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class SalesReturn
{
    public long SalesReturnId { get; set; }

    public string? ReturnNumber { get; set; }

    public DateTime ReturnDate { get; set; }

    public long? SalesOrderId { get; set; }

    public long CustomerId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public bool? IsActive { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual SalesOrder? SalesOrder { get; set; }

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();
}
