using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;
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
    public class SalesOrdersController : Controller
    {
        private readonly ILogger<SalesOrdersController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly AquasipContext _context;
        public SalesOrdersController(ILogger<SalesOrdersController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = new AquasipContext();
        }

        // GET: Orders
        public async Task<IActionResult> Index(int OrderStateId = 0, int PageSize = 10)
        {
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
            ViewData["nOrderStateId"] = OrderStateId;
            ViewData["PageSize"] = PageSize;
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            return View(soRepo.GetAll(OrderStateId, PageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
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

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderNumber,CustomerId,ShippingAddressId,PaymentMethodId,OrderDate,SubTotal,VatPercent,VatAmount,DeliveryCharge,GatewayCharge,GrandTotal,OrderStateId,Notes")] SalesOrder order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", order.PaymentMethodId);
            ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId", order.ShippingAddressId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //ViewData["OrderStateId"] = new SelectList(_context.OrderStates, "OrderStateId", "OrderStateId", order.OrderStateId);
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", order.PaymentMethodId);
            //ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId", order.ShippingAddressId);
            ViewData["OrderStateId"] = _context.SalesOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.OrderStateId.ToString(),
                    Text = x.OrderStatus
                })
                .ToList();
            /*var order = await (from o in _context.SalesOrders.Include(o => o.Customer)
                               join os in _context.SalesOrderStates on o.OrderStateId equals os.OrderStateId
                               join pm in _context.PaymentMethods on o.PaymentMethodId equals pm.PaymentMethodId
                               where o.OrderId == id
                               select new SalesOrderVM
                               {
                                   CustomerName = o.Customer.FullName,
                                   GrandTotal = o.GrandTotal,
                                   Notes = o.Notes,
                                   OrderId = o.OrderId,
                                   OrderNumber = o.OrderNumber,
                                   OrderDate = o.OrderDate,
                                   OrderStatus = os.OrderStatus,
                                   CustomerId = o.CustomerId,
                                   DeliveryCharge = o.DeliveryCharge,
                                   GatewayCharge = o.GatewayCharge,
                                   OrderStateId = o.OrderStateId,
                                   PaymentMethodId = o.PaymentMethodId,
                                   PaymentMethod = pm.PaymentMethodName,
                                   StreetAddress = o.ShippingAddress.StreetAddress,
                                   SubTotal = o.SubTotal,
                                   VatAmount = o.VatAmount,
                                   VatPercent = o.VatPercent,
                                   OrderDetails = (from od in _context.SalesOrderDetails.Include(pp=>pp.Product) 
                                                   join p in _context.Products on od.ProductId equals p.ProductId
                                                   where od.OrderId == o.OrderId
                                                   select new SalesOrderVM.SalesOrderDetailVM {
                                                       OrderDetailId = od.OrderDetailId,
                                                       OrderId = od.OrderId,
                                                       ProductId = od.ProductId,
                                                       Qty = od.Qty,
                                                       UnitPrice = od.UnitPrice,
                                                       TotalPrice = od.TotalPrice,
                                                       ProductName = od.Product.ProductName}).ToList()
                               }).FirstOrDefaultAsync();*/
            //if (order == null)
            //{
            //    return NotFound();
            //}
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            return View(soRepo.GetById((long)id));
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("OrderId,OrderNumber,CustomerId,ShippingAddressId,PaymentMethodId,OrderDate,SubTotal,VatPercent,VatAmount,DeliveryCharge,GatewayCharge,GrandTotal,OrderStateId,Notes")] SalesOrder order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            try
            {
                var oOrder = await (from x in _context.SalesOrders where x.OrderId == order.OrderId select x).FirstOrDefaultAsync();
                if (oOrder != null)
                {
                    oOrder.OrderStateId = order.OrderStateId;
                    oOrder.Notes = order.Notes;
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderExists(order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", order.PaymentMethodId);
            //ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId", order.ShippingAddressId);
            //return View(order);
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var order = await _context.SalesOrders.FindAsync(id);
            if (order != null)
            {
                _context.SalesOrders.Remove(order);
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
