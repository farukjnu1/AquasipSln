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
    public class ProductsController : Controller
    {
        //private readonly AquasipContext _context;

        //public ProductsController(AquasipContext context)
        //{
        //    _context = context;
        //}

        private readonly ILogger<PagesController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public ProductsController(ILogger<PagesController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            AquasipContext _context = new AquasipContext();
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AquasipContext _context = new AquasipContext();
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var oProduct = new Models.ProductVM();
            oProduct.ListProductMedia = (from x in _context.ProductMedia
                                         where x.ProductId == product.ProductId
                                         select new ProductMediumVM()
                                         {
                                             Description = x.Description,
                                             FileName = x.FileName,
                                             FilePath = x.FilePath,
                                             IsActive = x.IsActive,
                                             ProductId = x.ProductId,
                                             ProductMediaId = x.ProductMediaId,
                                             UploadedAt = x.UploadedAt,
                                             UploadedBy = x.UploadedBy
                                         }).OrderByDescending(y => y.UploadedAt).ToList();
            oProduct.UploadedBy = product.UploadedBy;
            oProduct.UploadedAt = product.UploadedAt;
            oProduct.Description = product.Description;
            oProduct.Price = product.Price;
            oProduct.ProductName = product.ProductName;
            oProduct.ProductId = product.ProductId;
            oProduct.ProductCode = product.ProductCode;
            oProduct.IsActive = product.IsActive;
            return View(oProduct);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View(new ProductVM());
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductCode,ProductName,Description,Price,IsActive,UploadedBy,UploadedAt")] Aquasip.EF.Product product)
        {
            if (ModelState.IsValid)
            {
                int? UploadedBy = HttpContext.Session.GetInt32("UserID");

                AquasipContext _context = new AquasipContext();
                _context.Add(product);
                await _context.SaveChangesAsync();

                if (product.Price != null)
                {
                    var oProductPrice = new ProductPrice();
                    oProductPrice.ProductId = product.ProductId;
                    oProductPrice.Price = product.Price;
                    oProductPrice.Description = product.Description;
                    oProductPrice.IsActive = true;
                    oProductPrice.UploadedAt = DateTime.Now;
                    oProductPrice.UploadedBy = UploadedBy;

                    _context.ProductPrices.Add(oProductPrice);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details([Bind("ProductId,ProductCode,ProductName,Description,Price,IsActive,UploadedBy,UploadedAt,MediaFile")] ProductVM model)
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
                //model.MediaId = mediaVm.MediaId;
                //TempData["message"] = pcRepo.Update(model);
                AquasipContext _context = new AquasipContext();
                ProductMedium productMedium = new ProductMedium
                {
                    ProductId = model.ProductId,
                    Description = model.Description,
                    FileName = uniqueFileName,
                    FilePath = "/img/" + uniqueFileName,
                    UploadedBy = UploadedBy,
                    UploadedAt = DateTime.Now,
                    IsActive = true
                };
                _context.Add(productMedium);
                await _context.SaveChangesAsync();
                #endregion
            }
            #endregion

            return RedirectToAction(nameof(Details), new { id = model.ProductId });
        }

        public async Task<IActionResult> DeleteMedia(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AquasipContext _context = new AquasipContext();
            var productMedia = await _context.ProductMedia
                .FirstOrDefaultAsync(m => m.ProductMediaId == id);
            if (productMedia == null)
            {
                return NotFound();
            }
            #region Delete Record
            _context.ProductMedia.Remove(productMedia);
            await _context.SaveChangesAsync();
            #endregion
            #region Delete File
            if (!string.IsNullOrEmpty(productMedia.FilePath))
            {
                string uploadPath = Path.Combine(_environment.WebRootPath, "img");
                string delFilePath = Path.Combine(uploadPath, productMedia.FileName);

                if (System.IO.File.Exists(delFilePath))
                {
                    System.IO.File.Delete(delFilePath);
                }
            }
            #endregion
            return RedirectToAction(nameof(Details), new { id = productMedia.ProductId });
            //return View(productMedia);
        }

        // GET: Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AquasipContext _context = new AquasipContext();
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ProductId,ProductCode,ProductName,Description,Price,IsActive")] EF.Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int? UploadedBy = HttpContext.Session.GetInt32("UserID");

                    AquasipContext _context = new AquasipContext();

                    var price = await (from x in _context.Products where x.ProductId == id select x.Price).FirstOrDefaultAsync();
                    if (price != null)
                    {
                        if (product.Price != price)
                        {
                            var oProductPrice = new ProductPrice();
                            oProductPrice.ProductId = id;
                            oProductPrice.Price = product.Price;
                            oProductPrice.Description = product.Description;
                            oProductPrice.IsActive = true;
                            oProductPrice.UploadedAt = DateTime.Now;
                            oProductPrice.UploadedBy = UploadedBy;

                            _context.ProductPrices.Add(oProductPrice);
                        }
                    }

                    _context.Update(product);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AquasipContext _context = new AquasipContext();
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
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
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool ProductExists(long id)
        {
            AquasipContext _context = new AquasipContext();
            return _context.Products.Any(e => e.ProductId == id);
        }

    }
}
