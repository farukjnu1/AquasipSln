using Microsoft.Data.SqlClient;
using Aquasip.Models;
using static Aquasip.Repositories.UserRepository;
using System.Data;
using Aquasip.EF;

namespace Aquasip.Repositories
{
    public class SiteSettingRepository
    {
        private readonly string _connectionString = "";
        public SiteSettingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // Read
        public List<SiteSettingVM> GetAll()
        {
            List<SiteSettingVM> list = new List<SiteSettingVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SiteSetting_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QueryType", SiteSettingVM.QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SiteSettingVM model = new SiteSettingVM();
                            model.SettingValue = reader.IsDBNull("SettingValue") ? "" : reader.GetString("SettingValue");
                            model.SettingKey = reader.IsDBNull("SettingKey") ? "" : reader.GetString("SettingKey");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read by id
        public List<SiteSettingVM> GetById(string SettingKey)
        {
            List<SiteSettingVM> list = new List<SiteSettingVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SiteSetting_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QueryType", SiteSettingVM.QueryType.GetById);
                    cmd.Parameters.AddWithValue("@SettingKey", SettingKey);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SiteSettingVM model = new SiteSettingVM();
                            model.SettingValue = reader.GetString("SettingValue");
                            model.SettingKey = reader.GetString("SettingKey");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

    }
}
