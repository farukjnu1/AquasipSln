using Aquasip.EF;

namespace Aquasip.Models
{
    public class PurchaseReturnVM
    {
        public long PurchaseReturnId { get; set; }

        public string? ReturnNumber { get; set; }

        public DateTime ReturnDate { get; set; }

        public long? PurchaseOrderId { get; set; }

        public int SupplierId { get; set; }

        public decimal? TotalAmount { get; set; }
        public string? Remark { get; set; }

        public bool? IsActive { get; set; }

        public virtual PurchaseOrderVM? PurchaseOrder { get; set; }

        public virtual ICollection<PurchaseReturnDetailVM> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetailVM>();

        public virtual SupplierVM Supplier { get; set; } = null!;
    }

}
