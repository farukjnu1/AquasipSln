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
    public class CustomerPaymentsController : Controller
    {
        private readonly ILogger<CustomerPaymentsController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly AquasipContext _context;
        public CustomerPaymentsController(ILogger<CustomerPaymentsController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = new AquasipContext();
        }

        // GET: CustomerPayments
        public async Task<IActionResult> Index(string? PaymentDateStart = "", string? PaymentDateEnd = "", int PageSize = 10)
        {
            List<CustomerPayment> listCustomerPayment = new List<CustomerPayment>();
            try
            {
                if (string.IsNullOrEmpty(PaymentDateStart) || string.IsNullOrEmpty(PaymentDateEnd))
                {
                    listCustomerPayment = await _context.CustomerPayments
                        .Where(x => x.IsActive == true)
                        .Skip(0)
                        .Take(PageSize)
                        .Include(p => p.Order)
                        .Include(p => p.PaymentMethod)
                        .Include(p => p.PaymentStatus)
                        .ToListAsync();
                }
                else
                {
                    DateTime DateStart = Convert.ToDateTime(PaymentDateStart);
                    DateTime DateEnd = Convert.ToDateTime(PaymentDateEnd);
                    listCustomerPayment = await _context.CustomerPayments
                        .Where(x=> x.IsActive == true && x.PaymentDate > DateStart && x.PaymentDate < DateEnd)
                        .Skip(0)
                        .Take(PageSize)
                        .Include(p => p.Order)
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
            return View(listCustomerPayment);
        }

        // GET: CustomerPayments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentTransaction = await _context.CustomerPayments
                .Include(p => p.Order)
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
        public async Task<IActionResult> Create([Bind("PaymentId,OrderId,PaymentMethodId,TransactionNumber,PaidAmount,PaymentStatusId,PaymentDate,Remarks")] CustomerPayment customerPayment)
        {
            try
            {
                customerPayment.IsActive = true;
                _context.Add(customerPayment);
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
            return RedirectToAction(actionName: "Details", controllerName: "SalesOrders", routeValues: new { id = customerPayment.OrderId });
        }

        // GET: PaymentTransactions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPayment = await _context.CustomerPayments.FindAsync(id);
            if (customerPayment == null)
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
            customerPayment.Order = _context.SalesOrders.Find(customerPayment.OrderId);
            return View(customerPayment);
        }

        // POST: PaymentTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PaymentId,OrderId,PaymentMethodId,TransactionNumber,PaidAmount,PaymentStatusId,PaymentDate,Remarks")] CustomerPayment customerPayment)
        {
            if (id != customerPayment.PaymentId)
            {
                return NotFound();
            }

            try
            {
                customerPayment.IsActive = true;
                _context.Update(customerPayment);
                await _context.SaveChangesAsync();
                TempData["message"] = "Data saved successfully.";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                TempData["message"] = ex.Message;
                if (!CustomerPaymentExists(customerPayment.PaymentId))
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
            return RedirectToAction("Details", "SalesOrders", new { id = customerPayment.OrderId });
        }

        // GET: PaymentTransactions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPayment = await _context.CustomerPayments
                .Include(p => p.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentStatus)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (customerPayment == null)
            {
                return NotFound();
            }

            return View(customerPayment);
        }

        // POST: CustomerPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var customerPayment = await _context.CustomerPayments.FindAsync(id);
            if (customerPayment != null)
            {
                _context.CustomerPayments.Remove(customerPayment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerPaymentExists(long id)
        {
            return _context.CustomerPayments.Any(e => e.PaymentId == id);
        }
    }
}
