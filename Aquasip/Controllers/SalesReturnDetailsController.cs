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

        // GET: PurchaseOrderDetails
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.PurchaseOrderDetails.Include(p => p.Product).Include(p => p.PurchaseOrder);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: PurchaseOrderDetails/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderDetail = await _context.PurchaseOrderDetails
                .Include(p => p.Product)
                .Include(p => p.PurchaseOrder)
                .FirstOrDefaultAsync(m => m.PurchaseOrderDetailId == id);
            if (purchaseOrderDetail == null)
            {
                return NotFound();
            }

            return View(purchaseOrderDetail);
        }

        // GET: PurchaseOrderDetails/Create
        public IActionResult Create(long PurchaseReturnId)
        {
            var oPurchaseReturn = _context.PurchaseReturns.Where(x => x.PurchaseReturnId == PurchaseReturnId).Include(i => i.Supplier).FirstOrDefault();
            if (oPurchaseReturn != null)
            {
                var oPurchaseOrder = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == oPurchaseReturn.PurchaseOrderId).Include(i => i.Supplier).FirstOrDefault();
                if (oPurchaseOrder != null)
                {
                    oPurchaseOrder.PurchaseOrderDetails = _context.PurchaseOrderDetails.Include(x => x.Product).Where(x => x.PurchaseOrderId == oPurchaseOrder.PurchaseOrderId && x.IsActive == true).ToList();
                }
                ViewData["PurchaseOrder"] = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == oPurchaseReturn.PurchaseOrderId).Include(i => i.Supplier).FirstOrDefault();
            }
            ViewData["PurchaseReturn"] = oPurchaseReturn;
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

        // POST: PurchaseOrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseReturnDetailId,PurchaseReturnId,ProductId,Qty,UnitCost,LineTotal,StoreId,IsActive")] PurchaseReturnDetail purchaseReturnDetail)
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
                    purchaseReturnDetail.IsActive = true;
                    _context.Add(purchaseReturnDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oPurchaseReturn = _context.PurchaseReturns.Where(x => x.PurchaseReturnId == purchaseReturnDetail.PurchaseReturnId).FirstOrDefault();
                    if (oPurchaseReturn != null) 
                    {
                        oPurchaseReturn.IsActive = true;
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
                    ViewData["PurchaseReturn"] = _context.PurchaseReturns.Where(x => x.PurchaseReturnId == purchaseReturnDetail.PurchaseReturnId).FirstOrDefault();
                    return RedirectToAction("Edit", "PurchaseReturns", new { id = purchaseReturnDetail.PurchaseReturnId });
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
            ViewData["PurchaseReturn"] = _context.PurchaseReturns.Where(x => x.PurchaseReturnId == purchaseReturnDetail.PurchaseReturnId).FirstOrDefault();
            return View(purchaseReturnDetail);
        }

        // GET: PurchaseOrderDetails/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var purchaseOrderDetail = await _context.PurchaseOrderDetails.FindAsync(id);
            var purchaseReturnDetail = await _context.PurchaseReturnDetails
                .Where(x=>x.PurchaseReturnDetailId == id)
                .Include(x=>x.Product)
                .Include(x=>x.PurchaseReturn)
                .FirstOrDefaultAsync();
            if (purchaseReturnDetail == null)
            {
                return NotFound();
            }
            //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", purchaseOrderDetail.ProductId);
            //ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrders, "PurchaseOrderId", "PurchaseOrderId", purchaseOrderDetail.PurchaseOrderId);
            return View(purchaseReturnDetail);
        }

        // POST: PurchaseOrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PurchaseOrderDetailId,PurchaseOrderId,ProductId,Qty,UnitCost,DiscountAmount,LineTotal,StoreId,IsActive")] PurchaseOrderDetail purchaseOrderDetail)
        {
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    #region order-details
                    var oPurchaseReturnDetail = await _context.PurchaseReturnDetails.FindAsync(id);
                    //var oPurchaseOrderDetail = await _context.PurchaseOrderDetails.Where(x=>x.PurchaseOrderDetailId == purchaseOrderDetail.PurchaseOrderDetailId).FirstOrDefaultAsync();
                    if (oPurchaseReturnDetail == null)
                    {
                        return NotFound();
                    }
                    oPurchaseReturnDetail.IsActive = purchaseOrderDetail.IsActive;
                    // =========================
                    // Save Order-Details
                    // =========================
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oPurchaseReturn = _context.PurchaseReturns.Where(x => x.PurchaseReturnId == oPurchaseReturnDetail.PurchaseReturnId).FirstOrDefault();
                    if (oPurchaseReturn != null)
                    {
                        oPurchaseReturn.IsActive = true;
                        //oPurchaseReturn.DiscountAmount = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.DiscountAmount);
                        //oPurchaseReturn.SubTotal = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.LineTotal);
                        //oPurchaseReturn.TaxAmount = (oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0)) * (oOrder.TaxPercent ?? 0) / 100;
                        //oPurchaseReturn.TotalAmount = oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0) + (oOrder.TaxAmount ?? 0);
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
                    TempData["message"] = "Please Add Purchase items";
                    return RedirectToAction("Edit", "PurchaseReturns", new { id = oPurchaseReturn.PurchaseReturnId });

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
            return View(purchaseOrderDetail);
        }

        // GET: PurchaseOrderDetails/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderDetail = await _context.PurchaseOrderDetails
                .Include(p => p.Product)
                .Include(p => p.PurchaseOrder)
                .FirstOrDefaultAsync(m => m.PurchaseOrderDetailId == id);
            if (purchaseOrderDetail == null)
            {
                return NotFound();
            }

            return View(purchaseOrderDetail);
        }

        // POST: PurchaseOrderDetails/Delete/5
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
                    var oPurchaseOrderDetail = await _context.PurchaseOrderDetails.FindAsync(id);
                    if (oPurchaseOrderDetail == null)
                    {
                        return NotFound();
                    }
                    // =========================
                    // Delete Order-Details
                    // =========================
                    _context.PurchaseOrderDetails.Remove(oPurchaseOrderDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oOrder = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId).FirstOrDefault();
                    if (oOrder != null)
                    {
                        oOrder.IsActive = true;
                        oOrder.DiscountAmount = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.DiscountAmount);
                        oOrder.SubTotal = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.LineTotal);
                        oOrder.TaxAmount = (oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0)) * (oOrder.TaxPercent ?? 0) / 100;
                        oOrder.TotalAmount = oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0) + (oOrder.TaxAmount ?? 0);
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
                    TempData["message"] = "Please Add Purchase items";
                    return RedirectToAction("Edit", "PurchaseOrders", new { id = oPurchaseOrderDetail.PurchaseOrderId });
                    
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
            return RedirectToAction("Index", "PurchaseOrders");
        }

        private bool PurchaseOrderDetailExists(long id)
        {
            return _context.PurchaseOrderDetails.Any(e => e.PurchaseOrderDetailId == id);
        }
    }
}
