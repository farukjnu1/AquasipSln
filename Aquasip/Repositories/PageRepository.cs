using Microsoft.Data.SqlClient;
using Aquasip.Models;
using System.Data;
using Aquasip.EF;

namespace Aquasip.Repositories
{
    public class PageRepository
    {
        private readonly string _connectionString = "";
        public PageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Read
        public List<PageVM> GetAll()
        {
            List<PageVM> list = new List<PageVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Page_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PageVM model = new PageVM();
                            model.PageId = reader.GetInt32("PageId");
                            model.AuthorId = reader.GetValue("AuthorId") == DBNull.Value ? (int?)null : reader.GetInt32("AuthorId");
                            model.Slug = reader.GetValue("Slug") == DBNull.Value ? (string?)null : reader.GetString("Slug");
                            model.Status = reader.GetValue("Status") == DBNull.Value ? (string?)null : reader.GetString("Status");
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.PublishedAt = reader.GetValue("PublishedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("PublishedAt");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? (string?)null : reader.GetString("Title");
                            model.Content = reader.GetValue("Content") == DBNull.Value ? (string?)null : reader.GetString("Content");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read by id
        public PageVM GetById(int PageID)
        {
            PageVM model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Page_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageID", PageID);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new PageVM();
                            model.PageId = reader.GetInt32("PageId");
                            model.AuthorId = reader.GetValue("AuthorId") == DBNull.Value ? (int?)null : reader.GetInt32("AuthorId");
                            model.Slug = reader.GetValue("Slug") == DBNull.Value ? (string?)null : reader.GetString("Slug");
                            model.Status = reader.GetValue("Status") == DBNull.Value ? (string?)null : reader.GetString("Status");
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.PublishedAt = reader.GetValue("PublishedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("PublishedAt");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? (string?)null : reader.GetString("Title");
                            model.Content = reader.GetValue("Content") == DBNull.Value ? (string?)null : reader.GetString("Content");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public PageVM GetBySlug(string Slug)
        {
            PageVM model = new PageVM();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Page_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Slug", Slug);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetBySlug);
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                model = new PageVM();
                                model.PageId = reader.GetInt32("PageId");
                                model.AuthorId = reader.GetValue("AuthorId") == DBNull.Value ? (int?)null : reader.GetInt32("AuthorId");
                                model.Slug = reader.GetValue("Slug") == DBNull.Value ? (string?)null : reader.GetString("Slug");
                                model.Status = reader.GetValue("Status") == DBNull.Value ? (string?)null : reader.GetString("Status");
                                model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                                model.PublishedAt = reader.GetValue("PublishedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("PublishedAt");
                                model.Title = reader.GetValue("Title") == DBNull.Value ? (string?)null : reader.GetString("Title");
                                model.Content = reader.GetValue("Content") == DBNull.Value ? (string?)null : reader.GetString("Content");
                            }
                        }
                        conn.Close();
                    }
                    catch
                    {
                    }
                }
            }
            return model;
        }

        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4, GetBySlug
        }

    }
}
