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
using static Aquasip.Models.SalesOrderVM;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class SalesOrderDetailsController : Controller
    {
        private readonly AquasipContext _context;

        public SalesOrderDetailsController(AquasipContext context)
        {
            _context = context;
        }

        // GET: SalesOrderDetails
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.SalesOrderDetails.Include(p => p.Product).Include(p => p.Order);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: SalesOrderDetails/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var salesOrderDetail = await _context.SalesOrderDetails
                .Include(p => p.Product)
                .Include(p => p.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (salesOrderDetail == null)
            {
                return NotFound();
            }
            return View(salesOrderDetail);
        }

        // GET: SalesOrderDetails/Create
        public IActionResult Create(long orderId)
        {
            var oSalesOrder = _context.SalesOrders.Where(x => x.OrderId == orderId).Include(i => i.Customer).FirstOrDefault();
            if (oSalesOrder != null)
            {
                oSalesOrder.SalesOrderDetails = _context.SalesOrderDetails.Include(x => x.Product).Where(x => x.OrderId == oSalesOrder.OrderId && x.IsActive == true).ToList();
            }
            ViewData["SalesOrder"] = oSalesOrder;
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

        // POST: SalesOrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesOrderDetail salesOrderDetail)
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
                    salesOrderDetail.IsActive = true;
                    _context.Add(salesOrderDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oSalesOrder = _context.SalesOrders.Where(x => x.OrderId == salesOrderDetail.OrderId).FirstOrDefault();
                    if (oSalesOrder != null) 
                    {
                        oSalesOrder.IsActive = true;
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
                    ViewData["SalesOrder"] = oSalesOrder;
                    return RedirectToAction("Edit", "SalesOrders", new { id = salesOrderDetail.OrderId });
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
            return View(salesOrderDetail);
        }

        // GET: SalesOrderDetails/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var salesOrderDetail = await _context.SalesOrderDetails
                .Where(x=>x.OrderDetailId == id)
                .Include(x=>x.Product)
                .Include(x=>x.Order)
                .FirstOrDefaultAsync();
            if (salesOrderDetail == null)
            {
                return NotFound();
            }
            
            return View(salesOrderDetail);
        }

        // POST: SalesOrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SalesOrderDetail salesOrderDetail)
        {
            using (var _context = new AquasipContext())
            {
                // Begin Transaction
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    #region order-details
                    var oSalesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);
                    if (oSalesOrderDetail == null)
                    {
                        return NotFound();
                    }
                    oSalesOrderDetail.IsActive = salesOrderDetail.IsActive;
                    // =========================
                    // Save Order-Details
                    // =========================
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oSalesOrder = _context.SalesOrders.Where(x => x.OrderId == salesOrderDetail.OrderId).FirstOrDefault();
                    if (oSalesOrder != null)
                    {
                        oSalesOrder.IsActive = true;
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
                    return RedirectToAction("Edit", "SalesOrders", new { id = oSalesOrder.OrderId });
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
            return View(salesOrderDetail);
        }

        // GET: SalesOrderDetails/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var salesOrderDetail = await _context.SalesOrderDetails
                .Include(p => p.Product)
                .Include(p => p.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (salesOrderDetail == null)
            {
                return NotFound();
            }
            return View(salesOrderDetail);
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
                    var oSalesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);
                    if (oSalesOrderDetail == null)
                    {
                        return NotFound();
                    }
                    // =========================
                    // Delete Order-Details
                    // =========================
                    _context.SalesOrderDetails.Remove(oSalesOrderDetail);
                    await _context.SaveChangesAsync();
                    #endregion
                    #region order-summery
                    var oOrder = _context.SalesOrders.Where(x => x.OrderId == oSalesOrderDetail.OrderId).FirstOrDefault();
                    if (oOrder != null)
                    {
                        oOrder.IsActive = true;
                        //oOrder.DiscountAmount = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == oPurchaseOrderDetail.PurchaseOrderId && x.IsActive == true).Sum(x => x.DiscountAmount);
                        oOrder.SubTotal = _context.SalesOrderDetails.Where(x => x.OrderId == oSalesOrderDetail.OrderId && x.IsActive == true).Sum(x => x.TotalPrice);
                        //oOrder.TaxAmount = (oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0)) * (oOrder.TaxPercent ?? 0) / 100;
                        //oOrder.TotalAmount = oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0) + (oOrder.TaxAmount ?? 0);
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
                    return RedirectToAction("Edit", "SalesOrders", new { id = oSalesOrderDetail.OrderId });
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
            return RedirectToAction("Index", "SalesOrders");
        }

        private bool SalesOrderDetailsExists(long id)
        {
            return _context.SalesOrderDetails.Any(e => e.OrderDetailId == id);
        }
    }
}
