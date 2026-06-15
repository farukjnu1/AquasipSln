using Aquasip.EF;

namespace Aquasip.Models
{
    public class CustomerPaymentVM
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
        public virtual SalesOrderVM Order { get; set; } = null!;

        public virtual PaymentMethodVM PaymentMethod { get; set; } = null!;

        public virtual PaymentStatusVM PaymentStatus { get; set; } = null!;

        public virtual PurchaseOrderVM PurchaseOrder { get; set; } = null!;
        public string? PaymentMethod1 { get; set; }
        public string? PaymentStatus1 { get; set; }
    }

}
