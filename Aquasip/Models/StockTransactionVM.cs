namespace Aquasip.Models
{
    public partial class StockTransactionVM
    {
        public long StockTransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public long ProductId { get; set; }

        public int? TransactionTypeId { get; set; }

        public int? ReferenceTypeId { get; set; }

        public long? ReferenceId { get; set; }

        public decimal? QtyIn { get; set; }

        public decimal? QtyOut { get; set; }

        public decimal? UnitCost { get; set; }

        public int? StoreId { get; set; }

        public bool? IsActive { get; set; }

        public virtual ProductVM Product { get; set; } = null!;
        public virtual ReferenceTypeVM ReferenceType { get; set; } = null!;
        public virtual StoreVM Store { get; set; } = null!;
        public virtual TransactionTypeVM TransactionType { get; set; } = null!;
    }

}
