using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;
using Aquasip.Services.EmailServices;
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
    [CustomerFilter]
    public class CustomersController : Controller
    {
        private readonly AquasipContext _context;

        //public CustomersController(AquasipContext context)
        //{
        //    _context = context;
        //}

        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        public CustomersController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment environment, IEmailService emailService, ITokenService tokenService, AquasipContext context)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _emailService = emailService;
            _tokenService = tokenService;
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);
            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");
            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            return View();
        }

        public async Task<IActionResult> SalesOrders(int OrderStateId = 0, int PageSize = 10)
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);
            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");
            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            long CustomerId = HttpContext.Session.GetString("CustomerId") == null ? 0 : Convert.ToInt64(HttpContext.Session.GetString("CustomerId"));

            var listOrderStatus = new List<SelectListItem>();
            listOrderStatus.Add(new SelectListItem { Value = "0", Text = "All" });
            listOrderStatus.AddRange(_context.SalesOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.OrderStateId.ToString(),
                    Text = x.OrderStatus
                })
                .ToList());
            ViewData["OrderStateId"] = listOrderStatus;
            /*var aquasipContext = (from o in _context.SalesOrders.Include(o => o.Customer)
                                  join os in _context.SalesOrderStates on o.OrderStateId equals os.OrderStateId
                                  where o.IsActive == true && o.CustomerId == CustomerId && o.OrderStateId == (OrderStateId == 0 ? o.OrderStateId : OrderStateId)
                                  select new SalesOrderVM
                                  {
                                      CustomerName = o.Customer.FullName,
                                      GrandTotal = o.GrandTotal,
                                      Notes = o.Notes,
                                      OrderId = o.OrderId,
                                      OrderNumber = o.OrderNumber,
                                      OrderDate = o.OrderDate,
                                      OrderStatus = os.OrderStatus,
                                      ColorCode = os.ColorCode
                                  }).OrderByDescending(x => x.OrderId).Take(PageSize);
            return View(await aquasipContext.ToListAsync());*/
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            return View(soRepo.GetAll(OrderStateId, PageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> SalesOrderDetails(long? id)
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);
            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");
            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            if (id == null)
            {
                return NotFound();
            }
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
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            return View(soRepo.GetById((long)id));
        }

        public async Task<IActionResult> CustomerPayments()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);
            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");
            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            long CustomerId = HttpContext.Session.GetString("CustomerId") == null ? 0 : Convert.ToInt64(HttpContext.Session.GetString("CustomerId"));

            var aquasipContext = _context.CustomerPayments.Include(s => s.Order).Include(s => s.PaymentMethod).Include(s => s.PaymentStatus).Where(x=> x.Order.CustomerId == CustomerId && x.PaymentStatusId == 2); // paid
            return View(await aquasipContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.PaymentMethod)
                .Include(s => s.ShippingAddress)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderNumber,CustomerId,ShippingAddressId,PaymentMethodId,OrderDate,SubTotal,VatPercent,VatAmount,DeliveryCharge,GatewayCharge,GrandTotal,OrderStateId,Notes,IsActive")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", salesOrder.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", salesOrder.PaymentMethodId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId", salesOrder.ShippingAddressId);
            return View(salesOrder);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", salesOrder.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", salesOrder.PaymentMethodId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId", salesOrder.ShippingAddressId);
            return View(salesOrder);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("OrderId,OrderNumber,CustomerId,ShippingAddressId,PaymentMethodId,OrderDate,SubTotal,VatPercent,VatAmount,DeliveryCharge,GatewayCharge,GrandTotal,OrderStateId,Notes,IsActive")] SalesOrder salesOrder)
        {
            if (id != salesOrder.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(salesOrder.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", salesOrder.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", salesOrder.PaymentMethodId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId", salesOrder.ShippingAddressId);
            return View(salesOrder);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.PaymentMethod)
                .Include(s => s.ShippingAddress)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder != null)
            {
                _context.SalesOrders.Remove(salesOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderExists(long id)
        {
            return _context.SalesOrders.Any(e => e.OrderId == id);
        }
    }
}
