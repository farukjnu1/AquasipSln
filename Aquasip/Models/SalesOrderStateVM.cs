namespace Aquasip.Models
{
    public class SalesOrderStateVM
    {
        public int OrderStateId { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string? Remark { get; set; }

        public int? Sequence { get; set; }

        public string? ColorCode { get; set; }

        public bool? IsActive { get; set; }
    }
}
