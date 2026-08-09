using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class ReviewsController : Controller
    {
        private readonly AquasipContext _context;
        private readonly ILogger<ReviewsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public ReviewsController(ILogger<ReviewsController> logger, IConfiguration configuration, IWebHostEnvironment environment, AquasipContext context)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.Reviews.Include(r => r.Customer).Include(r => r.Product);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }
            review.ReviewMedia = _context.ReviewMedia.Where(x => x.ReviewId == id).ToList();
            var oReview = new ReviewVM()
            {
                CreatedAt = review.CreatedAt,
                CustomerId = review.CustomerId,
                IsApproved = review.IsApproved,
                IsDeleted = review.IsDeleted,
                ModerationStatus = review.ModerationStatus,
                ProductId = review.ProductId,
                Rating = review.Rating,
                ReviewId = review.ReviewId,
                ReviewText = review.ReviewText,
                Title = review.Title
            };
            oReview.ReviewMedia = (from x in _context.ReviewMedia
                                   where x.ReviewId == id
                                   select new ReviewMediumVM
                                   {
                                       CreatedAt = x.CreatedAt,
                                       MediaId = x.MediaId,
                                       MediaType = x.MediaType,
                                       ReviewId = x.ReviewId,
                                       MediaUrl = x.MediaUrl
                                   }).ToList();
            return View(oReview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ReviewVM model)
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
                ReviewMedium oMedia = new ReviewMedium
                {
                    ReviewId = model.ReviewId,
                    MediaUrl = "/img/" + uniqueFileName,
                    CreatedAt = DateTime.Now
                };
                _context.Add(oMedia);
                await _context.SaveChangesAsync();
                #endregion
            }
            #endregion
            return RedirectToAction(nameof(Details), new { id = model.ReviewId });
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            #region dropdown list
            ViewData["CustomerId"] = _context.Customers.Where(x => x.IsActive == true).OrderBy(x => x.FullName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CustomerId.ToString(),
                        Text = x.FullName
                    }).ToList();
            ViewData["ProductId"] = _context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ProductId.ToString(),
                        Text = x.ProductName
                    }).ToList();
            #endregion
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,ProductId,CustomerId,Title,ReviewText,Rating,IsApproved,CreatedAt,IsDeleted,ModerationStatus,Attachments")] ReviewVM review)
        {
            if(review.ProductId == 0)
            {
                TempData["message"] = "Product is required";
                return RedirectToAction("Review", "Home");
            }
            if (review.CustomerId == 0)
            {
                TempData["message"] = "Please, Sign-in to review a product.";
                return RedirectToAction("Review", "Home");
            }
            #region Review
            Review oReview = new Review
            {
                ProductId = review.ProductId,
                CustomerId = review.CustomerId,
                Title = review.Title,
                ReviewText = review.ReviewText,
                Rating = review.Rating,
                IsApproved = review.IsApproved,
                CreatedAt = DateTime.Now,
                IsDeleted = review.IsDeleted,
                ModerationStatus = review.ModerationStatus
            };
            _context.Add(oReview);
            await _context.SaveChangesAsync();
            #endregion
            #region Media
            foreach (var attch in review.Attachments)
            {
                if (attch != null && attch.Length > 0)
                {
                    #region Create File
                    if (!FileValidation.IsValidFileForReview(attch.FileName))
                    {
                        TempData["message"] = "Media file not supported.";
                        return RedirectToAction("Review", "Home");
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "img");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string extension = Path.GetExtension(attch.FileName);
                    DateTime currentDateTime = DateTime.Now;
                    string timeStamp = currentDateTime.ToString("yyyyMMdd") + "_" + currentDateTime.ToString("HHmmss") + "_" + currentDateTime.ToString("fff");
                    string uniqueFileName = $"{timeStamp}{extension}";

                    var filePath = Path.Combine(uploadsFolder, Path.GetFileName(uniqueFileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        attch.CopyTo(stream);
                    }
                    #endregion
                    #region Media Update
                    ReviewMedium medium = new ReviewMedium
                    {
                        ReviewId = oReview.ReviewId,
                        CreatedAt = DateTime.Now,
                        MediaType = extension,
                        MediaUrl = "/img/" + uniqueFileName,
                        
                    };
                    _context.Add(medium);
                    await _context.SaveChangesAsync();
                    #endregion
                }
            }
            #endregion
            return RedirectToAction("Review", "Home");
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var review = await _context.Reviews
                .Where(x => x.ReviewId == id)
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync();
            if (review == null)
            {
                return NotFound();
            }
            #region dropdown list
            ViewData["CustomerId"] = _context.Customers.Where(x => x.IsActive == true).OrderBy(x => x.FullName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CustomerId.ToString(),
                        Text = x.FullName
                    }).ToList();
            ViewData["ProductId"] = _context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ProductId.ToString(),
                        Text = x.ProductName
                    }).ToList();
            #endregion
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReviewId,ProductId,CustomerId,Title,ReviewText,Rating,IsApproved,CreatedAt,IsDeleted,ModerationStatus")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }
            try
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(review.ReviewId))
                {
                    return NotFound();
                }
                else
                {
                    return RedirectToAction(nameof(Edit), new { id = review.ReviewId });
                    //throw;
                }
            }
            #region dropdown list
            ViewData["CustomerId"] = _context.Customers.Where(x => x.IsActive == true).OrderBy(x => x.FullName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.CustomerId.ToString(),
                        Text = x.FullName
                    }).ToList();
            ViewData["ProductId"] = _context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ProductId.ToString(),
                        Text = x.ProductName
                    }).ToList();
            #endregion
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteMedia(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var oMedia = await _context.ReviewMedia.FirstOrDefaultAsync(m => m.MediaId == id);
            if (oMedia == null)
            {
                return NotFound();
            }
            string fileName = FileValidation.GetFileNameFromURL(oMedia.MediaUrl);
            #region Delete Record
            _context.ReviewMedia.Remove(oMedia);
            await _context.SaveChangesAsync();
            #endregion
            #region Delete File
            if (!string.IsNullOrEmpty(oMedia.MediaUrl))
            {
                string uploadPath = Path.Combine(_environment.WebRootPath, "img");
                string delFilePath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(delFilePath))
                {
                    System.IO.File.Delete(delFilePath);
                }
            }
            #endregion
            return RedirectToAction(nameof(Details), new { id = oMedia.ReviewId });
        }

        private bool ReviewExists(long id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
    }
}
