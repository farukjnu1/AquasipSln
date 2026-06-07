using Aquasip.EF;

namespace Aquasip.Models
{
    public class SalesOrderVM
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
        public bool? IsActive { get; set; }

        public virtual ICollection<SalesOrderDetailVM> OrderDetails { get; set; } = new List<SalesOrderDetailVM>();

        public virtual CustomerVM Customer { get; set; } = null!;
        //public string? CustomerName { get; set; }
        //public string? OrderStatus { get; set; }
        //public string? PaymentMethod { get; set; }
        //public string? StreetAddress { get; set; }
        //public string? ColorCode { get; set; }

        //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual PaymentMethodVM PaymentMethod { get; set; } = null!;

        public virtual ICollection<CustomerPaymentVM> CustomerPayments { get; set; } = new List<CustomerPaymentVM>();

        public virtual ShippingAddressVM ShippingAddress { get; set; } = null!;
        public virtual CustomerPaymentVM CustomerPayment { get; set; } = null!;
        public virtual SalesOrderStateVM SalesOrderState { get; set; } = null!;
        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4
        }

        public class SalesOrderDetailVM
        {
            public long OrderDetailId { get; set; }

            public long OrderId { get; set; }

            public long ProductId { get; set; }

            public int Qty { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal TotalPrice { get; set; }

            public virtual SalesOrderVM Order { get; set; } = null!;

            public virtual ProductVM Product { get; set; } = null!;
            public string ProductName { get; set; } = null!;

        }
    }
}



