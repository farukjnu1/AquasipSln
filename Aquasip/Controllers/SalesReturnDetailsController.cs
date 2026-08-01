using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class SalesReturnDetailsController : Controller
    {
        private readonly AquasipContext _context;

        public SalesReturnDetailsController(AquasipContext context)
        {
            _context = context;
        }

        // GET: SalesReturnDetails
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.SalesReturnDetails.Include(p => p.Product).Include(p => p.SalesReturn);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: SalesReturnDetails/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesReturnDetail = await _context.SalesReturnDetails
                .Include(p => p.Product)
                .Include(p => p.SalesReturn)
                .FirstOrDefaultAsync(m => m.SalesReturnDetailId == id);
            if (salesReturnDetail == null)
            {
                return NotFound();
            }

            return View(salesReturnDetail);
        }

        // GET: SalesReturnDetails/Create
        public IActionResult Create(long salesReturnId)
        {
            var oSalesReturn = _context.SalesReturns.Where(x => x.SalesReturnId == salesReturnId).Include(i => i.Customer).FirstOrDefault();
            if (oSalesReturn != null)
            {
                var oSalesOrder = _context.SalesOrders.Where(x => x.OrderId == oSalesReturn.SalesOrderId).Include(i => i.Customer).FirstOrDefault();
                if (oSalesOrder != null)
                {
                    oSalesOrder.SalesOrderDetails = _context.SalesOrderDetails.Include(x => x.Product).Where(x => x.OrderId == oSalesOrder.OrderId && x.IsActive == true).ToList();
                }
                ViewData["SalesOrder"] = _context.SalesOrders.Where(x => x.OrderId == oSalesReturn.SalesOrderId).Include(i => i.Customer).FirstOrDefault();
            }
            ViewData["SalesReturn"] = oSalesReturn;
            #region dropdown list
            var listProducts = new List<SelectListItem>();
            listProducts.AddRange(_context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName).Select(x => new SelectListItem
            {
                Value = x.ProductId.ToString(),
                Text = x.ProductName
            })
                .ToList());
            ViewData["Products"] = listProducts;
            #endregion
            return View();
        }

        // POST: SalesReturnDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesReturnDetailId,SalesReturnId,ProductId,Qty,UnitPrice,LineTotal,StoreId,IsActive")] SalesReturnDetail salesReturnDetail)
        {
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    #region order-details
                    // =========================
                    // Save Order Details
                    // =========================
                    salesReturnDetail.IsActive = true;
                    _context.Add(salesReturnDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oSalesReturn = _context.SalesReturns.Where(x => x.SalesReturnId == salesReturnDetail.SalesReturnId).FirstOrDefault();
                    if (oSalesReturn != null) 
                    {
                        oSalesReturn.IsActive = true;
                        // =========================
                        // Save Order Header
                        // =========================
                        _context.SaveChanges();
                    }
                    #endregion
                    // =========================
                    // Commit Transaction
                    // =========================
                    transaction.Commit();
                    return RedirectToAction("Edit", "SalesReturns", new { id = salesReturnDetail.SalesReturnId });
                }
                catch (Exception ex)
                {
                    // =========================
                    // Rollback Transaction
                    // =========================
                    transaction.Rollback();
                }
            }
            #region dropDown list
            var listProducts = new List<SelectListItem>();
            listProducts.AddRange(_context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName)
                .Select(x => new SelectListItem
                {
                    Value = x.ProductId.ToString(),
                    Text = x.ProductName
                })
                .ToList());
            ViewData["ProductId"] = listProducts;
            #endregion
            ViewData["SalesReturn"] = _context.SalesReturns.Where(x => x.SalesReturnId == salesReturnDetail.SalesReturnId).FirstOrDefault();
            return View(salesReturnDetail);
        }

        // GET: SalesReturnDetails/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var SalesReturnDetail = await _context.SalesReturnDetails.FindAsync(id);
            var salesReturnDetail = await _context.SalesReturnDetails
                .Where(x=>x.SalesReturnDetailId == id)
                .Include(x=>x.Product)
                .Include(x=>x.SalesReturn)
                .FirstOrDefaultAsync();
            if (salesReturnDetail == null)
            {
                return NotFound();
            }
            
            return View(salesReturnDetail);
        }

        // POST: SalesReturnDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("SalesReturnDetailId,SalesReturnId,ProductId,Qty,UnitPrice,LineTotal,StoreId,IsActive")] SalesReturnDetail salesReturnDetail)
        {
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    #region order-details
                    var oSalesReturnDetail = await _context.SalesReturnDetails.FindAsync(id);
                    if (oSalesReturnDetail == null)
                    {
                        return NotFound();
                    }
                    oSalesReturnDetail.IsActive = salesReturnDetail.IsActive;
                    // =========================
                    // Save Order-Details
                    // =========================
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oSalesReturn = _context.SalesReturns.Where(x => x.SalesReturnId == oSalesReturnDetail.SalesReturnId).FirstOrDefault();
                    if (oSalesReturn != null)
                    {
                        oSalesReturn.IsActive = true;
                        
                        // =========================
                        // Save Order Header
                        // =========================
                        _context.SaveChanges();
                    }
                    #endregion

                    // =========================
                    // Commit Transaction
                    // =========================
                    transaction.Commit();
                    TempData["message"] = "Please Add Sales items";
                    return RedirectToAction("Edit", "SalesReturns", new { id = oSalesReturn.SalesReturnId });

                }
                catch (Exception ex)
                {
                    // =========================
                    // Rollback Transaction
                    // =========================
                    transaction.Rollback();
                    TempData["message"] = "Exceptions!";
                }
            }
            return View(salesReturnDetail);
        }

        // GET: SalesReturnDetails/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var salesReturnDetail = await _context.SalesReturnDetails
                .Include(p => p.Product)
                .Include(p => p.SalesReturn)
                .FirstOrDefaultAsync(m => m.SalesReturnDetailId == id);
            if (salesReturnDetail == null)
            {
                return NotFound();
            }
            return View(salesReturnDetail);
        }

        // POST: SalesReturnDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    #region order-details
                    var oSalesReturnDetail = await _context.SalesReturnDetails.FindAsync(id);
                    if (oSalesReturnDetail == null)
                    {
                        return NotFound();
                    }
                    // =========================
                    // Delete Order-Details
                    // =========================
                    _context.SalesReturnDetails.Remove(oSalesReturnDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oReturn = _context.SalesReturns.Where(x => x.SalesReturnId == oSalesReturnDetail.SalesReturnId).FirstOrDefault();
                    if (oReturn != null)
                    {
                        oReturn.IsActive = true;
                        //oReturn.DiscountAmount = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.DiscountAmount);
                        //oReturn.SubTotal = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.LineTotal);
                        //oReturn.TaxAmount = (oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0)) * (oOrder.TaxPercent ?? 0) / 100;
                        //oReturn.TotalAmount = oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0) + (oOrder.TaxAmount ?? 0);
                        // =========================
                        // Save Order Header
                        // =========================
                        _context.SaveChanges();
                    }
                    #endregion

                    // =========================
                    // Commit Transaction
                    // =========================
                    transaction.Commit();
                    TempData["message"] = "Please Add Sales Return items";
                    return RedirectToAction("Edit", "SalesReturns", new { id = oSalesReturnDetail.SalesReturnId });
                    
                }
                catch (Exception ex)
                {
                    // =========================
                    // Rollback Transaction
                    // =========================
                    transaction.Rollback();
                    TempData["message"] = "Exceptions!";
                }
            }
            return RedirectToAction("Index", "SalesReturns");
        }

        private bool SalesReturnDetailsExists(long id)
        {
            return _context.SalesReturnDetails.Any(e => e.SalesReturnDetailId == id);
        }
    }
}
