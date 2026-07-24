using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class GalleriesController : Controller
    {
        private readonly AquasipContext _context;

        /* public GalleriesController(AquasipContext context)
         {
             _context = context;
         }*/

        private readonly ILogger<GalleriesController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public GalleriesController(ILogger<GalleriesController> logger, IConfiguration configuration, IWebHostEnvironment environment, AquasipContext context)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Galleries.ToListAsync());
        }

        

        // GET: Products/Create
        public IActionResult Create()
        {
            return View(new GalleryVM());
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GalleryId,Code,Title,Header,Body,Footer,IsActive,UploadedBy,UploadedAt")] Aquasip.EF.Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                int? UploadedBy = HttpContext.Session.GetInt32("UserID");
                gallery.UploadedBy = UploadedBy;
                _context.Add(gallery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gallery);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var gallery = await _context.Galleries.FirstOrDefaultAsync(m => m.GalleryId == id);
            if (gallery == null)
            {
                return NotFound();
            }
            var oGallery = new Models.GalleryVM();
            oGallery.GalleryMedia = (from x in _context.GalleryMedia
                                     where x.GalleryId == gallery.GalleryId
                                     select new GalleryMediumVM()
                                     {
                                         Description = x.Description,
                                         FileName = x.FileName,
                                         FilePath = x.FilePath,
                                         IsActive = x.IsActive,
                                         GalleryId = x.GalleryId,
                                         MediaId = x.MediaId,
                                         UploadedAt = x.UploadedAt,
                                         UploadedBy = x.UploadedBy
                                     }).OrderByDescending(y => y.UploadedAt).ToList();
            oGallery.UploadedBy = gallery.UploadedBy;
            oGallery.UploadedAt = gallery.UploadedAt;
            oGallery.GalleryId = gallery.GalleryId;
            oGallery.Code = gallery.Code;
            oGallery.Title = gallery.Title;
            oGallery.Header = gallery.Header;
            oGallery.Body = gallery.Body;
            oGallery.Footer = gallery.Footer;
            oGallery.IsActive = gallery.IsActive;
            return View(oGallery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("GalleryId,Code,Title,Header,Body,Footer,IsActive,UploadedBy,UploadedAt,MediaFile")] GalleryVM model)
        {
            int? UploadedBy = HttpContext.Session.GetInt32("UserID");
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
                DateTime currentDateTime = DateTime.Now;
                string timeStamp = currentDateTime.ToString("yyyyMMdd") + "_" + currentDateTime.ToString("HHmmss") + "_" + currentDateTime.ToString("fff");
                string uniqueFileName = $"{timeStamp}{extension}";
                var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uniqueFileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.MediaFile.CopyTo(stream);
                }
                #endregion
                #region Media Update
                GalleryMedium galleryMedium = new GalleryMedium
                {
                    GalleryId = model.GalleryId,
                    Description = model.Title,
                    FileName = uniqueFileName,
                    FilePath = "/img/" + uniqueFileName,
                    UploadedBy = UploadedBy,
                    UploadedAt = DateTime.Now,
                    IsActive = true
                };
                _context.Add(galleryMedium);
                await _context.SaveChangesAsync();
                #endregion
            }
            #endregion
            return RedirectToAction(nameof(Details), new { id = model.GalleryId });
        }

        public async Task<IActionResult> DeleteMedia(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var galleryMedia = await _context.GalleryMedia
                .FirstOrDefaultAsync(m => m.MediaId == id);
            if (galleryMedia == null)
            {
                return NotFound();
            }
            #region Delete Record
            _context.GalleryMedia.Remove(galleryMedia);
            await _context.SaveChangesAsync();
            #endregion
            #region Delete File
            if (!string.IsNullOrEmpty(galleryMedia.FilePath))
            {
                string uploadPath = Path.Combine(_environment.WebRootPath, "img");
                string delFilePath = Path.Combine(uploadPath, galleryMedia.FileName);

                if (System.IO.File.Exists(delFilePath))
                {
                    System.IO.File.Delete(delFilePath);
                }
            }
            #endregion
            return RedirectToAction(nameof(Details), new { id = galleryMedia.MediaId });
        }

        // GET: Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var gallery = await _context.Galleries.Where(x=>x.GalleryId == id).FirstOrDefaultAsync();
            if (gallery == null)
            {
                return NotFound();
            }
            return View(gallery);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("GalleryId,Code,Title,Header,Body,Footer,IsActive,UploadedBy,UploadedAt")] Gallery gallery)
        {
            if (id != gallery.GalleryId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    int? UploadedBy = HttpContext.Session.GetInt32("UserID");
                    gallery.UploadedBy = UploadedBy;
                    _context.Update(gallery);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!GalleryExists(gallery.GalleryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gallery);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Galleries
                .FirstOrDefaultAsync(m=> m.GalleryId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            AquasipContext _context = new AquasipContext();
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery != null)
            {
                _context.Galleries.Remove(gallery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool GalleryExists(long id)
        {
            return _context.Galleries.Any(e=> e.GalleryId == id);
        }

    }
}
