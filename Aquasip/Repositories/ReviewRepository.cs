using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Aquasip.Repositories
{
    public class ReviewRepository
    {
        private readonly string _connectionString = "";
        #region constructor
        public ReviewRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion

        // Read all
        public List<ReviewVM> GetAll()
        {
            var list = new List<ReviewVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Review_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", ReviewVM.QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReviewVM model = new ReviewVM();
                            model.Attaches = reader.GetValue("Attaches") == DBNull.Value ? string.Empty : reader.GetString("Attaches");
                            model.ReviewMedia = JsonConversion.DeserializeObject<ICollection<ReviewMediumVM>>(model.Attaches);
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.CreatedAtPast = reader.GetValue("CreatedAtPast") == DBNull.Value ? string.Empty : reader.GetString("CreatedAtPast");
                            model.CustomerId = reader.GetValue("CustomerId") == DBNull.Value ? 0 : reader.GetInt64("CustomerId");
                            model.Customer = new CustomerVM
                            {
                                CustomerId = model.CustomerId,
                                FullName = reader.GetValue("FullName") == DBNull.Value ? string.Empty : reader.GetString("FullName"),
                                ShortName = reader.GetValue("ShortName") == DBNull.Value ? string.Empty : reader.GetString("ShortName"),
                                Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email")
                            };
                            model.ReviewId = reader.GetValue("ReviewId") == DBNull.Value ? 0 : reader.GetInt64("ReviewId");
                            model.ProductId = reader.GetValue("ProductId") == DBNull.Value ? 0 : reader.GetInt64("ProductId");
                            model.Product = new ProductVM
                            {
                                ProductId = model.ProductId,
                                ProductName = reader.GetValue("ProductName") == DBNull.Value ? string.Empty : reader.GetString("ProductName"),
                                Description = reader.GetValue("Description") == DBNull.Value ? string.Empty : reader.GetString("Description")
                            };
                            model.Title = reader.GetValue("Title") == DBNull.Value ? string.Empty : reader.GetString("Title");
                            model.ReviewText = reader.GetValue("ReviewText") == DBNull.Value ? string.Empty : reader.GetString("ReviewText");
                            model.Rating = reader.GetValue("Rating") == DBNull.Value ? 0 : reader.GetInt32("Rating");
                            model.IsApproved = reader.GetValue("IsApproved") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsApproved");
                            model.IsDeleted = reader.GetValue("IsDeleted") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsDeleted");
                            model.ModerationStatus = reader.GetValue("ModerationStatus") == DBNull.Value ? string.Empty : reader.GetString("ModerationStatus");
                            model.ModerationStatus = reader.GetValue("ModerationStatus") == DBNull.Value ? string.Empty : reader.GetString("ModerationStatus");
                            model.Helpful = reader.GetValue("Helpful") == DBNull.Value ? 0 : reader.GetInt32("Helpful");
                            model.NotHelpful = reader.GetValue("NotHelpful") == DBNull.Value ? 0 : reader.GetInt32("NotHelpful");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read one
        public ReviewVM? GetById(long id)
        {
            ReviewVM? model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Review_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReviewId", id);
                    cmd.Parameters.AddWithValue("@QueryType", ReviewVM.QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new ReviewVM();
                            model.Attaches = reader.GetValue("Attaches") == DBNull.Value ? string.Empty : reader.GetString("Attaches");
                            model.ReviewMedia = JsonConversion.DeserializeObject<ICollection<ReviewMediumVM>>(model.Attaches);
                            model.CreatedAt = reader.GetValue("CreatedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("CreatedAt");
                            model.CreatedAtPast = reader.GetValue("CreatedAtPast") == DBNull.Value ? string.Empty : reader.GetString("CreatedAtPast");
                            model.CustomerId = reader.GetValue("CustomerId") == DBNull.Value ? 0 : reader.GetInt64("CustomerId");
                            model.Customer = new CustomerVM
                            {
                                CustomerId = model.CustomerId,
                                FullName = reader.GetValue("FullName") == DBNull.Value ? string.Empty : reader.GetString("FullName"),
                                ShortName = reader.GetValue("ShortName") == DBNull.Value ? string.Empty : reader.GetString("ShortName"),
                                Email = reader.GetValue("Email") == DBNull.Value ? string.Empty : reader.GetString("Email")
                            };
                            model.ReviewId = reader.GetValue("ReviewId") == DBNull.Value ? 0 : reader.GetInt64("ReviewId");
                            model.ProductId = reader.GetValue("ProductId") == DBNull.Value ? 0 : reader.GetInt64("ProductId");
                            model.Product = new ProductVM
                            {
                                ProductId = model.ProductId,
                                ProductName = reader.GetValue("ProductName") == DBNull.Value ? string.Empty : reader.GetString("ProductName"),
                                Description = reader.GetValue("Description") == DBNull.Value ? string.Empty : reader.GetString("Description")
                            };
                            model.Title = reader.GetValue("Title") == DBNull.Value ? string.Empty : reader.GetString("Title");
                            model.ReviewText = reader.GetValue("ReviewText") == DBNull.Value ? string.Empty : reader.GetString("ReviewText");
                            model.Rating = reader.GetValue("Rating") == DBNull.Value ? 0 : reader.GetInt32("Rating");
                            model.IsApproved = reader.GetValue("IsApproved") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsApproved");
                            model.IsDeleted = reader.GetValue("IsDeleted") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsDeleted");
                            model.ModerationStatus = reader.GetValue("ModerationStatus") == DBNull.Value ? string.Empty : reader.GetString("ModerationStatus");
                            model.ModerationStatus = reader.GetValue("ModerationStatus") == DBNull.Value ? string.Empty : reader.GetString("ModerationStatus");
                            model.Helpful = reader.GetValue("Helpful") == DBNull.Value ? 0 : reader.GetInt32("Helpful");
                            model.NotHelpful = reader.GetValue("NotHelpful") == DBNull.Value ? 0 : reader.GetInt32("NotHelpful");

                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public bool UpdateReviewVote(ReviewVoteVM model)
        {
            bool isSave = false;
            try
            {
                using (var _context = new AquasipContext())
                {
                    var oReviewVote = (from rv in _context.ReviewVotes where rv.ReviewId == model.ReviewId && rv.CustomerId == model.CustomerId select rv).FirstOrDefault();
                    if (oReviewVote == null)
                    {
                        oReviewVote = new ReviewVote();
                        oReviewVote.ReviewId = model.ReviewId;
                        oReviewVote.CustomerId = model.CustomerId;
                        oReviewVote.IsHelpful = model.IsHelpful;
                        oReviewVote.CreatedAt = DateTime.UtcNow;
                        _context.ReviewVotes.Add(oReviewVote);
                        _context.SaveChanges();
                    }
                    else
                    {
                        oReviewVote.ReviewId = model.ReviewId;
                        oReviewVote.CustomerId = model.CustomerId;
                        oReviewVote.IsHelpful = model.IsHelpful;
                        oReviewVote.CreatedAt = DateTime.UtcNow;
                        _context.SaveChanges();
                    }
                    isSave = true;
                }
            }
            catch 
            {

            }
            return isSave;
        }

    }
}
