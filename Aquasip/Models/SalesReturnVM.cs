using Aquasip.EF;
using System;
using System.Collections.Generic;

namespace Aquasip.Models
{
    public class SalesReturnVM
    {
        public long SalesReturnId { get; set; }

        public string? ReturnNumber { get; set; }

        public DateTime ReturnDate { get; set; }

        public long? SalesOrderId { get; set; }

        public long CustomerId { get; set; }

        public decimal? TotalAmount { get; set; }
        public string? Notes { get; set; }

        public bool? IsActive { get; set; }

        public virtual CustomerVM Customer { get; set; } = null!;

        public virtual SalesOrderVM? SalesOrder { get; set; }

        public virtual ICollection<SalesReturnDetailVM> SalesReturnDetails { get; set; } = new List<SalesReturnDetailVM>();
    }
}

    
