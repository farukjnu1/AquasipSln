using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Services.TokenServices;
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
    public class StockTransactionsController : Controller
    {
        private readonly AquasipContext _context;
        private readonly ITokenService _tokenService;

        public StockTransactionsController(AquasipContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // GET: StockTransactions
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.StockTransactions.Include(s => s.Product);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: StockTransactions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockTransaction = await _context.StockTransactions
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.StockTransactionId == id);
            if (stockTransaction == null)
            {
                return NotFound();
            }

            return View(stockTransaction);
        }

        // GET: StockTransactions/Create
        public IActionResult Create(long id, string referenceToken, string transactionType)
        {
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                }).ToList());
            ViewData["StoreId"] = listStore;
            #endregion
            //ViewData["ProductId"] = new SelectList(_context.Products.Where(x=>x.IsActive == true), "ProductId", "ProductId");
            #region Model of StockTransaction
            var referenceCode = _tokenService.Decrypt(referenceToken);
            var referenceType = _context.ReferenceTypes.FirstOrDefault(x => x.Code == referenceCode);
            if(referenceType == null)
            {
                return NotFound();
            }
            var transactionTypeId = Convert.ToInt32(_tokenService.Decrypt(transactionType));
            if (transactionTypeId != 1 && transactionTypeId != 2) // 1 for plus stock, 2 for minus stock
            {
                return NotFound();
            }
            long productId = 0; 
            decimal? unitCost = 0;
            if (referenceCode == "PurchaseOrderDetail")
            {
                var oPurchaseOrderDetail = _context.PurchaseOrderDetails.FirstOrDefault(x => x.PurchaseOrderDetailId == id);
                if (oPurchaseOrderDetail == null)
                {
                    return NotFound();
                }
                productId = oPurchaseOrderDetail.ProductId;
                unitCost = oPurchaseOrderDetail.UnitCost;
            }
            if (referenceCode == "PurchaseReturnDetail")
            {
                var oPurchaseReturnDetail = _context.PurchaseReturnDetails.FirstOrDefault(x => x.PurchaseReturnDetailId == id);
                if (oPurchaseReturnDetail == null)
                {
                    return NotFound();
                }
                productId = oPurchaseReturnDetail.ProductId;
                unitCost = oPurchaseReturnDetail.UnitCost;
            }
            if (referenceCode == "SalesOrderDetails")
            {
                var oSalesOrderDetail = _context.SalesOrderDetails.FirstOrDefault(x => x.OrderDetailId == id);
                if (oSalesOrderDetail == null)
                {
                    return NotFound();
                }
                productId = oSalesOrderDetail.ProductId;
                unitCost = oSalesOrderDetail.UnitPrice;
            }
            if (referenceCode == "SalesReturnDetail")
            {
                var oSalesReturnDetail = _context.SalesReturnDetails.FirstOrDefault(x => x.SalesReturnDetailId == id);
                if (oSalesReturnDetail == null)
                {
                    return NotFound();
                }
                productId = oSalesReturnDetail.ProductId;
                unitCost = oSalesReturnDetail.UnitPrice;
            }
            var oProduct = _context.Products.FirstOrDefault(x => x.ProductId == productId);
            if (oProduct == null) 
            {
                return NotFound();
            }
            StockTransaction stockTransaction = new StockTransaction
            {
                IsActive = true,
                ProductId = productId,
                Product = oProduct,
                QtyIn = 0,
                QtyOut = 0,
                ReferenceId = id,
                ReferenceTypeId = referenceType != null ? referenceType.ReferenceTypeId : (int?)null,
                StockTransactionId = 0,
                StoreId = 0,
                TransactionDate = DateTime.Now,
                TransactionTypeId = transactionTypeId,
                UnitCost = unitCost,
            };
            #endregion
            
            return View(stockTransaction);
        }

        // POST: StockTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockTransactionId,TransactionDate,ProductId,TransactionTypeId,ReferenceTypeId,ReferenceId,QtyIn,QtyOut,UnitCost,StoreId,IsActive")] StockTransaction stockTransaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                }).ToList());
            ViewData["StoreId"] = listStore;
            #endregion
            return View(stockTransaction);
        }

        // GET: StockTransactions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockTransaction = await _context.StockTransactions.FindAsync(id);
            if (stockTransaction == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products.Where(x => x.IsActive == true), "ProductId", "ProductId", stockTransaction.ProductId);
            return View(stockTransaction);
        }

        // POST: StockTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("StockTransactionId,TransactionDate,ProductId,TransactionTypeId,ReferenceTypeId,ReferenceId,QtyIn,QtyOut,UnitCost,StoreId,IsActive")] StockTransaction stockTransaction)
        {
            if (id != stockTransaction.StockTransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockTransactionExists(stockTransaction.StockTransactionId))
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
            ViewData["ProductId"] = new SelectList(_context.Products.Where(x => x.IsActive == true), "ProductId", "ProductId", stockTransaction.ProductId);
            return View(stockTransaction);
        }

        // GET: StockTransactions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockTransaction = await _context.StockTransactions
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.StockTransactionId == id);
            if (stockTransaction == null)
            {
                return NotFound();
            }

            return View(stockTransaction);
        }

        // POST: StockTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var stockTransaction = await _context.StockTransactions.FindAsync(id);
            if (stockTransaction != null)
            {
                _context.StockTransactions.Remove(stockTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockTransactionExists(long id)
        {
            return _context.StockTransactions.Any(e => e.StockTransactionId == id);
        }
    }
}
