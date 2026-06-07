using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class CustomerPayment
{
    public long PaymentId { get; set; }

    public long OrderId { get; set; }

    public int PaymentMethodId { get; set; }

    public string? TransactionNumber { get; set; }

    public decimal PaidAmount { get; set; }

    public int PaymentStatusId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Remarks { get; set; }

    public bool? IsActive { get; set; }

    public virtual SalesOrder Order { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;
}
