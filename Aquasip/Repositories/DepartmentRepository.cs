using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Aquasip.EF;
using Aquasip.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Aquasip.Repositories
{
    public class DepartmentRepository
    {
        // Read all
        public List<DepartmentVM> GetAll()
        {
            var list = new List<DepartmentVM>();

            using (var _context = new AquasipContext())
            {
                list = (from x in _context.Departments
                        select new DepartmentVM
                        {
                            Code = x.Code,
                            DepartmentId = x.DepartmentId,
                            Description = x.Description,
                            IsActive = x.IsActive,
                            Name = x.Name,
                        }).ToList();
            }
            return list;
        }

        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4
        }

    }

}
