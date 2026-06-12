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
    public class PurchaseOrdersController : Controller
    {
        private readonly AquasipContext _context;
        private readonly ITokenService _tokenService;

        public PurchaseOrdersController(AquasipContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(x => x.PurchaseState);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PurchaseOrderVM oPurchaseOrder = new PurchaseOrderVM();
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
                #region Model of PurchaseOrderVM
                var purchaseOrder = await _context.PurchaseOrders
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
                if (purchaseOrder == null)
                {
                    return NotFound();
                }
                string referenceCode = "PurchaseOrderDetail";
                var listPurchaseOrderDetail = await _context.PurchaseOrderDetails
                    .Where(x => x.PurchaseOrderId == id && x.IsActive == true)
                    .Include(x => x.Product)
                    .Select(x => new PurchaseOrderDetailVM
                    {
                        LineTotal = x.LineTotal,
                        UnitCost = x.UnitCost,
                        ProductId = x.ProductId,
                        PurchaseOrderDetailId = x.PurchaseOrderDetailId,
                        PurchaseOrderId = x.PurchaseOrderId,
                        Qty = x.Qty,
                        IsActive = x.IsActive,
                        Product = new ProductVM { ProductId = x.Product.ProductId, ProductName = x.Product.ProductName },
                        DiscountAmount = x.DiscountAmount,
                        StoreId = x.StoreId,
                        ReferenceToken = CodeGenerate.TextToHex(referenceCode),
                        TransactionTypeToken = _tokenService.Encrypt("1") // 1 for plus stock, 2 for minus stock
                    }).ToListAsync();
                var oReferenceType = _context.ReferenceTypes.Where(x => x.Code == referenceCode).FirstOrDefault();
                if (oReferenceType == null)
                {
                    return NotFound();
                }
                foreach (var item in listPurchaseOrderDetail)
                {
                    var oStock = _context.StockTransactions.Where(x => x.ReferenceId == item.PurchaseOrderDetailId && x.ReferenceTypeId == oReferenceType.ReferenceTypeId).FirstOrDefault();
                    item.IsStockUpdated = oStock == null ? false : true;
                }
                oPurchaseOrder = new PurchaseOrderVM
                {
                    DiscountAmount = purchaseOrder.DiscountAmount,
                    IsActive = purchaseOrder.IsActive,
                    OtherCharge = purchaseOrder.OtherCharge,
                    Podate = purchaseOrder.Podate,
                    Ponumber = purchaseOrder.Ponumber,
                    PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                    PurchaseStateId = purchaseOrder.PurchaseStateId,
                    Remark = purchaseOrder.Remark,
                    SubTotal = purchaseOrder.SubTotal,
                    Supplier = new SupplierVM { SupplierId = purchaseOrder.Supplier.SupplierId, SupplierCode = purchaseOrder.Supplier.SupplierCode, SupplierName = purchaseOrder.Supplier.SupplierName },
                    SupplierId = purchaseOrder.SupplierId,
                    SupplierPayments = new List<SupplierPaymentVM>(),
                    SupplierPayment = new SupplierPaymentVM() { PurchaseOrderId = purchaseOrder.PurchaseOrderId },
                    TaxAmount = purchaseOrder.TaxAmount,
                    TaxPercent = purchaseOrder.TaxPercent,
                    TotalAmount = purchaseOrder.TotalAmount,
                    PurchaseOrderDetails = listPurchaseOrderDetail
                };
                oPurchaseOrder.SupplierPayments = _context.SupplierPayments
                    .Where(sp => sp.PurchaseOrderId == oPurchaseOrder.PurchaseOrderId)
                    .Include(pm => pm.PaymentMethod)
                    .Include(ps => ps.PaymentStatus).Select(x => new SupplierPaymentVM
                    {
                        PaidAmount = x.PaidAmount,
                        PaymentMethodId = x.PaymentMethodId,
                        PaymentStatusId = x.PaymentStatusId,
                        PaymentDate = x.PaymentDate,
                        Remarks = x.Remarks,
                        PaymentId = x.PaymentId,
                        IsActive = x.IsActive,
                        PaymentMethod = new PaymentMethodVM { PaymentMethodId = x.PaymentMethod.PaymentMethodId, PaymentMethodName = x.PaymentMethod.PaymentMethodName },
                        PaymentStatus = new PaymentStatusVM { PaymentStateId = x.PaymentStatus.PaymentStateId, PaymentStatus1 = x.PaymentStatus.PaymentStatus1 },
                        PurchaseOrderId = x.PurchaseOrderId,
                        TransactionNumber = x.TransactionNumber
                    }).ToList();
                #endregion
            }
            catch 
            {
                
            }
            return View(oPurchaseOrder);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {
            var listSuppliers = new List<SelectListItem>();
            listSuppliers.AddRange(_context.Suppliers.OrderBy(x => x.SupplierName)
                .Select(x => new SelectListItem
                {
                    Value = x.SupplierId.ToString(),
                    Text = x.SupplierName
                })
                .ToList());
            ViewData["SupplierId"] = listSuppliers;
            //ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId");
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PurchaseOrderId,Ponumber,Podate,SupplierId,SubTotal,DiscountAmount,TaxAmount,TotalAmount,IsActive")] PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.Ponumber = CodeGenerate.PurchaseOrderNumber(DateTime.Now);
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                TempData["message"] = "Please Add Purchase items";
                return RedirectToAction("Create", "PurchaseOrderDetails", new { purchaseOrderId = purchaseOrder.PurchaseOrderId });
            }
            catch 
            {
                TempData["message"] = "Exceptions!";
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
            
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            //ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId", purchaseOrder.SupplierId);
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
            ViewData["PurchaseOrderDetails"] = await _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == id && x.IsActive == true).Include(x=>x.Product).ToListAsync();

            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PurchaseOrderId,Ponumber,Podate,SupplierId,SubTotal,DiscountAmount,TaxPercent,TaxAmount,TotalAmount,IsActive,PurchaseStateId,Remark")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.PurchaseOrderId)
            {
                return NotFound();
            }
            
            try
            {
                #region order-summery
                _context.Update(purchaseOrder);
                await _context.SaveChangesAsync();

                var oOrder = _context.PurchaseOrders.Where(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId).FirstOrDefault();
                if (oOrder != null)
                {
                    oOrder.IsActive = true;
                    oOrder.SubTotal = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderId == purchaseOrder.PurchaseOrderId).Sum(x => x.LineTotal);
                    oOrder.TaxAmount = (oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0)) * (oOrder.TaxPercent ?? 0) / 100;
                    oOrder.TotalAmount = oOrder.SubTotal - (oOrder.DiscountAmount ?? 0) + (oOrder.OtherCharge ?? 0) + (oOrder.TaxAmount ?? 0);
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
                if (!PurchaseOrderExists(purchaseOrder.PurchaseOrderId))
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

            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseOrderId == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(purchaseOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(long id)
        {
            return _context.PurchaseOrders.Any(e => e.PurchaseOrderId == id);
        }
    }
}
