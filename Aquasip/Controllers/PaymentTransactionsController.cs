using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aquasip.EF;

namespace Aquasip.Controllers
{
    public class PaymentTransactionsController : Controller
    {
        private readonly ILogger<PaymentTransactionsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly AquasipContext _context;
        public PaymentTransactionsController(ILogger<PaymentTransactionsController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = new AquasipContext();
        }

        // GET: PaymentTransactions
        public async Task<IActionResult> Index()
        {
            var aquasipContext = _context.PaymentTransactions.Include(p => p.Order).Include(p => p.PaymentMethod).Include(p => p.PaymentStatus);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: PaymentTransactions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _context.PaymentTransactions
                .Include(p => p.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(m => m.PaymentTransactionId == id);
            if (paymentTransaction == null)
            {
                return NotFound();
            }

            return View(paymentTransaction);
        }

        // GET: PaymentTransactions/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId");
            return View();
        }

        // POST: PaymentTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentTransactionId,OrderId,PaymentMethodId,TransactionNumber,PaidAmount,PaymentStatusId,PaymentDate,Remarks")] PaymentTransaction paymentTransaction)
        {
            try
            {
                _context.Add(paymentTransaction);
                await _context.SaveChangesAsync();
                TempData["message"] = "Payment created successfully!";
            }
            catch
            {
                TempData["message"] = "Exception!";
            }
            //ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", paymentTransaction.OrderId);
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", paymentTransaction.PaymentMethodId);
            //ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId", paymentTransaction.PaymentStatusId);
            //return View(paymentTransaction);
            return RedirectToAction(actionName: "Details", controllerName: "Orders", routeValues: new { id = paymentTransaction.OrderId });
        }

        // GET: PaymentTransactions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _context.PaymentTransactions.FindAsync(id);
            if (paymentTransaction == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", paymentTransaction.OrderId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", paymentTransaction.PaymentMethodId);
            ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId", paymentTransaction.PaymentStatusId);
            return View(paymentTransaction);
        }

        // POST: PaymentTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PaymentTransactionId,OrderId,PaymentMethodId,TransactionNumber,PaidAmount,PaymentStatusId,PaymentDate,Remarks")] PaymentTransaction paymentTransaction)
        {
            if (id != paymentTransaction.PaymentTransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentTransactionExists(paymentTransaction.PaymentTransactionId))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", paymentTransaction.OrderId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", paymentTransaction.PaymentMethodId);
            ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId", paymentTransaction.PaymentStatusId);
            return View(paymentTransaction);
        }

        // GET: PaymentTransactions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _context.PaymentTransactions
                .Include(p => p.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(m => m.PaymentTransactionId == id);
            if (paymentTransaction == null)
            {
                return NotFound();
            }

            return View(paymentTransaction);
        }

        // POST: PaymentTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var paymentTransaction = await _context.PaymentTransactions.FindAsync(id);
            if (paymentTransaction != null)
            {
                _context.PaymentTransactions.Remove(paymentTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentTransactionExists(long id)
        {
            return _context.PaymentTransactions.Any(e => e.PaymentTransactionId == id);
        }
    }
}
