using Microsoft.Data.SqlClient;
using Aquasip.Models;
using System.Data;
using Aquasip.EF;

namespace Aquasip.Repositories
{
    public class MenuRepository
    {
        private readonly string _connectionString = "";
        public MenuRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Read
        public List<MenuVM> GetAll()
        {
            List<MenuVM> list = new List<MenuVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Menu_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MenuVM model = new MenuVM();
                            model.MenuId = reader.GetValue("MenuId") == DBNull.Value ? 0 : reader.GetInt32("MenuId");
                            model.Label = reader.GetValue("Label") == DBNull.Value ? "" : reader.GetString("Label");
                            model.PageId = reader.GetValue("PageId") == DBNull.Value ? 0 : reader.GetInt32("PageId");
                            model.ParentId = reader.GetValue("ParentId") == DBNull.Value ? 0 : reader.GetInt32("ParentId");
                            model.Position = reader.GetValue("Position") == DBNull.Value ? 0 : reader.GetInt32("Position");

                            model.AuthorId = reader.GetValue("AuthorId") == DBNull.Value ? 0 : reader.GetInt32("AuthorId");
                            model.Slug = reader.GetValue("Slug") == DBNull.Value ? "" : reader.GetString("Slug");
                            model.Status = reader.GetValue("Status") == DBNull.Value ? "" : reader.GetString("Status");
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.PublishedAt = reader.GetValue("PublishedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("PublishedAt");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? "" : reader.GetString("Title");
                            model.Content = reader.GetValue("Content") == DBNull.Value ? "" : reader.GetString("Content");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read by id
        public MenuVM GetById(int PageID)
        {
            MenuVM model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Menu_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MenuID", PageID);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new MenuVM();
                            model.MenuId = reader.GetInt32("MenuId");
                            model.Label = reader.GetString("Label");
                            model.PageId = reader.GetInt32("PageId");
                            model.ParentId = reader.GetInt32("ParentId");
                            model.Position = reader.GetInt32("Position");

                            model.PageId = reader.GetInt32("PageId");
                            model.AuthorId = reader.GetInt32("AuthorId");
                            model.Slug = reader.GetString("Slug");
                            model.Status = reader.GetString("Status");
                            model.CreatedAt = reader.GetDateTime("CreatedAt");
                            model.PublishedAt = reader.GetDateTime("PublishedAt");
                            model.Title = reader.GetString("Title");
                            model.Content = reader.GetString("Content");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4
        }

    }
}
