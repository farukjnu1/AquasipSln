using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public PagesController(ILogger<PagesController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        // GET: PagesController
        public ActionResult Index()
        {
            var listPage = new List<PageVM>();
            try
            {
                PageRepository pageRepo = new PageRepository(_connectionString);
                listPage = pageRepo.GetAll();
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(listPage);
        }

        // GET: PagesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PagesController/Edit/5
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

        //[Route("Pages/Details/{slug}")]
        public ActionResult Details(string slug)
        {
            var listPage = new List<PageVM>();
            try
            {
                PageRepository pRepo = new PageRepository(_connectionString);
                PageContentRepository pcRepo = new PageContentRepository(_connectionString);

                var oPage = pRepo.GetBySlug(slug);
                oPage.PageContents = pcRepo.GetBySlugPage(slug);

                listPage = new List<PageVM>();
                listPage.Add(oPage);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(listPage);
        }

    }
}
