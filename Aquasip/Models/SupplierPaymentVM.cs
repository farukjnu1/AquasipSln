using Aquasip.EF;

namespace Aquasip.Models
{
    public class SupplierPaymentVM
    {
        public long PaymentId { get; set; }

        public long PurchaseOrderId { get; set; }

        public int PaymentMethodId { get; set; }

        public string? TransactionNumber { get; set; }

        public decimal PaidAmount { get; set; }

        public int PaymentStatusId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? Remarks { get; set; }

        public bool? IsActive { get; set; }

        public virtual PaymentMethodVM PaymentMethod { get; set; } = null!;

        public virtual PaymentStatusVM PaymentStatus { get; set; } = null!;

        public virtual PurchaseOrderVM PurchaseOrder { get; set; } = null!;
    }
}
