using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Aquasip.Repositories
{
    public class ProductRepository
    {
        private readonly string _connectionString = "";
        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Read
        public List<ProductVM> GetAll()
        {
            var list = new List<ProductVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Product_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetAll);
                    try 
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductVM model = new ProductVM();
                                model.ProductId = reader.GetInt64("ProductId");
                                model.ProductCode = reader.GetValue("ProductCode") == DBNull.Value ? "" : reader.GetString("ProductCode");
                                model.ProductName = reader.GetValue("ProductName") == DBNull.Value ? "" : reader.GetString("ProductName");
                                model.Description = reader.GetValue("Description") == DBNull.Value ? (string?)null : reader.GetString("Description");
                                model.Price = reader.GetValue("Price") == DBNull.Value ? (decimal?)null : reader.GetDecimal("Price");
                                model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsActive");
                                model.UploadedBy = reader.GetValue("UploadedBy") == DBNull.Value ? (int?)null : reader.GetInt32("UploadedBy");
                                model.UploadedAt = reader.GetValue("UploadedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("UploadedAt");
                                model.AverageRating = reader.GetValue("AverageRating") == DBNull.Value ? (decimal?)null : reader.GetDecimal("AverageRating");
                                model.TotalReviews = reader.GetValue("TotalReviews") == DBNull.Value ? (int?)null : reader.GetInt32("TotalReviews");
                                model.Medias = reader.GetValue("Medias") == DBNull.Value ? "" : reader.GetString("Medias");
                                model.ListProductMedia = string.IsNullOrEmpty(model.Medias) ? new List<ProductMediumVM>() : JsonConversion.DeserializeObject<List<ProductMediumVM>>(model.Medias);
                                list.Add(model);
                            }
                        }
                        conn.Close();
                    }
                    catch
                    { }
                }
            }
            return list;
        }

        // Read by id
        public ProductVM? GetById(long productId)
        {
            ProductVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Product_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetById);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new ProductVM();
                            model.ProductId = reader.GetInt64("ProductId");
                            model.ProductCode = reader.GetValue("ProductCode") == DBNull.Value ? "" : reader.GetString("ProductCode");
                            model.ProductName = reader.GetValue("ProductName") == DBNull.Value ? "" : reader.GetString("ProductName");
                            model.Description = reader.GetValue("Description") == DBNull.Value ? (string?)null : reader.GetString("Description");
                            model.Price = reader.GetValue("Price") == DBNull.Value ? (decimal?)null : reader.GetDecimal("Price");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsActive");
                            model.UploadedBy = reader.GetValue("UploadedBy") == DBNull.Value ? (int?)null : reader.GetInt32("UploadedBy");
                            model.UploadedAt = reader.GetValue("UploadedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("UploadedAt");
                            model.AverageRating = reader.GetValue("AverageRating") == DBNull.Value ? (decimal?)null : reader.GetDecimal("AverageRating");
                            model.TotalReviews = reader.GetValue("TotalReviews") == DBNull.Value ? (int?)null : reader.GetInt32("TotalReviews");
                            model.Medias = reader.GetValue("Medias") == DBNull.Value ? "" : reader.GetString("Medias");
                            model.ListProductMedia = string.IsNullOrEmpty(model.Medias) ? new List<ProductMediumVM>() : JsonConversion.DeserializeObject<List<ProductMediumVM>>(model.Medias);
                            model.Reviews = reader.GetValue("Reviews") == DBNull.Value ? "" : reader.GetString("Reviews");
                            //model.ListReview = JsonConversion.DeserializeObject<List<ReviewVM>>(reader.GetValue("Reviews") == DBNull.Value ? "" : reader.GetString("Reviews")) ?? new List<ReviewVM>();
                            model.ListReview = string.IsNullOrEmpty(model.Reviews) ? new List<ReviewVM>() : JsonConversion.DeserializeObject<List<ReviewVM>>(model.Reviews);
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
