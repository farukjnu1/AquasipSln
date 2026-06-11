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
    public class PurchaseOrderDetailsController : Controller
    {
        private readonly AquasipContext _context;

        public PurchaseOrderDetailsController(AquasipContext context)
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
        public IActionResult Create(long PurchaseOrderId)
        {
            //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            var listProducts = new List<SelectListItem>();
            listProducts.AddRange(_context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName).Select(x => new SelectListItem
                {
                    Value = x.ProductId.ToString(),
                    Text = x.ProductName
                })
                .ToList());
            ViewData["ProductId"] = listProducts;
            ViewData["PurchaseOrderId"] = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == PurchaseOrderId).Include(i => i.Supplier).FirstOrDefault();
            return View();
        }

        // POST: PurchaseOrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseOrderDetailId,PurchaseOrderId,ProductId,Qty,UnitCost,DiscountAmount,LineTotal,StoreId,IsActive")] PurchaseOrderDetail purchaseOrderDetail)
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
                    purchaseOrderDetail.IsActive = true;
                    purchaseOrderDetail.LineTotal = (purchaseOrderDetail.Qty ?? 0) * (purchaseOrderDetail.UnitCost ?? 0) - (purchaseOrderDetail.DiscountAmount ?? 0);
                    _context.Add(purchaseOrderDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oOrder = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == purchaseOrderDetail.PurchaseOrderId).FirstOrDefault();
                    if (oOrder != null) 
                    {
                        oOrder.IsActive = true;
                        oOrder.DiscountAmount = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.DiscountAmount);
                        oOrder.SubTotal = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.LineTotal);
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
                    return RedirectToAction("Edit", "PurchaseOrders", new { id = purchaseOrderDetail.PurchaseOrderId });
                }
                catch (Exception ex)
                {
                    // =========================
                    // Rollback Transaction
                    // =========================
                    transaction.Rollback();
                }
            }

            //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            var listProducts = new List<SelectListItem>();
            listProducts.AddRange(_context.Products.Where(x => x.IsActive == true).OrderBy(x => x.ProductName)
                .Select(x => new SelectListItem
                {
                    Value = x.ProductId.ToString(),
                    Text = x.ProductName
                })
                .ToList());
            ViewData["ProductId"] = listProducts;
            ViewData["PurchaseOrderId"] = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == purchaseOrderDetail.PurchaseOrderId).FirstOrDefault();

            return View(purchaseOrderDetail);
        }

        // GET: PurchaseOrderDetails/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var purchaseOrderDetail = await _context.PurchaseOrderDetails.FindAsync(id);
            var purchaseOrderDetail = await _context.PurchaseOrderDetails.Where(x=>x.PurchaseOrderDetailId == id).Include(x=>x.Product).Include(x=>x.PurchaseOrder).FirstOrDefaultAsync();
            if (purchaseOrderDetail == null)
            {
                return NotFound();
            }
            //ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", purchaseOrderDetail.ProductId);
            //ViewData["PurchaseOrderId"] = new SelectList(_context.PurchaseOrders, "PurchaseOrderId", "PurchaseOrderId", purchaseOrderDetail.PurchaseOrderId);
            return View(purchaseOrderDetail);
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
                    var oPurchaseOrderDetail = await _context.PurchaseOrderDetails.FindAsync(id);
                    //var oPurchaseOrderDetail = await _context.PurchaseOrderDetails.Where(x=>x.PurchaseOrderDetailId == purchaseOrderDetail.PurchaseOrderDetailId).FirstOrDefaultAsync();
                    if (oPurchaseOrderDetail == null)
                    {
                        return NotFound();
                    }
                    oPurchaseOrderDetail.IsActive = purchaseOrderDetail.IsActive;
                    // =========================
                    // Save Order-Details
                    // =========================
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
