using Microsoft.Data.SqlClient;
using Aquasip.Models;
using System.Data;
using Aquasip.EF;
using Microsoft.EntityFrameworkCore;

namespace Aquasip.Repositories
{
    public class MediaRepository
    {
        private readonly string _connectionString = "";
        public MediaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // Create
        public MediaVM Add(MediaVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                Medium oMedium = new Medium();
                //oMedium.Description = model.Description;
                oMedium.FileName = model.FileName;
                oMedium.FilePath = model.FilePath;
                oMedium.UploadedAt = DateTime.Now;
                oMedium.UploadedBy = model.UploadedBy;
                _context.Media.Add(oMedium);
                _context.SaveChanges();
                model.MediaId = oMedium.MediaId;
                message = "media has been added successfully.";
            }
            return model;
        }

        // Update
        public string Update(MediaVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                var oMedium = (from x in _context.Media where x.MediaId == model.MediaId select x).FirstOrDefault();
                if (oMedium != null)
                {
                    //oMedium.Description = model.Description;
                    oMedium.FileName = model.FileName;
                    oMedium.FilePath = model.FilePath;
                    oMedium.UploadedAt = DateTime.Now;
                    oMedium.UploadedBy = model.UploadedBy;
                    _context.SaveChanges();
                    message = "media has been updated successfully.";
                }
            }
            return message;
        }

        // Delete
        public string Delete(int id)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                var oMedium = (from x in _context.Media where x.MediaId == id select x).FirstOrDefault();
                if (oMedium != null)
                {
                    _context.Media.Remove(oMedium);

                    _context.SaveChanges();

                    message = "media has been removed successfully.";
                }
            }
            return message;
        }

        // Read all
        public List<MediaVM> GetAll()
        {
            var list = new List<MediaVM>();
            using (var _context = new AquasipContext())
            {
                list = (from x in _context.Media
                        select new MediaVM
                        {
                            //Description = x.Description,
                            FileName = x.FileName, 
                            FilePath = x.FilePath,
                            UploadedAt = x.UploadedAt,
                            UploadedBy = x.UploadedBy,
                        }).ToList();
            }
            return list;
        }

        // Read one
        public MediaVM? GetById(int id)
        {
            MediaVM? model = null;
            using (var _context = new AquasipContext())
            {
                model = (from x in _context.Media
                         where x.MediaId == id
                         select new MediaVM
                         {
                             Description = x.Description,
                             FileName = x.FileName,
                             FilePath = x.FilePath,
                             UploadedAt = x.UploadedAt,
                             UploadedBy = x.UploadedBy,
                         }).FirstOrDefault();
            }
            return model;
        }

    }
}
