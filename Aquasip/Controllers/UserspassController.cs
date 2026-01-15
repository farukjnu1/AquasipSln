using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Aquasip.Models;
using Aquasip.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Aquasip.Fiters;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class UserspassController : Controller
    {
        private readonly ILogger<UserspassController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public UserspassController(ILogger<UserspassController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        public IActionResult PasswordChange()
        {
            var listUser = new List<UserVM>();
            try
            {
                UserRepository userRepo = new UserRepository(_connectionString);
                int? UserId = HttpContext.Session.GetInt32("UserID");
                if (UserId != null)
                {
                    var oUser = userRepo.GetById((int)UserId);
                    if (oUser != null)
                    {
                        listUser.Add(oUser);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(listUser);
        }

        public IActionResult Edit(int id, UserVM.QueryType queryType)
        {
            var model = new UserVM();
            try
            {
                UserRepository userRepo = new UserRepository(_connectionString);
                model = userRepo.GetById(id);
                if (model != null)
                {
                    RoleRepository roleRepo = new RoleRepository(_connectionString);
                    var listRole = roleRepo.GetAll();
                    List<SelectListItem> selectRoles = new List<SelectListItem>();
                    foreach (var role in listRole)
                    {
                        selectRoles.Add(new SelectListItem { Text = role.RoleName, Value = role.RoleId.ToString() });
                    }
                    model.RoleOptions = selectRoles;
                    model.QueryTypes = queryType;
                }
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult EditPassword(UserVM model)
        {
            try
            {
                model.CreateBy = HttpContext.Session.GetInt32("UserID");
                UserRepository userRepo = new UserRepository(_connectionString);
                TempData["message"] = userRepo.UpdatePassword(model);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("PasswordChange");
        }


    }
}
