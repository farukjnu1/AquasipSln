using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Aquasip.EF;
using Aquasip.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Aquasip.Repositories
{
    public class CustomerRepository
    {
        private readonly string _connectionString = "";
        #region constructor
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        // Create
        public string? Add(CustomerVM model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Customer_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
            cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
            cmd.Parameters.AddWithValue("@FullName", model.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
            cmd.Parameters.AddWithValue("@ConfirmPassword", model.ConfirmPassword);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.Insert);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        public string? Update(CustomerVM model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Customer_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
            cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
            cmd.Parameters.AddWithValue("@FullName", model.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
            cmd.Parameters.AddWithValue("@ConfirmPassword", model.ConfirmPassword);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.Update);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Delete
        public string? Delete(int customerId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Customer_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.Delete);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        public string? UpdateEmailVerify(CustomerVM model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Customer_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
            cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
            cmd.Parameters.AddWithValue("@FullName", model.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
            cmd.Parameters.AddWithValue("@ConfirmPassword", model.ConfirmPassword);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.UpdateEmailVerify);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Read all
        public List<CustomerVM> GetAll()
        {
            var list = new List<CustomerVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customer_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CustomerVM model = new CustomerVM();
                            model.CustomerCode = reader.GetValue("CustomerCode") == DBNull.Value ? string.Empty : reader.GetString("CustomerCode");
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.CustomerId = reader.GetValue("CustomerId") == DBNull.Value ? 0 : reader.GetInt32("CustomerId");
                            model.FullName = reader.GetValue("FullName") == DBNull.Value ? string.Empty : reader.GetString("FullName");
                            model.PhoneNumber = reader.GetValue("PhoneNumber") == DBNull.Value ? string.Empty : reader.GetString("PhoneNumber");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                            
                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read one
        public CustomerVM? GetById(int customerId)
        {
            CustomerVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customer_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new CustomerVM();
                            model.CustomerCode = reader.GetValue("CustomerCode") == DBNull.Value ? string.Empty : reader.GetString("CustomerCode");
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.CustomerId = reader.GetValue("CustomerId") == DBNull.Value ? 0 : reader.GetInt32("CustomerId");
                            model.FullName = reader.GetValue("FullName") == DBNull.Value ? string.Empty : reader.GetString("FullName");
                            model.PhoneNumber = reader.GetValue("PhoneNumber") == DBNull.Value ? string.Empty : reader.GetString("PhoneNumber");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public CustomerVM? GetByEmail(string email)
        {
            CustomerVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customer_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.GetByEmail);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new CustomerVM();
                            model.CustomerCode = reader.GetValue("CustomerCode") == DBNull.Value ? string.Empty : reader.GetString("CustomerCode");
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.CustomerId = reader.GetValue("CustomerId") == DBNull.Value ? 0 : reader.GetInt64("CustomerId");
                            model.FullName = reader.GetValue("FullName") == DBNull.Value ? string.Empty : reader.GetString("FullName");
                            model.PhoneNumber = reader.GetValue("PhoneNumber") == DBNull.Value ? string.Empty : reader.GetString("PhoneNumber");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public CustomerVM Signin(CustomerVM model)
        {
            CustomerVM? oCustomer = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customer_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
                    cmd.Parameters.AddWithValue("@QueryType", CustomerVM.QueryType.Signin);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            oCustomer = new CustomerVM();
                            oCustomer.CustomerCode = reader.GetValue("CustomerCode") == DBNull.Value ? string.Empty : reader.GetString("CustomerCode");
                            oCustomer.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            oCustomer.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            oCustomer.CustomerId = reader.GetValue("CustomerId") == DBNull.Value ? 0 : reader.GetInt64("CustomerId");
                            oCustomer.FullName = reader.GetValue("FullName") == DBNull.Value ? string.Empty : reader.GetString("FullName");
                            oCustomer.PhoneNumber = reader.GetValue("PhoneNumber") == DBNull.Value ? string.Empty : reader.GetString("PhoneNumber");
                            oCustomer.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                        }
                    }
                    conn.Close();
                }
            }
            return oCustomer;
        }

    }
}
