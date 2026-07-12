using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;
using Aquasip.Services.TokenServices;
using Aquasip.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Aquasip.Controllers
{
    [AdminFilter]
    public class StockTransactionsController : Controller
    {
        private readonly AquasipContext _context;
        private readonly ITokenService _tokenService;
        private readonly string _connectionString;

        public StockTransactionsController(AquasipContext context, ITokenService tokenService, IConfiguration configuration)
        {
            _context = context;
            _tokenService = tokenService;
            _connectionString = configuration.GetConnectionString("AquasipContext");
        }

        // GET: StockTransactions
        public async Task<IActionResult> Index(long ProductId = 0, int StoreId = 0, int PageSize = 10)
        {
            #region Dropdown List
            var listProductSelect = new List<SelectListItem>();
            listProductSelect.Add(new SelectListItem { Value = "0", Text = "All" });
            listProductSelect.AddRange(_context.Products.Where(x=>x.IsActive == true).OrderBy(x => x.ProductName)
                .Select(x => new SelectListItem
                {
                    Value = x.ProductId.ToString(),
                    Text = x.ProductName
                })
                .ToList());
            ViewData["Products"] = listProductSelect;
            ViewData["nProductId"] = ProductId;
            var listStoreSelect = new List<SelectListItem>();
            listStoreSelect.Add(new SelectListItem { Value = "0", Text = "All" });
            listStoreSelect.AddRange(_context.Stores.Where(x=>x.IsActive == true).OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                })
                .ToList());
            ViewData["Stores"] = listStoreSelect;
            ViewData["nStoreId"] = StoreId;
            ViewData["PageSize"] = PageSize == 0 ? 10 : PageSize;
            #endregion
            #region Data
            var listTransactionType = _context.TransactionTypes.ToList();
            var listReferenceType = _context.ReferenceTypes.ToList();
            var listStore = _context.Stores.Where(x => x.IsActive == true).ToList();
            var listStockTransaction = await _context.StockTransactions.Include(s => s.Product).Select(x => new StockTransactionVM
            {
                IsActive = x.IsActive,
                Product = new ProductVM { ProductId = x.Product.ProductId, ProductName = x.Product.ProductName },
                ProductId = x.ProductId,
                QtyIn = x.QtyIn,
                QtyOut = x.QtyOut,
                ReferenceId = x.ReferenceId,
                ReferenceTypeId = x.ReferenceTypeId,
                StockTransactionId = x.StockTransactionId,
                StoreId = x.StoreId,
                TransactionDate = x.TransactionDate,
                TransactionTypeId = x.TransactionTypeId,
                UnitCost = x.UnitCost
            }).ToListAsync();//.OrderBy(x => x.TransactionDate).Skip(0).Take(PageSize).ToListAsync();
            if (ProductId > 0)
            {
                listStockTransaction = listStockTransaction.Where(x => x.ProductId == ProductId).ToList();
            }
            if (StoreId > 0)
            {
                listStockTransaction = listStockTransaction.Where(x => x.StoreId == StoreId).ToList();
            }
            listStockTransaction = listStockTransaction.OrderBy(x => x.TransactionDate).Skip(0).Take(PageSize).ToList();
            decimal? currentStock = 0;
            foreach (var item in listStockTransaction)
            {
                item.ReferenceType = listReferenceType.Where(rt => rt.ReferenceTypeId == item.ReferenceTypeId).Select(rtt => new ReferenceTypeVM { ReferenceTypeId = rtt.ReferenceTypeId, Code = rtt.Code, Name = rtt.Name }).First();
                item.Store = listStore.Where(s => s.StoreId == item.StoreId).Select(ss => new StoreVM { StoreId = ss.StoreId, StoreCode = ss.StoreCode, StoreName = ss.StoreName }).First();
                item.TransactionType = listTransactionType.Where(tt => tt.TransactionTypeId == item.TransactionTypeId).Select(ttt => new TransactionTypeVM { TransactionTypeId = ttt.TransactionTypeId, Code = ttt.Code, Name = ttt.Name }).First();
                currentStock += item.QtyIn - item.QtyOut;
                item.CurrentStock = currentStock;
            }
            #endregion
            return View(listStockTransaction);
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

        // PurchaseOrderDetail
        public IActionResult PurchaseOrderDetail(long id, string referenceToken, string transactionType)
        {
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.Where(x => x.IsActive == true).OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                }).ToList());
            ViewData["StoreId"] = listStore;
            #endregion
            //ViewData["ProductId"] = new SelectList(_context.Products.Where(x=>x.IsActive == true), "ProductId", "ProductId");
            #region Model of StockTransaction
            var referenceCode = CodeGenerate.HexToText(referenceToken);
            var referenceType = _context.ReferenceTypes.FirstOrDefault(x => x.Code == referenceCode);
            if (referenceType == null)
            {
                //return NotFound();
                TempData["message"] = "Reference not valid.";
                return RedirectToAction("Index", "PurchaseOrders");
            }
            var transactionTypeId = Convert.ToInt32(_tokenService.Decrypt(transactionType));
            if (transactionTypeId != 1) // 1 for plus stock, 2 for minus stock
            {
                //return NotFound();
                TempData["message"] = "Transaction not valid.";
                return RedirectToAction("Index", "PurchaseOrders");
            }
            long productId = 0;
            decimal? unitCost = 0, qtyIn = 0, qtyOut = 0;
            if (referenceCode != "PurchaseOrderDetail")
            {
                //return NotFound();
                TempData["message"] = "Purchase not valid.";
                return RedirectToAction("Index", "PurchaseOrders");
            }
            if (referenceCode == "PurchaseOrderDetail")
            {
                var oPurchaseOrderDetail = _context.PurchaseOrderDetails.FirstOrDefault(x => x.PurchaseOrderDetailId == id);
                if (oPurchaseOrderDetail == null)
                {
                    //return NotFound();
                    TempData["message"] = "Purchase not found.";
                    return RedirectToAction("Index", "PurchaseOrders");
                }
                productId = oPurchaseOrderDetail.ProductId;
                unitCost = oPurchaseOrderDetail.UnitCost;
                qtyIn = oPurchaseOrderDetail.Qty;
            }
            var oProduct = _context.Products.Where(x => x.IsActive == true).FirstOrDefault(x => x.ProductId == productId);
            if (oProduct == null)
            {
                //return NotFound();
                TempData["message"] = "Product not found.";
                return RedirectToAction("Index", "PurchaseOrders");
            }
            StockTransactionRepository stockRepo = new StockTransactionRepository(_connectionString);
            var listStock = stockRepo.GetCurrentStock(productId);
            StockTransaction stockTransaction = new StockTransaction
            {
                IsActive = true,
                ProductId = productId,
                Product = oProduct,
                QtyIn = qtyIn,
                QtyOut = qtyOut,
                ReferenceId = id,
                ReferenceTypeId = referenceType != null ? referenceType.ReferenceTypeId : (int?)null,
                StockTransactionId = 0,
                StoreId = 0,
                TransactionDate = DateTime.Now,
                TransactionTypeId = transactionTypeId,
                UnitCost = unitCost,
            };
            #endregion
            ViewData["ReferenceType"] = new ReferenceTypeVM { ReferenceTypeId = referenceType.ReferenceTypeId, Code = referenceType.Code, Name = referenceType.Name };
            ViewData["Stocks"] = listStock;
            return View("Create", stockTransaction);
        }

        // PurchaseReturnDetail 
        public IActionResult PurchaseReturnDetail(long id, string referenceToken, string transactionType)
        {
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.Where(x => x.IsActive == true).OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                }).ToList());
            ViewData["StoreId"] = listStore;
            #endregion
            #region Model of StockTransaction
            var referenceCode = CodeGenerate.HexToText(referenceToken);
            var referenceType = _context.ReferenceTypes.FirstOrDefault(x => x.Code == referenceCode);
            if (referenceType == null)
            {
                //return NotFound();
                TempData["message"] = "Reference not valid.";
                return RedirectToAction("Index", "PurchaseReturns");
            }
            var transactionTypeId = Convert.ToInt32(_tokenService.Decrypt(transactionType));
            if (transactionTypeId != 2) // 1 for plus stock, 2 for minus stock
            {
                //return NotFound();
                TempData["message"] = "Transaction not valid.";
                return RedirectToAction("Index", "PurchaseReturns");
            }
            long productId = 0;
            decimal? unitCost = 0, qtyIn = 0, qtyOut = 0;
            if (referenceCode != "PurchaseReturnDetail")
            {
                //return NotFound();
                TempData["message"] = "Purchase not valid.";
                return RedirectToAction("Index", "PurchaseReturns");
            }
            if (referenceCode == "PurchaseReturnDetail")
            {
                var oPurchaseReturnDetail = _context.PurchaseReturnDetails.FirstOrDefault(x => x.PurchaseReturnDetailId == id);
                if (oPurchaseReturnDetail == null)
                {
                    //return NotFound();
                    TempData["message"] = "Purchase not found.";
                    return RedirectToAction("Index", "PurchaseReturns");
                }
                productId = oPurchaseReturnDetail.ProductId;
                unitCost = oPurchaseReturnDetail.UnitCost;
                qtyOut = oPurchaseReturnDetail.Qty;
            }
            var oProduct = _context.Products.Where(x => x.IsActive == true).FirstOrDefault(x => x.ProductId == productId);
            if (oProduct == null)
            {
                //return NotFound();
                TempData["message"] = "Product not found.";
                return RedirectToAction("Index", "PurchaseReturns");
            }
            StockTransactionRepository stockRepo = new StockTransactionRepository(_connectionString);
            var listStock = stockRepo.GetCurrentStock(productId);
            StockTransaction stockTransaction = new StockTransaction
            {
                IsActive = true,
                ProductId = productId,
                Product = oProduct,
                QtyIn = qtyIn,
                QtyOut = qtyOut,
                ReferenceId = id,
                ReferenceTypeId = referenceType != null ? referenceType.ReferenceTypeId : (int?)null,
                StockTransactionId = 0,
                StoreId = 0,
                TransactionDate = DateTime.Now,
                TransactionTypeId = transactionTypeId,
                UnitCost = unitCost,
            };
            #endregion
            ViewData["ReferenceType"] = new ReferenceTypeVM { ReferenceTypeId = referenceType.ReferenceTypeId, Code = referenceType.Code, Name = referenceType.Name };
            ViewData["Stocks"] = listStock;
            return View("Create", stockTransaction);
        }

        //SalesOrderDetail
        public IActionResult SalesOrderDetail(long id, string referenceToken, string transactionType)
        {
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.Where(x => x.IsActive == true).OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                }).ToList());
            ViewData["StoreId"] = listStore;
            #endregion
            //ViewData["ProductId"] = new SelectList(_context.Products.Where(x=>x.IsActive == true), "ProductId", "ProductId");
            #region Model of StockTransaction
            var referenceCode = CodeGenerate.HexToText(referenceToken);
            var referenceType = _context.ReferenceTypes.FirstOrDefault(x => x.Code == referenceCode);
            if (referenceType == null)
            {
                //return NotFound();
                TempData["message"] = "Reference not valid.";
                return RedirectToAction("Index", "SalesOrders");
            }
            var transactionTypeId = Convert.ToInt32(_tokenService.Decrypt(transactionType));
            if (transactionTypeId != 2) // 1 for plus stock, 2 for minus stock
            {
                //return NotFound();
                TempData["message"] = "Transaction not valid.";
                return RedirectToAction("Index", "SalesOrders");
            }
            long productId = 0;
            decimal? unitCost = 0, qtyIn = 0, qtyOut = 0;
            if (referenceCode != "SalesOrderDetails")
            {
                //return NotFound();
                TempData["message"] = "Sales not valid.";
                return RedirectToAction("Index", "SalesOrders");
            }
            if (referenceCode == "SalesOrderDetails")
            {
                var oSalesOrderDetail = _context.SalesOrderDetails.FirstOrDefault(x => x.OrderDetailId == id);
                if (oSalesOrderDetail == null)
                {
                    //return NotFound();
                    TempData["message"] = "Sales not found.";
                    return RedirectToAction("Index", "SalesOrders");
                }
                productId = oSalesOrderDetail.ProductId;
                unitCost = oSalesOrderDetail.UnitPrice;
                qtyOut = oSalesOrderDetail.Qty;
            }
            var oProduct = _context.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (oProduct == null)
            {
                //return NotFound();
                TempData["message"] = "Product not found.";
                return RedirectToAction("Index", "SalesOrders");
            }
            StockTransactionRepository stockRepo = new StockTransactionRepository(_connectionString);
            var listStock = stockRepo.GetCurrentStock(productId);
            var oStock = listStock.Where(x=>x.ProductId == productId).FirstOrDefault();
            if (oStock == null)
            {
                //return NotFound();
                TempData["message"] = "Stock not found.";
                return RedirectToAction("Index", "SalesOrders");
            }
            if(oStock.CurrentStock < qtyOut)
            {
                //return NotFound();
                TempData["message"] = "Not enough stock. Current stock is " + oStock.CurrentStock;
                return RedirectToAction("Index", "SalesOrders");
            }
            StockTransaction stockTransaction = new StockTransaction
            {
                IsActive = true,
                ProductId = productId,
                Product = oProduct,
                QtyIn = qtyIn,
                QtyOut = qtyOut,
                ReferenceId = id,
                ReferenceTypeId = referenceType != null ? referenceType.ReferenceTypeId : (int?)null,
                StockTransactionId = 0,
                StoreId = 0,
                TransactionDate = DateTime.Now,
                TransactionTypeId = transactionTypeId,
                UnitCost = unitCost,
            };
            #endregion
            ViewData["ReferenceType"] = new ReferenceTypeVM { ReferenceTypeId = referenceType.ReferenceTypeId, Code = referenceType.Code, Name = referenceType.Name };
            ViewData["Stocks"] = listStock;
            return View("Create", stockTransaction);
        }

        // GET: StockTransactions/Create
        public IActionResult Create(long id, string referenceToken, string transactionType)
        {
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.Where(x => x.IsActive == true).OrderBy(x => x.StoreName)
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
            var oProduct = _context.Products.Where(x => x.IsActive == true).FirstOrDefault(x => x.ProductId == productId);
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
            try
            {
                var oStockTransaction = await _context.StockTransactions.Where(x => x.ReferenceId == stockTransaction.ReferenceId && x.ReferenceTypeId == stockTransaction.ReferenceTypeId).FirstOrDefaultAsync();
                if (oStockTransaction != null) 
                {
                    TempData["message"] = "Stocks already received. Try aanother.";
                    return RedirectToAction(nameof(Index));
                }
                stockTransaction.TransactionDate = DateTime.Now;
                _context.Add(stockTransaction);
                await _context.SaveChangesAsync();
                TempData["message"] = "Stocks saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["message"] = "Something went wrong.";
            }
            #region Dropdown list
            var listStore = new List<SelectListItem>();
            listStore.AddRange(_context.Stores.Where(x => x.IsActive == true).OrderBy(x => x.StoreName)
                .Select(x => new SelectListItem
                {
                    Value = x.StoreId.ToString(),
                    Text = x.StoreName
                }).ToList());
            ViewData["StoreId"] = listStore;
            #endregion
            return RedirectToAction(nameof(Index));
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
