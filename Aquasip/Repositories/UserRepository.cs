using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Aquasip.EF;
using Aquasip.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Aquasip.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString = "";
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // Create
        public string? Add(UserVM user)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("User_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@ConfirmPassword", user.ConfirmPassword);
            cmd.Parameters.AddWithValue("@CreateBy", user.CreateBy);
            cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.Insert);

            conn.Open();
            //cmd.ExecuteNonQuery();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        public string? Update(UserVM user)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("User_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@Email", user.Email);
            //cmd.Parameters.AddWithValue("@Username", user.Username);
            //cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@CreateBy", user.CreateBy);
            cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.Update);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Update
        public string? UpdateEmail(UserVM user)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("User_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", user.Email);
            //cmd.Parameters.AddWithValue("@Username", user.Username);
            //cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@CreateBy", user.CreateBy);
            cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.UpdateEmail);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Update
        public string? UpdateUsername(UserVM user)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("User_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            //cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@CreateBy", user.CreateBy);
            cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.UpdateUsername);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Update
        public string? UpdatePassword(UserVM user)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("User_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@Email", user.Email);
            //cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@ConfirmPassword", user.ConfirmPassword);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);
            cmd.Parameters.AddWithValue("@CreateBy", user.CreateBy);
            cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.UpdatePassword);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Delete
        public string? Delete(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("User_Modify", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@CreateBy", userId);
            cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.Delete);

            conn.Open();
            string? messageSQL = Convert.ToString(cmd.ExecuteScalar());
            return messageSQL;
        }

        // Read all
        public List<UserVM> GetAll()
        {
            var list = new List<UserVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("User_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserVM model = new UserVM();
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.UserID = reader.GetValue("UserID") == DBNull.Value ? 0 : reader.GetInt32("UserID");
                            model.Username = reader.GetValue("Username") == DBNull.Value ? string.Empty : reader.GetString("Username");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                            model.RoleName = reader.GetValue("Description") == DBNull.Value ? string.Empty : reader.GetString("Description");
                            
                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read one
        public UserVM? GetById(int userId)
        {
            UserVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("User_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new UserVM();
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email");
                            model.UserID = reader.GetValue("UserID") == DBNull.Value ? 0 : reader.GetInt32("UserID");
                            model.Username = reader.GetValue("Username") == DBNull.Value ? string.Empty : reader.GetString("Username");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                            model.RoleName = reader.GetValue("Description") == DBNull.Value ? string.Empty : reader.GetString("Description");
                            model.RoleId = reader.GetValue("RoleId") == DBNull.Value ? 0 : reader.GetInt32("RoleId");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public UserVM Login(UserVM model)
        {
            UserVM oUser = new UserVM();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("User_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@QueryType", UserVM.QueryType.Login);

                    conn.Open();
                    string? output = Convert.ToString(cmd.ExecuteScalar());
                    int userId = 0;
                    bool isValid = int.TryParse(output, out userId);
                    oUser.IsActive = isValid;
                    oUser.UserID = userId;
                    oUser.Username = model.Username;
                    conn.Close();
                }
            }
            return oUser;
        }

    }
}
