using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Aquasip.EF;
using Aquasip.Models;
using Aquasip.Repositories;

namespace Aquasip.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserVM model)
        {
            try
            {
                UserRepository userRepo = new UserRepository(_connectionString);
                UserVM oUser = userRepo.Login(model);
                if (oUser != null) 
                {
                    if (oUser.IsActive == true)
                    {
                        HttpContext.Session.SetInt32("UserID", oUser.UserID);
                        HttpContext.Session.SetString("Username", oUser.Username);
                        return RedirectToAction("Index", "Pages");
                    }
                    else
                    {
                        TempData["message"] = "User not valid.";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index");
        }

        public IActionResult Logout(int? UserID)
        {
            HttpContext.Session.Remove("UserID");
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Index");
        }

    }
}
