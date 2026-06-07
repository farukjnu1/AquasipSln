using Aquasip.EF;
using Aquasip.Fiters;
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

        public StockTransactionsController(AquasipContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", stockTransaction.ProductId);
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", stockTransaction.ProductId);
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", stockTransaction.ProductId);
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
