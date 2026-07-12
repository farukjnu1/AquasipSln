using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Services.TokenServices;
using Aquasip.Utilities;
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
    public class PurchaseReturnsController : Controller
    {
        private readonly AquasipContext _context;
        private readonly ITokenService _tokenService;

        public PurchaseReturnsController(AquasipContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index(string? poNumber, int pageSize = 100)
        {
            var listPurchaseOrder = new List<PurchaseOrder>();
            if (!string.IsNullOrEmpty(poNumber))
            {
                listPurchaseOrder = _context.PurchaseOrders.Where(x => x.Ponumber == poNumber)
                    .Include(p => p.Supplier)
                    .Include(x => x.PurchaseState)  
                    .OrderByDescending(x => x.Podate)
                    .ToList();
            }
            ViewData["PurchaseOrders"] = listPurchaseOrder;
            ViewData["Ponumber"] = poNumber;
            ViewData["PageSize"] = pageSize;
            var aquasipContext = _context.PurchaseReturns
                .Include(p => p.PurchaseOrder)
                .Include(p => p.Supplier).OrderByDescending(x => x.ReturnDate);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PurchaseReturnVM oPurchaseReturn = new PurchaseReturnVM();
            try
            {
                #region dropdown list
                ViewData["PaymentMethodId"] = _context.PaymentMethods.OrderBy(x => x.PaymentMethodId)
                    .Select(x => new SelectListItem
                    {
                        Value = x.PaymentMethodId.ToString(),
                        Text = x.PaymentMethodName
                    })
                    .ToList();
                ViewData["PaymentStatusId"] = _context.PaymentStatuses.OrderBy(x => x.Sequence)
                    .Select(x => new SelectListItem
                    {
                        Value = x.PaymentStateId.ToString(),
                        Text = x.PaymentStatus1
                    })
                    .ToList();
                #endregion
                #region Model of PurchaseReturnVM
                var purchaseReturn = await _context.PurchaseReturns
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(m => m.PurchaseReturnId == id);
                if (purchaseReturn == null)
                {
                    return NotFound();
                }
                string referenceCode = "PurchaseReturnDetail";
                var oReferenceType = _context.ReferenceTypes.Where(x => x.Code == referenceCode).FirstOrDefault();
                if (oReferenceType == null)
                {
                    return NotFound();
                }
                var listPurchaseReturnDetail = await _context.PurchaseReturnDetails
                    .Where(x => x.PurchaseReturnId == id && x.IsActive == true)
                    .Include(x => x.Product)
                    .Select(x => new PurchaseReturnDetailVM
                    {
                        LineTotal = x.LineTotal,
                        UnitCost = x.UnitCost,
                        ProductId = x.ProductId,
                        PurchaseReturnDetailId = x.PurchaseReturnDetailId,
                        PurchaseReturnId = x.PurchaseReturnId,
                        Qty = x.Qty,
                        IsActive = x.IsActive,
                        Product = new ProductVM { ProductId = x.Product.ProductId, ProductName = x.Product.ProductName },
                        StoreId = x.StoreId,
                        ReferenceToken = CodeGenerate.TextToHex(referenceCode),
                        TransactionTypeToken = _tokenService.Encrypt("2") // 1 for plus stock, 2 for minus stock
                    }).ToListAsync();
                foreach (var item in listPurchaseReturnDetail)
                {
                    var oStock = _context.StockTransactions.Where(x => x.ReferenceId == item.PurchaseReturnDetailId && x.ReferenceTypeId == oReferenceType.ReferenceTypeId).FirstOrDefault();
                    item.IsStockUpdated = oStock == null ? false : true;
                }
                oPurchaseReturn = new PurchaseReturnVM
                {
                    IsActive = purchaseReturn.IsActive,
                    ReturnDate = purchaseReturn.ReturnDate,
                    ReturnNumber = purchaseReturn.ReturnNumber,
                    PurchaseOrderId = purchaseReturn.PurchaseOrderId,
                    PurchaseReturnId = purchaseReturn.PurchaseReturnId,
                    Remark = purchaseReturn.Remark,
                    Supplier = new SupplierVM { SupplierId = purchaseReturn.Supplier.SupplierId, SupplierCode = purchaseReturn.Supplier.SupplierCode, SupplierName = purchaseReturn.Supplier.SupplierName },
                    SupplierId = purchaseReturn.SupplierId,
                    PurchaseReturnDetails = listPurchaseReturnDetail
                };
                #endregion
            }
            catch 
            {
            }
            return View(oPurchaseReturn);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create(long id)
        {
            var oPurchaseOrder = _context.PurchaseOrders.Include(x => x.Supplier).Where(x => x.PurchaseOrderId == id).FirstOrDefault();
            if (oPurchaseOrder != null)
            {
                oPurchaseOrder.PurchaseOrderDetails = _context.PurchaseOrderDetails.Include(x=>x.Product).Where(x => x.PurchaseOrderId == id && x.IsActive == true).ToList();
            }
            ViewData["PurchaseOrder"] = oPurchaseOrder;
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseOrderId,ReturnDate,SupplierId,IsActive")] PurchaseReturn purchaseReturn)
        {
            try
            {
                purchaseReturn.ReturnNumber = CodeGenerate.PurchaseReturnNum(DateTime.Now);
                _context.Add(purchaseReturn);
                await _context.SaveChangesAsync();
                TempData["message"] = "Please Add Purchase-Return items";
                return RedirectToAction("Create", "PurchaseReturnDetails", new { purchaseReturnId = purchaseReturn.PurchaseReturnId });
            }
            catch 
            {
                TempData["message"] = "Exceptions!";
            }
            return View(purchaseReturn);
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseReturn = await _context.PurchaseReturns.Include(x=>x.Supplier).FirstOrDefaultAsync(x=>x.PurchaseReturnId == id);
            if (purchaseReturn == null)
            {
                return NotFound();
            }

            #region dropdown list
            var listSuppliers = new List<SelectListItem>();
            listSuppliers.AddRange(_context.Suppliers.OrderBy(x => x.SupplierName)
                .Select(x => new SelectListItem
                {
                    Value = x.SupplierId.ToString(),
                    Text = x.SupplierName
                })
                .ToList());
            ViewData["SupplierId"] = listSuppliers;
            var listPurchaseOrderState = new List<SelectListItem>();
            listPurchaseOrderState.AddRange(_context.PurchaseOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.PurchaseStateId.ToString(),
                    Text = x.PurchaseStatus
                })
                .ToList());
            ViewData["PurchaseStateId"] = listPurchaseOrderState;
            #endregion
            ViewData["PurchaseReturnDetails"] = await _context.PurchaseReturnDetails.Where(x => x.PurchaseReturnId == id && x.IsActive == true).Include(x=>x.Product).ToListAsync();
            return View(purchaseReturn);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PurchaseReturnId,ReturnDate,SupplierId,IsActive,Remark")] PurchaseReturn purchaseReturn)
        {
            if (id != purchaseReturn.PurchaseReturnId)
            {
                return NotFound();
            }
            
            try
            {
                #region order-summery
                _context.Update(purchaseReturn);
                await _context.SaveChangesAsync();

                var oPurchaseReturn = _context.PurchaseReturns.Where(x => x.PurchaseReturnId == purchaseReturn.PurchaseReturnId).FirstOrDefault();
                if (oPurchaseReturn != null)
                {
                    oPurchaseReturn.IsActive = true;
                    oPurchaseReturn.Remark = purchaseReturn.Remark;
                    // =========================
                    // Save Order Header
                    // =========================
                    _context.SaveChanges();
                }
                #endregion
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseReturnExists(purchaseReturn.PurchaseReturnId))
                {
                    //return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var listSuppliers = new List<SelectListItem>();
            listSuppliers.AddRange(_context.Suppliers.OrderBy(x => x.SupplierName)
                .Select(x => new SelectListItem
                {
                    Value = x.SupplierId.ToString(),
                    Text = x.SupplierName
                })
                .ToList());
            ViewData["SupplierId"] = listSuppliers;
            var listPurchaseOrderState = new List<SelectListItem>();
            listPurchaseOrderState.AddRange(_context.PurchaseOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.PurchaseStateId.ToString(),
                    Text = x.PurchaseStatus
                })
                .ToList());
            ViewData["PurchaseStateId"] = listPurchaseOrderState;
            ViewData["PurchaseOrderDetails"] = await _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == id && x.IsActive == true).Include(x => x.Product).ToListAsync();

            return View(purchaseReturn);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseReturn = await _context.PurchaseReturns
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseReturnId == id);
            if (purchaseReturn == null)
            {
                return NotFound();
            }

            return View(purchaseReturn);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var purchaseReturn = await _context.PurchaseReturns.FindAsync(id);
            if (purchaseReturn != null)
            {
                _context.PurchaseReturns.Remove(purchaseReturn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseReturnExists(long id)
        {
            return _context.PurchaseReturns.Any(e => e.PurchaseReturnId == id);
        }
    }
}
