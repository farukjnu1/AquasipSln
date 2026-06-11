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
    public class SupplierPaymentsController : Controller
    {
        private readonly ILogger<SupplierPaymentsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly AquasipContext _context;
        public SupplierPaymentsController(ILogger<SupplierPaymentsController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = new AquasipContext();
        }

        // GET: SupplierPayments
        public async Task<IActionResult> Index(string? PaymentDateStart = "", string? PaymentDateEnd = "", int PageSize = 10)
        {
            List<SupplierPayment> listSupplierPayment = new List<SupplierPayment>();
            try
            {
                if (string.IsNullOrEmpty(PaymentDateStart) || string.IsNullOrEmpty(PaymentDateEnd))
                {
                    listSupplierPayment = await _context.SupplierPayments
                        .Where(x => x.IsActive == true)
                        .Skip(0)
                        .Take(PageSize)
                        .Include(p => p.PurchaseOrder)
                        .Include(p => p.PaymentMethod)
                        .Include(p => p.PaymentStatus)
                        .ToListAsync();
                }
                else
                {
                    DateTime DateStart = Convert.ToDateTime(PaymentDateStart);
                    DateTime DateEnd = Convert.ToDateTime(PaymentDateEnd);
                    listSupplierPayment = await _context.SupplierPayments
                        .Where(x=> x.IsActive == true && x.PaymentDate > DateStart && x.PaymentDate < DateEnd)
                        .Skip(0)
                        .Take(PageSize)
                        .Include(p => p.PurchaseOrder)
                        .Include(p => p.PaymentMethod)
                        .Include(p => p.PaymentStatus)
                        .ToListAsync();
                }
            }
            catch 
            {
            }
            ViewData["PaymentDateStart"] = PaymentDateStart;
            ViewData["PaymentDateEnd"] = PaymentDateEnd;
            ViewData["PageSize"] = PageSize;
            return View(listSupplierPayment);
        }

        // GET: SupplierPayments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _context.SupplierPayments
                .Include(p => p.PurchaseOrder)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (paymentTransaction == null)
            {
                return NotFound();
            }

            return View(paymentTransaction);
        }

        // GET: CustomerPayments/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.SalesOrders, "OrderId", "OrderId");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId");
            return View();
        }

        // POST: CustomerPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,PurchaseOrderId,PaymentMethodId,TransactionNumber,PaidAmount,PaymentStatusId,PaymentDate,Remarks")] SupplierPayment supplierPayment)
        {
            try
            {
                supplierPayment.IsActive = true;
                _context.Add(supplierPayment);
                await _context.SaveChangesAsync();
                TempData["message"] = "Payment created successfully!";
            }
            catch(Exception ex)
            {
                TempData["message"] = "Exception!";
            }
            //ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", paymentTransaction.OrderId);
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", paymentTransaction.PaymentMethodId);
            //ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId", paymentTransaction.PaymentStatusId);
            //return View(paymentTransaction);
            return RedirectToAction(actionName: "Details", controllerName: "PurchaseOrders", routeValues: new { id = supplierPayment.PurchaseOrderId });
        }

        // GET: PaymentTransactions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplierPayment = await _context.SupplierPayments.FindAsync(id);
            if (supplierPayment == null)
            {
                return NotFound();
            }
            //ViewData["OrderId"] = new SelectList(_context.SalesOrders, "OrderId", "OrderId", paymentTransaction.OrderId);
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", paymentTransaction.PaymentMethodId);
            //ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId", paymentTransaction.PaymentStatusId);
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
            supplierPayment.PurchaseOrder = _context.PurchaseOrders.Find(supplierPayment.PurchaseOrderId);
            return View(supplierPayment);
        }

        // POST: PaymentTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PaymentId,PurchaseOrderId,PaymentMethodId,TransactionNumber,PaidAmount,PaymentStatusId,PaymentDate,Remarks")] SupplierPayment supplierPayment)
        {
            if (id != supplierPayment.PaymentId)
            {
                return NotFound();
            }

            try
            {
                supplierPayment.IsActive = true;
                _context.Update(supplierPayment);
                await _context.SaveChangesAsync();
                TempData["message"] = "Data saved successfully.";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData["message"] = ex.Message;
                if (!CustomerPaymentExists(supplierPayment.PaymentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            //ViewData["OrderId"] = new SelectList(_context.SalesOrders, "OrderId", "OrderId", paymentTransaction.OrderId);
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", paymentTransaction.PaymentMethodId);
            //ViewData["PaymentStatusId"] = new SelectList(_context.PaymentStatuses, "PaymentStateId", "PaymentStateId", paymentTransaction.PaymentStatusId);
            //return View(paymentTransaction);
            return RedirectToAction("Details", "PurchaseOrders", new { id = supplierPayment.PurchaseOrderId });
        }

        // GET: PaymentTransactions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplierPayment = await _context.SupplierPayments
                .Include(p => p.PurchaseOrder)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (supplierPayment == null)
            {
                return NotFound();
            }

            return View(supplierPayment);
        }

        // POST: CustomerPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var supplierPayment = await _context.SupplierPayments.FindAsync(id);
            if (supplierPayment != null)
            {
                _context.SupplierPayments.Remove(supplierPayment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerPaymentExists(long id)
        {
            return _context.SupplierPayments.Any(e => e.PaymentId == id);
        }
    }
}
