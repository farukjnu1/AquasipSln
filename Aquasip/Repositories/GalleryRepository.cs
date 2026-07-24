using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Aquasip.Repositories
{
    public class GalleryRepository
    {
        private readonly string _connectionString = "";
        public GalleryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Read
        public List<GalleryVM> GetAll()
        {
            var list = new List<GalleryVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Gallery_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetAll);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GalleryVM model = new GalleryVM();
                            model.GalleryId = reader.GetInt32("GalleryId");
                            model.Code = reader.GetValue("Code") == DBNull.Value ? "" : reader.GetString("Code");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? "" : reader.GetString("Title");
                            model.Header = reader.GetValue("Header") == DBNull.Value ? (string?)null : reader.GetString("Header");
                            model.Body = reader.GetValue("Body") == DBNull.Value ? (string?)null : reader.GetString("Body");
                            model.Footer = reader.GetValue("Footer") == DBNull.Value ? (string?)null : reader.GetString("Footer");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsActive");
                            model.UploadedBy = reader.GetValue("UploadedBy") == DBNull.Value ? (int?)null : reader.GetInt32("UploadedBy");
                            model.UploadedAt = reader.GetValue("UploadedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("UploadedAt");
                            model.Medias = reader.GetValue("Medias") == DBNull.Value ? "" : reader.GetString("Medias");

                            model.ListGalleryMedia = string.IsNullOrEmpty(model.Medias) ? new List<GalleryMediumVM>() : JsonConversion.DeserializeObject<List<GalleryMediumVM>>(model.Medias);

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read by id
        public GalleryVM? GetById(long productId)
        {
            GalleryVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Gallery_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@GalleryId", productId);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetById);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new GalleryVM();
                            model.GalleryId = reader.GetInt32("GalleryId");
                            model.Code = reader.GetValue("Code") == DBNull.Value ? "" : reader.GetString("Code");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? "" : reader.GetString("Title");
                            model.Header = reader.GetValue("Header") == DBNull.Value ? (string?)null : reader.GetString("Header");
                            model.Body = reader.GetValue("Body") == DBNull.Value ? (string?)null : reader.GetString("Body");
                            model.Footer = reader.GetValue("Footer") == DBNull.Value ? (string?)null : reader.GetString("Footer");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsActive");
                            model.UploadedBy = reader.GetValue("UploadedBy") == DBNull.Value ? (int?)null : reader.GetInt32("UploadedBy");
                            model.UploadedAt = reader.GetValue("UploadedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("UploadedAt");
                            model.Medias = reader.GetValue("Medias") == DBNull.Value ? "" : reader.GetString("Medias");

                            model.ListGalleryMedia = string.IsNullOrEmpty(model.Medias) ? new List<GalleryMediumVM>() : JsonConversion.DeserializeObject<List<GalleryMediumVM>>(model.Medias);
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
