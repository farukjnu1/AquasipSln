using Aquasip.Fiters;
using Aquasip.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class SiteSettingController : Controller
    {
        private readonly ILogger<SiteSettingController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public SiteSettingController(ILogger<SiteSettingController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        // GET: SiteSettingController
        public ActionResult Index()
        {
            var siteSettingRepo = new SiteSettingRepository(_connectionString);
            var listSiteSetting = siteSettingRepo.GetAll();
            return View(listSiteSetting);
        }

        // GET: SiteSettingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SiteSettingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SiteSettingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SiteSettingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SiteSettingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SiteSettingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SiteSettingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
