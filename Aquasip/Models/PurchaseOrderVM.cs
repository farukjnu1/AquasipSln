using Aquasip.EF;

namespace Aquasip.Models
{
    public class PurchaseOrderVM
    {
        public long PurchaseOrderId { get; set; }

        public string? Ponumber { get; set; }

        public DateTime Podate { get; set; }

        public int SupplierId { get; set; }

        public decimal? SubTotal { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? TaxPercent { get; set; }

        public decimal? OtherCharge { get; set; }

        public decimal? TaxAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        public string? Remark { get; set; }

        public int? PurchaseStateId { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<PurchaseOrderDetailVM> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetailVM>();

        public virtual ICollection<PurchaseReturnVM> PurchaseReturns { get; set; } = new List<PurchaseReturnVM>();

        public virtual SupplierVM Supplier { get; set; } = null!;

        public virtual ICollection<SupplierPayment> SupplierPayments { get; set; } = new List<SupplierPayment>();
    }

}
