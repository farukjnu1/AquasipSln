using Microsoft.Data.SqlClient;
using Aquasip.Models;
using System.Data;
using Aquasip.EF;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aquasip.Repositories
{
    public class PageContentRepository
    {
        private readonly string _connectionString = "";
        public PageContentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public PageContentVM Add(PageContentVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                PageContent oPageContent = new PageContent();
                oPageContent.Body = model.Body;
                oPageContent.Footer = model.Footer;
                oPageContent.Header = model.Header;
                oPageContent.IsActive = model.IsActive;
                oPageContent.MediaId = model.MediaId;
                oPageContent.Title = model.Title;
                oPageContent.UploadedAt = DateTime.Now;
                oPageContent.UploadedBy = model.UploadedBy;
                oPageContent.SlugPage = model.SlugPage;
                oPageContent.SlugPageContent = model.SlugPageContent;
                _context.Add(oPageContent);
                _context.SaveChanges();
                model.PageContentId = oPageContent.PageContentId;
                message = "data has been added successfully.";
            }
            return model;
        }

        public string Update(PageContentVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                PageContent? oPageContent = (from x in _context.PageContents where x.PageContentId == model.PageContentId select x).FirstOrDefault();
                if (oPageContent != null)
                {
                    oPageContent.Body = model.Body;
                    oPageContent.Footer = model.Footer;
                    oPageContent.Header = model.Header;
                    oPageContent.IsActive = model.IsActive;
                    oPageContent.MediaId = model.MediaId;
                    oPageContent.Title = model.Title;
                    oPageContent.UploadedAt = DateTime.Now;
                    oPageContent.UploadedBy = model.UploadedBy;
                    _context.SaveChanges();
                    #region Media
                    var oMedia = (from x in _context.Media where x.MediaId == oPageContent.MediaId select x).FirstOrDefault();
                    if (oMedia != null)
                    {
                        oMedia.UploadedBy = model.UploadedBy;
                        oMedia.UploadedAt = oPageContent.UploadedAt;
                        oMedia.Description = model.Description;
                        _context.SaveChanges();
                    }
                    #endregion
                    message = "data has been updated successfully.";
                }

            }
            return message;
        }

        // Read
        public List<PageContentVM> GetAll()
        {
            List<PageContentVM> list = new List<PageContentVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PageContent_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetAll);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PageContentVM model = new PageContentVM();
                            model.PageContentId = reader.GetInt32("PageContentId");
                            model.SlugPageContent = reader.GetString("SlugPageContent");
                            model.SlugPage = reader.GetString("SlugPage");
                            model.Title = reader.GetString("Title");
                            model.Header = reader.GetString("Header");
                            model.Body = reader.GetString("Body");
                            model.Footer = reader.GetString("Footer");
                            model.IsActive = reader.GetBoolean("IsActive");
                            model.MediaId = reader.GetInt32("ContMediaIdent");
                            model.UploadedAt = reader.GetDateTime("UploadedAt");
                            model.UploadedBy = reader.GetInt32("UploadedBy");

                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        // Read by id
        public PageContentVM GetById(int PageContentId)
        {
            PageContentVM model = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PageContent_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PageContentId", PageContentId);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetById);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new PageContentVM();
                            model.PageContentId = reader.GetInt32("PageContentId");

                            model.PageContentId = reader.GetInt32("PageContentId");
                            model.SlugPageContent = reader.GetValue("SlugPageContent") == DBNull.Value ? "" : reader.GetString("SlugPageContent");
                            model.SlugPage = reader.GetValue("SlugPage") == DBNull.Value ? "" : reader.GetString("SlugPage");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? "" : reader.GetString("Title");
                            model.Header = reader.GetValue("Header") == DBNull.Value ? (string?)null : reader.GetString("Header");
                            model.Body = reader.GetValue("Body") == DBNull.Value ? (string?)null : reader.GetString("Body");
                            model.Footer = reader.GetValue("Footer") == DBNull.Value ? (string?)null : reader.GetString("Footer");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsActive");
                            model.MediaId = reader.GetValue("MediaId") == DBNull.Value ? (int?)null : reader.GetInt32("MediaId");
                            model.UploadedAt = reader.GetValue("UploadedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("UploadedAt");
                            model.UploadedBy = reader.GetValue("UploadedBy") == DBNull.Value ? (int?)null : reader.GetInt32("UploadedBy");
                            model.FileName = reader.GetValue("FileName") == DBNull.Value ? "" : reader.GetString("FileName");
                            model.FilePath = reader.GetValue("FilePath") == DBNull.Value ? "" : reader.GetString("FilePath");
                            model.Description = reader.GetValue("Description") == DBNull.Value ? "" : reader.GetString("Description");
                        }
                    }
                    conn.Close();
                }
            }
            return model;
        }

        public List<PageContentVM> GetBySlugPage(string SlugPage)
        {
            List<PageContentVM> list = new List<PageContentVM>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PageContent_Read", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SlugPage", SlugPage);
                    cmd.Parameters.AddWithValue("@QueryType", QueryType.GetByPageSlug);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PageContentVM model = new PageContentVM();
                            model.PageContentId = reader.GetInt32("PageContentId");
                            model.SlugPageContent = reader.GetValue("SlugPageContent") == DBNull.Value ? "" : reader.GetString("SlugPageContent");
                            model.SlugPage = reader.GetValue("SlugPage") == DBNull.Value ? "" : reader.GetString("SlugPage");
                            model.Title = reader.GetValue("Title") == DBNull.Value ? "" : reader.GetString("Title");
                            model.Header = reader.GetValue("Header") == DBNull.Value ? (string?)null : reader.GetString("Header");
                            model.Body = reader.GetValue("Body") == DBNull.Value ? (string?)null : reader.GetString("Body");
                            model.Footer = reader.GetValue("Footer") == DBNull.Value ? (string?)null : reader.GetString("Footer");
                            model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? (bool?)null : reader.GetBoolean("IsActive");
                            model.MediaId = reader.GetValue("MediaId") == DBNull.Value ? (int?)null : reader.GetInt32("MediaId");
                            model.UploadedAt = reader.GetValue("UploadedAt") == DBNull.Value ? (DateTime?)null : reader.GetDateTime("UploadedAt");
                            model.UploadedBy = reader.GetValue("UploadedBy") == DBNull.Value ? (int?)null : reader.GetInt32("UploadedBy");
                            model.FileName = reader.GetValue("FileName") == DBNull.Value ? "" : reader.GetString("FileName");
                            model.FilePath = reader.GetValue("FilePath") == DBNull.Value ? "" : reader.GetString("FilePath");
                            model.Description = reader.GetValue("Description") == DBNull.Value ? "" : reader.GetString("Description");
                            list.Add(model);
                        }
                    }
                    conn.Close();
                }
            }
            return list;
        }

        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4, GetByPageSlug = 5
        }

    }
}
