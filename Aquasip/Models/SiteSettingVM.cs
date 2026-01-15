namespace Aquasip.Models
{
    public class SiteSettingVM
    {
        public string SettingKey { get; set; } = null!;

        public string? SettingValue { get; set; }

        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4, GetPermissionByRole = 5
        }
    }
}
