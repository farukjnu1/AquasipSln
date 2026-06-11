using Aquasip.EF;

namespace Aquasip.Models
{
    public partial class PurchaseOrderDetailVM
    {
        public long PurchaseOrderDetailId { get; set; }

        public long PurchaseOrderId { get; set; }

        public long ProductId { get; set; }

        public decimal? Qty { get; set; }

        public decimal? UnitCost { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? LineTotal { get; set; }

        public int? StoreId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Product Product { get; set; } = null!;

        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
        public string ReferenceToken { get; set; } = null!;
        public string TransactionTypeToken { get; set; } = null!;
    }
}
