using Microsoft.Data.SqlClient;
using System.Data;
using Aquasip.EF;
using Aquasip.Models;

namespace Aquasip.Repositories
{
    public class RoleRepository
    {
        private readonly string _connectionString = "";
        public RoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Read all
        public List<RoleVM> GetAll()
        {
            var list = new List<RoleVM>();
            using (var _context = new AquasipContext())
            {
                list = (from x in _context.Roles
                        where x.IsActive == true
                        select new RoleVM
                        {
                            RoleId = x.RoleId,
                            RoleName = x.RoleName,
                            Description = x.Description
                        }).OrderBy(x => x.RoleId).ToList();
            }
            return list;
        }

        public bool SetUserRole(UserVM model)
        {
            bool isSave = false;
            using (var _context = new AquasipContext())
            {
                var oUserRole = (from ur in _context.UserRoles where ur.UserId == model.UserID select ur).FirstOrDefault();
                if (oUserRole == null)
                {
                    oUserRole = new UserRole();
                    oUserRole.UserId = model.UserID;
                    oUserRole.RoleId = model.RoleId;
                    _context.UserRoles.Add(oUserRole);
                    _context.SaveChanges();
                }
                else
                {
                    oUserRole.RoleId = model.RoleId;
                    _context.SaveChanges();
                }
                isSave = true;
            }
            return isSave;
        }

        // Read all
        public List<RolePermissionVM> GetPermissionByRole(int roleId, string controller)
        {
            var list = new List<RolePermissionVM>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Role_Read", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@RoleId", roleId);
                        cmd.Parameters.AddWithValue("@Controller", controller);
                        cmd.Parameters.AddWithValue("@QueryType", RoleVM.QueryType.GetPermissionByRole);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RolePermissionVM model = new RolePermissionVM();
                                model.Controller = reader.GetValue("Controller") == DBNull.Value ? string.Empty : reader.GetString("Controller");
                                model.IsActive = reader.GetValue("IsActive") == DBNull.Value ? false : reader.GetBoolean("IsActive");
                                model.RoleId = reader.GetValue("RoleId") == DBNull.Value ? 0 : reader.GetInt32("RoleId");
                                model.RolePermissionId = reader.GetValue("RolePermissionId") == DBNull.Value ? 0 : reader.GetInt32("RolePermissionId");

                                list.Add(model);
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch { }
            return list;
        }


    }
}
