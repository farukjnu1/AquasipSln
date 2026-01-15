using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public AppointmentsController(ILogger<AppointmentsController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        // GET: AppointmentsController
        public async Task<ActionResult> Index(int pageNumber = 1)
        {
            List<Appointment> listAppointment = new List<Appointment>();
            try
            {
                AppointmentRepository appointmentRepo = new AppointmentRepository();
                listAppointment = await appointmentRepo.GetAll(pageNumber);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(listAppointment);
        }

        // GET: AppointmentsController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                int? CreateBy = HttpContext.Session.GetInt32("UserID");
                AppointmentRepository appointmentRepo = new AppointmentRepository();
                TempData["message"] = appointmentRepo.MarkAsReadOrUnread(id, CreateBy);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Index");
        }

        // GET: AppointmentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppointmentsController/Create
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

        // GET: AppointmentsController/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                int? CreateBy = HttpContext.Session.GetInt32("UserID");
                AppointmentRepository appointmentRepo = new AppointmentRepository();
                TempData["message"] = appointmentRepo.SuspendOrRestore(id, CreateBy);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Index");
        }

        // POST: AppointmentsController/Edit/5
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

        // GET: AppointmentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AppointmentsController/Delete/5
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
