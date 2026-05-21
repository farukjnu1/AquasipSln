using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Order
{
    public long OrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public long CustomerId { get; set; }

    public long ShippingAddressId { get; set; }

    public int PaymentMethodId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal VatPercent { get; set; }

    public decimal VatAmount { get; set; }

    public decimal DeliveryCharge { get; set; }

    public decimal GatewayCharge { get; set; }

    public decimal GrandTotal { get; set; }

    public int OrderStateId { get; set; }

    public string? Notes { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ShippingAddress ShippingAddress { get; set; } = null!;
}
