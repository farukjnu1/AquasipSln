using Microsoft.EntityFrameworkCore;
using Aquasip.EF;
using Aquasip.Models;

namespace Aquasip.Repositories
{
    public class ContactMessageRepository
    {
        // Create
        public string Add(ContactMessageVM model, out bool isSave, out string code)
        {
            string message = "operation failed."; isSave = false; code = string.Empty;
            try
            {
                using (var _context = new AquasipContext())
                {
                    var currentDate = DateTime.Now;
                    ContactMessage oContactMessage = new ContactMessage();
                    oContactMessage.Code = Utilities.CodeGenerate.ContactMessageNum(currentDate);
                    oContactMessage.CreateAt = currentDate;
                    oContactMessage.Email = model.Email;
                    oContactMessage.FullName = model.FullName;
                    oContactMessage.IsActive = true;
                    oContactMessage.Message = model.Message;
                    oContactMessage.Phone = model.Phone;
                    oContactMessage.Subject = model.Subject;
                    _context.ContactMessages.Add(oContactMessage);
                    _context.SaveChanges();
                    message = "message has been sent successfully.";
                    isSave = true;
                    code = oContactMessage.Code;
                }
            }
            catch 
            {
                isSave = false;
            }
            return message;
        }

        // Update
        public string Update(ContactMessageVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                ContactMessage? oContactMessage = (from x in _context.ContactMessages where x.ContactMessageId == model.ContactMessageId select x).FirstOrDefault();
                if (oContactMessage != null)
                {
                    oContactMessage.CreateAt = DateTime.Now;
                    oContactMessage.Email = model.Email;
                    oContactMessage.FullName = model.FullName;
                    oContactMessage.IsActive = model.IsActive;
                    oContactMessage.Message = model.Message;
                    oContactMessage.Phone = model.Phone;
                    oContactMessage.Subject = model.Subject;
                    oContactMessage.IsRead = model.IsRead;
                    oContactMessage.ReadBy = model.ReadBy;
                    //oContactMessage.CreateBy = model.CreateBy;
                    _context.SaveChanges();

                    message = "data has been updated successfully.";
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
                ContactMessage? oContactMessage = (from x in _context.ContactMessages where x.ContactMessageId == id select x).FirstOrDefault();
                if (oContactMessage != null)
                {
                    _context.ContactMessages.Remove(oContactMessage);
                    _context.SaveChanges();
                    message = "data has been removed successfully.";
                }
            }
            return message;
        }

        // Read all
        public async Task<List<ContactMessage>> GetAll(int pageNumber = 0, int pageSize = 20)
        {
            var list = new List<ContactMessage>();
            using (var _context = new AquasipContext())
            {
                //int pageSize = 10;
                var listContact = _context.ContactMessages.OrderByDescending(x=>x.ContactMessageId).AsNoTracking();
                list = await PaginatedList<ContactMessage>.CreateAsync(listContact, pageNumber, pageSize);
            }
            return list;
        }

        // Read one
        public ContactMessageVM? GetById(int id)
        {
            ContactMessageVM? model = null;
            using (var _context = new AquasipContext())
            {
                model = (from x in _context.ContactMessages
                         where x.ContactMessageId == id
                         select new ContactMessageVM
                         {
                             ContactMessageId = x.ContactMessageId,
                             CreateAt = x.CreateAt,
                             Email = x.Email,
                             FullName = x.FullName,
                             IsActive = x.IsActive,
                             Message = x.Message,
                             Phone = x.Phone,
                             Subject = x.Subject,
                             IsRead = x.IsRead,
                             ReadBy = x.ReadBy
                         }).FirstOrDefault();
            }
            return model;
        }

        public string MarkAsReadOrUnread(int id, int? CreateBy)
        {
            string message = "operation failed.";
            var list = new List<ContactMessage>();
            using (var _context = new AquasipContext())
            {
                var oContactMessage = _context.ContactMessages.Where(x=>x.ContactMessageId == id).FirstOrDefault();
                if (oContactMessage != null)
                {
                    //oContactMessage.CreateBy = CreateBy;
                    oContactMessage.ReadBy = CreateBy;
                    oContactMessage.IsRead = oContactMessage.IsRead == true ? false : true;
                    _context.SaveChanges();
                    message = oContactMessage.IsRead == true ? "Marked as read successfully." : "Marked as unread successfully.";
                }
            }
            return message;
        }

        public string SuspendOrRestore(int id, int? CreateBy)
        {
            string message = "operation failed.";
            var list = new List<ContactMessage>();
            using (var _context = new AquasipContext())
            {
                var oContactMessage = _context.ContactMessages.Where(x => x.ContactMessageId == id).FirstOrDefault();
                if (oContactMessage != null)
                {
                    oContactMessage.CreateBy = CreateBy;
                    oContactMessage.IsActive = oContactMessage.IsActive == true ? false : true;
                    _context.SaveChanges();
                    message = oContactMessage.IsActive == true ? "Message has been restored." : "Message has been suspended.";
                }
            }
            return message;
        }

    }
}
