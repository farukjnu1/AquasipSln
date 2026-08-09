using Aquasip.EF;

namespace Aquasip.Models
{
    public class ShippingAddressVM
    {
        public long ShippingAddressId { get; set; }

        public long CustomerId { get; set; }

        public string FullName { get; set; } = null!;

        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public string StreetAddress { get; set; } = null!;

        public string City { get; set; } = null!;

        public string? StateProvince { get; set; }

        public string? PostalCode { get; set; }

        public string? CountryCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CustomerVM customer { get; set; } = null!;

        public virtual ICollection<SalesOrder> Orders { get; set; } = new List<SalesOrder>();
    }
}
