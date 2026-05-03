namespace Aquasip.Models
{
    public class NavbarVM
    {
        public string? Name { get; set; } = null!;
        public string? Slug { get; set; } = null!;
        public List<PageVM> Pages { get; set; } = new List<PageVM>();
    }
}
