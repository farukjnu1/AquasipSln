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
    public class SalesReturnsController : Controller
    {
        private readonly AquasipContext _context;
        private readonly ITokenService _tokenService;

        public SalesReturnsController(AquasipContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index(string? orderNum, int pageSize = 100)
        {
            var listSalesOrder = new List<SalesOrder>();
            if (!string.IsNullOrEmpty(orderNum))
            {
                listSalesOrder = _context.SalesOrders.Where(x => x.OrderNumber == orderNum)
                    .Include(p => p.Customer)
                    .OrderByDescending(x => x.OrderDate)
                    .ToList();
            }
            ViewData["SalesOrders"] = listSalesOrder;
            ViewData["OrderNumber"] = orderNum;
            ViewData["PageSize"] = pageSize;
            var aquasipContext = _context.SalesReturns
                .Include(p => p.SalesOrder)
                .Include(p => p.Customer).OrderByDescending(x => x.ReturnDate);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SalesReturnVM oSalesReturn = new SalesReturnVM();
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
                var salesReturn = await _context.SalesReturns
                    .Include(p => p.Customer)
                    .FirstOrDefaultAsync(m => m.SalesReturnId == id);
                if (salesReturn == null)
                {
                    return NotFound();
                }
                string referenceCode = "SalesReturnDetail";
                var oReferenceType = _context.ReferenceTypes.Where(x => x.Code == referenceCode).FirstOrDefault();
                if (oReferenceType == null)
                {
                    return NotFound();
                }
                var listSalesReturnDetail = await _context.SalesReturnDetails
                    .Where(x => x.SalesReturnId == id && x.IsActive == true)
                    .Include(x => x.Product)
                    .Select(x => new SalesReturnDetailVM
                    {
                        LineTotal = x.LineTotal,
                        ProductId = x.ProductId,
                        UnitPrice = x.UnitPrice,
                        SalesReturnDetailId = x.SalesReturnDetailId,
                        SalesReturnId = x.SalesReturnId,
                        Qty = x.Qty,
                        IsActive = x.IsActive,
                        Product = new ProductVM { ProductId = x.Product.ProductId, ProductName = x.Product.ProductName },
                        StoreId = x.StoreId,
                        ReferenceToken = CodeGenerate.TextToHex(referenceCode),
                        TransactionTypeToken = _tokenService.Encrypt("1") // 1 for plus stock, 2 for minus stock
                    }).ToListAsync();
                foreach (var item in listSalesReturnDetail)
                {
                    var oStock = _context.StockTransactions.Where(x => x.ReferenceId == item.SalesReturnDetailId && x.ReferenceTypeId == oReferenceType.ReferenceTypeId).FirstOrDefault();
                    item.IsStockUpdated = oStock == null ? false : true;
                }
                oSalesReturn = new SalesReturnVM
                {
                    IsActive = salesReturn.IsActive,
                    ReturnDate = salesReturn.ReturnDate,
                    ReturnNumber = salesReturn.ReturnNumber,
                    SalesOrderId = salesReturn.SalesOrderId,
                    SalesReturnId = salesReturn.SalesReturnId,
                    Notes = salesReturn.Notes,
                    Customer = new CustomerVM { CustomerId = salesReturn.Customer.CustomerId, CustomerCode = salesReturn.Customer.CustomerCode, FullName = salesReturn.Customer.FullName },
                    CustomerId = salesReturn.CustomerId,
                    SalesReturnDetails = listSalesReturnDetail
                };
                #endregion
            }
            catch 
            {
            }
            return View(oSalesReturn);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create(long id)
        {
            var oSalesOrder = _context.SalesOrders.Include(x => x.Customer).Where(x => x.OrderId == id).FirstOrDefault();
            if (oSalesOrder != null)
            {
                oSalesOrder.SalesOrderDetails = _context.SalesOrderDetails.Include(x=>x.Product).Where(x => x.OrderId == id && x.IsActive == true).ToList();
            }
            ViewData["SalesOrder"] = oSalesOrder;
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalesOrderId,ReturnDate,CustomerId,IsActive")] SalesReturn salesReturn)
        {
            try
            {
                salesReturn.ReturnNumber = CodeGenerate.PurchaseReturnNum(DateTime.Now);
                _context.Add(salesReturn);
                await _context.SaveChangesAsync();
                TempData["message"] = "Please Add Purchase-Return items";
                return RedirectToAction("Create", "SalesReturnDetails", new { purchaseReturnId = salesReturn.SalesReturnId });
            }
            catch 
            {
                TempData["message"] = "Exceptions!";
            }
            return View(salesReturn);
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesReturn = await _context.SalesReturns.Include(x=>x.Customer).FirstOrDefaultAsync(x=>x.SalesReturnId == id);
            if (salesReturn == null)
            {
                return NotFound();
            }

            #region dropdown list
            var listCustomer = new List<SelectListItem>();
            listCustomer.AddRange(_context.Customers.OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.CustomerId.ToString(),
                    Text = x.FullName
                })
                .ToList());
            ViewData["CustomerId"] = listCustomer;
            var listSalesOrderState = new List<SelectListItem>();
            listSalesOrderState.AddRange(_context.SalesOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.OrderStateId.ToString(),
                    Text = x.Remark
                })
                .ToList());
            ViewData["OrderStateId"] = listSalesOrderState;
            #endregion
            ViewData["SalesReturnDetails"] = await _context.SalesReturnDetails.Where(x => x.SalesReturnId == id && x.IsActive == true).Include(x=>x.Product).ToListAsync();
            return View(salesReturn);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("SalesOrderId,ReturnDate,CustomerId,IsActive,Notes")] SalesReturn salesReturn)
        {
            if (id != salesReturn.SalesReturnId)
            {
                return NotFound();
            }
            try
            {
                #region order-summery
                _context.Update(salesReturn);
                await _context.SaveChangesAsync();

                var oSalesReturn = _context.SalesReturns.Where(x => x.SalesReturnId == salesReturn.SalesReturnId).FirstOrDefault();
                if (oSalesReturn != null)
                {
                    oSalesReturn.IsActive = true;
                    oSalesReturn.Notes = salesReturn.Notes;
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
                if (!SalesReturnExists(salesReturn.SalesReturnId))
                {
                    //return NotFound();
                }
                else
                {
                    throw;
                }
            }

            #region dropdown list
            var listCustomer = new List<SelectListItem>();
            listCustomer.AddRange(_context.Customers.OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.CustomerId.ToString(),
                    Text = x.FullName
                })
                .ToList());
            ViewData["CustomerId"] = listCustomer;
            var listSalesOrderState = new List<SelectListItem>();
            listSalesOrderState.AddRange(_context.SalesOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.OrderStateId.ToString(),
                    Text = x.Remark
                })
                .ToList());
            ViewData["OrderStateId"] = listSalesOrderState;
            #endregion
            ViewData["SalesOrderDetails"] = await _context.SalesOrderDetails.Where(x => x.OrderId == id && x.IsActive == true).Include(x => x.Product).ToListAsync();

            return View(salesReturn);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesReturn = await _context.SalesReturns
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.SalesReturnId == id);
            if (salesReturn == null)
            {
                return NotFound();
            }

            return View(salesReturn);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var salesReturn = await _context.SalesReturns.FindAsync(id);
            if (salesReturn != null)
            {
                _context.SalesReturns.Remove(salesReturn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesReturnExists(long id)
        {
            return _context.SalesReturns.Any(e => e.SalesReturnId == id);
        }
    }
}
