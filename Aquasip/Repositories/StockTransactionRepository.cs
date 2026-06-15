using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Aquasip.EF;
using Aquasip.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Aquasip.Repositories
{
    public class StockTransactionRepository
    {
        private readonly string _connectionString = "";
        #region constructor
        public StockTransactionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        // Read one
        public List<StockTransactionVM> GetCurrentStock(long productId = 0, int storeId = 0)
        {
            List<StockTransactionVM> list = new List<StockTransactionVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("StockTransaction_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@StoreId", storeId);
                    cmd.Parameters.AddWithValue("@QueryType", StockTransactionVM.QueryType.CurrentStock);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StockTransactionVM model = new StockTransactionVM();
                            model.StoreId = reader.GetValue("StoreId") == DBNull.Value ? (int?)null : reader.GetInt32("StoreId");
                            model.ProductId = reader.GetValue("ProductId") == DBNull.Value ? 0 : reader.GetInt64("ProductId");
                            model.CurrentStock = reader.GetValue("CurrentStock") == DBNull.Value ? (decimal?)null : reader.GetDecimal("CurrentStock");
                            model.Product = new ProductVM { ProductId = model.ProductId, ProductName = reader.GetValue("ProductName") == DBNull.Value ? string.Empty : reader.GetString("ProductName") };
                            model.Store = new StoreVM { StoreId = (int)model.StoreId, StoreName = reader.GetValue("StoreName") == DBNull.Value ? string.Empty : reader.GetString("StoreName") };
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
