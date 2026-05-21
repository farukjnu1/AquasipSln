using Aquasip.EF;

namespace Aquasip.Models
{
    public class OrderVM
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
        public virtual ICollection<OrderDetailVM> OrderDetails { get; set; } = new List<OrderDetailVM>();

        public virtual CustomerVM Customer { get; set; } = null!;

        //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        //public virtual PaymentMethod PaymentMethod { get; set; } = null!;

        //public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

        public virtual ShippingAddressVM ShippingAddress { get; set; } = null!;
        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4
        }

        public class OrderDetailVM
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

    }
}



