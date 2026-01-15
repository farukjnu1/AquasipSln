using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class ContactsController : Controller
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public ContactsController(ILogger<ContactsController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        // GET: ContactsController
        public async Task<ActionResult> Index(int pageNumber = 1)
        {
            List<ContactMessage> listContact = new List<ContactMessage>();
            try 
            {
                ContactMessageRepository contactRepo = new ContactMessageRepository();
                listContact = await contactRepo.GetAll(pageNumber);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(listContact);
        }

        // GET: ContactsController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                int? CreateBy = HttpContext.Session.GetInt32("UserID");
                ContactMessageRepository contactRepo = new ContactMessageRepository();
                TempData["message"] = contactRepo.MarkAsReadOrUnread(id, CreateBy);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Index");
        }

        // GET: ContactsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactsController/Create
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

        // GET: ContactsController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                int? CreateBy = HttpContext.Session.GetInt32("UserID");
                ContactMessageRepository contactRepo = new ContactMessageRepository();
                TempData["message"] = contactRepo.SuspendOrRestore(id, CreateBy);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Index");
        }

        // POST: ContactsController/Edit/5
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

        // GET: ContactsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ContactsController/Delete/5
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
