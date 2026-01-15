using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class PageContentsController : Controller
    {
        private readonly ILogger<PageContentsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public PageContentsController(ILogger<PageContentsController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        // GET: PageContentsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PageContentsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PageContentsController/Create
        public ActionResult Create(string slug, string slugPageContent)
        {
            PageContentVM oPageContent = new PageContentVM();
            oPageContent.SlugPage = slug;
            oPageContent.SlugPageContent = slugPageContent;
            return View(oPageContent);
        }

        // POST: PageContentsController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        // POST: PageContentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PageContentVM model) //IFormCollection collection)
        {
            try
            {
                #region PageContent Update
                int? UploadedBy = HttpContext.Session.GetInt32("UserID");
                PageContentRepository pcRepo = new PageContentRepository(_connectionString);
                model.UploadedBy = UploadedBy;
                //TempData["message"] = pcRepo.Add(model);
                var pageContentVM = pcRepo.Add(model);
                model.PageContentId = pageContentVM.PageContentId;
                #endregion
                #region Media
                if (model.MediaFile != null && model.MediaFile.Length > 0)
                {
                    #region Create File
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "img");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string extension = Path.GetExtension(model.MediaFile.FileName);
                    //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // to milliseconds
                    DateTime currentDateTime = DateTime.Now;
                    string timeStamp = currentDateTime.ToString("yyyyMMdd") + "_" + currentDateTime.ToString("HHmmss") + "_" + currentDateTime.ToString("fff");
                    string uniqueFileName = $"{timeStamp}{extension}";

                    var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uniqueFileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.MediaFile.CopyTo(stream);
                    }
                    #endregion
                    #region Delete File
                    if (!string.IsNullOrEmpty(model.FilePath))
                    {
                        string uploadPath = Path.Combine(_environment.WebRootPath, "img");
                        string delFilePath = Path.Combine(uploadPath, model.FileName);

                        if (System.IO.File.Exists(delFilePath))
                        {
                            System.IO.File.Delete(delFilePath);
                        }
                    }
                    #endregion
                    #region Media Update
                    MediaRepository mediaRepo = new MediaRepository(_connectionString);
                    MediaVM oMedia = new MediaVM();
                    oMedia.Description = model.Description;
                    oMedia.FileName = uniqueFileName;
                    oMedia.FilePath = "/img/" + uniqueFileName;
                    oMedia.MediaId = Convert.ToInt32(model.MediaId);
                    oMedia.UploadedBy = UploadedBy;
                    var mediaVm = mediaRepo.Add(oMedia);
                    model.MediaId = mediaVm.MediaId;
                    TempData["message"] = pcRepo.Update(model);
                    #endregion
                }
                #endregion
                return RedirectToAction("Details", "Pages", new { slug = model.SlugPage });
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
                return View();
            }
        }

        // GET: PageContentsController/Edit/5
        public ActionResult Edit(int id)
        {
            PageContentVM oPageContent = new PageContentVM();
            try
            {
                PageContentRepository pcRepo = new PageContentRepository(_connectionString);
                oPageContent = pcRepo.GetById(id);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return View(oPageContent);
        }

        // POST: PageContentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PageContentVM model) //IFormCollection collection)
        {
            try
            {
                #region PageContent Update
                int? UploadedBy = HttpContext.Session.GetInt32("UserID");
                PageContentRepository pcRepo = new PageContentRepository(_connectionString);
                model.UploadedBy = UploadedBy;
                TempData["message"] = pcRepo.Update(model);
                #endregion
                #region Media
                if (model.MediaFile != null && model.MediaFile.Length > 0)
                {
                    #region Create File
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "img");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string extension = Path.GetExtension(model.MediaFile.FileName);
                    //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // to milliseconds
                    DateTime currentDateTime = DateTime.Now;
                    string timeStamp = currentDateTime.ToString("yyyyMMdd") + "_" + currentDateTime.ToString("HHmmss") + "_" + currentDateTime.ToString("fff");
                    string uniqueFileName = $"{timeStamp}{extension}";

                    var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uniqueFileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.MediaFile.CopyTo(stream);
                    }
                    #endregion
                    #region Delete File
                    if (!string.IsNullOrEmpty(model.FilePath))
                    {
                        string uploadPath = Path.Combine(_environment.WebRootPath, "img");
                        string delFilePath = Path.Combine(uploadPath, model.FileName);

                        if (System.IO.File.Exists(delFilePath))
                        {
                            System.IO.File.Delete(delFilePath);
                        }
                    }
                    #endregion
                    #region Media Update
                    MediaRepository mediaRepo = new MediaRepository(_connectionString);
                    MediaVM oMedia = new MediaVM();
                    oMedia.Description = model.Description;
                    oMedia.FileName = uniqueFileName;
                    oMedia.FilePath = "/img/"+ uniqueFileName;
                    oMedia.MediaId = Convert.ToInt32(model.MediaId);
                    oMedia.UploadedBy = UploadedBy;
                    var mediaVm = mediaRepo.Add(oMedia);
                    model.MediaId = mediaVm.MediaId;
                    TempData["message"] = pcRepo.Update(model);
                    #endregion
                }
                #endregion
                return RedirectToAction("Details", "Pages", new { slug = model.SlugPage });
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
                return View();
            }
        }

        // GET: PageContentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PageContentsController/Delete/5
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
