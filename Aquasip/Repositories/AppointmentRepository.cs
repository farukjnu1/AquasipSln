using System;
using System.Collections.Generic;
using System.Data;
using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Aquasip.EF;
using Aquasip.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Aquasip.Repositories
{
    public class AppointmentRepository
    {
        // Create
        public string Add(AppointmentVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                Appointment oAppointment = new Appointment();
                oAppointment.Address = model.Address;
                oAppointment.AppointmentDate = model.AppointmentDate;
                oAppointment.Age = model.Age;
                oAppointment.CreateAt = DateTime.Now;
                oAppointment.Email = model.Email;
                oAppointment.FullName = model.FullName;
                oAppointment.Gender = model.Gender;
                oAppointment.IsActive = true;
                oAppointment.Phone = model.Phone;
                oAppointment.ProblemDetail = model.ProblemDetail;
                oAppointment.SpecialistId = model.SpecialistId;
                oAppointment.DepartmentId = model.DepartmentId;

                _context.Appointments.Add(oAppointment);

                _context.SaveChanges();

                message = "message has been sent successfully.";
            }

            return message;
        }

        // Update
        public string Update(AppointmentVM model)
        {
            string message = "operation failed.";
            using (var _context = new AquasipContext())
            {
                Appointment? oAppointment = (from x in _context.Appointments where x.AppointmentId == model.AppointmentId select x).FirstOrDefault();
                if (oAppointment != null)
                {
                    oAppointment.Address = model.Address;
                    oAppointment.AppointmentDate = model.AppointmentDate;
                    oAppointment.Age = model.Age;
                    oAppointment.CreateAt = DateTime.Now;
                    oAppointment.Email = model.Email;
                    oAppointment.FullName = model.FullName;
                    oAppointment.Gender = model.Gender;
                    oAppointment.IsActive = model.IsActive;
                    oAppointment.Phone = model.Phone;
                    oAppointment.ProblemDetail = model.ProblemDetail;
                    oAppointment.SpecialistId = model.SpecialistId;
                    oAppointment.DepartmentId = model.DepartmentId;
                    oAppointment.IsRead = model.IsRead;
                    oAppointment.ReadBy = model.ReadBy;
                    //oAppointment.CreateBy = model.CreateBy;

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
                Appointment? oAppointment = (from x in _context.Appointments where x.AppointmentId == id select x).FirstOrDefault();
                if (oAppointment != null) 
                {
                    _context.Appointments.Remove(oAppointment);

                    _context.SaveChanges();

                    message = "data has been removed successfully.";
                }
            }
            return message;
        }

        // Read all
        public async Task<List<Appointment>> GetAll(int pageNumber = 1)
        {
            var list = new List<Appointment>();
            using (var _context = new AquasipContext())
            {
                int pageSize = 10;
                var listAppointment = _context.Appointments.AsNoTracking();
                list = await PaginatedList<Appointment>.CreateAsync(listAppointment, pageNumber, pageSize);
                //list = await _context.Appointments.ToListAsync();
            }
            return list;
        }

        // Read one
        public AppointmentVM? GetById(int id)
        {
            AppointmentVM? model = null;
            using (var _context = new AquasipContext())
            {
                model = (from x in _context.Appointments
                         where x.AppointmentId == id
                         select new AppointmentVM
                         {
                             Address = x.Address,
                             Age = x.Age,
                             AppointmentDate = x.AppointmentDate,
                             AppointmentId = x.AppointmentId,
                             CreateAt = x.CreateAt,
                             Email = x.Email,
                             FullName = x.FullName,
                             Gender = x.Gender,
                             IsActive = x.IsActive,
                             Phone = x.Phone,
                             ProblemDetail = x.ProblemDetail,
                             SpecialistId = x.SpecialistId
                         }).FirstOrDefault();
            }
            return model;
        }

        public string MarkAsReadOrUnread(int id, int? CreateBy)
        {
            string message = "operation failed.";
            var list = new List<Appointment>();
            using (var _context = new AquasipContext())
            {
                var oAppointment = _context.Appointments.Where(x => x.AppointmentId == id).FirstOrDefault();
                if (oAppointment != null)
                {
                    //oAppointment.CreateBy = CreateBy;
                    oAppointment.ReadBy = CreateBy;
                    oAppointment.IsRead = oAppointment.IsRead == true ? false : true;
                    _context.SaveChanges();
                    message = oAppointment.IsRead == true ? "Marked as read successfully." : "Marked as unread successfully.";
                }
            }
            return message;
        }

        public string SuspendOrRestore(int id, int? CreateBy)
        {
            string message = "operation failed.";
            var list = new List<Appointment>();
            using (var _context = new AquasipContext())
            {
                var oAppointment = _context.Appointments.Where(x => x.AppointmentId == id).FirstOrDefault();
                if (oAppointment != null)
                {
                    oAppointment.CreateBy = CreateBy;
                    oAppointment.IsActive = oAppointment.IsActive == true ? false : true;
                    _context.SaveChanges();
                    message = oAppointment.IsActive == true ? "Message has been restored." : "Message has been suspended.";
                }
            }
            return message;
        }

    }

}
