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
        private readonly ITokenService _tokenService;
        public SalesOrdersController(ILogger<SalesOrdersController> logger, IConfiguration configuration, IWebHostEnvironment environment, ITokenService tokenService)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _context = new AquasipContext();
            _tokenService = tokenService;
        }

        // GET: Orders
        public async Task<IActionResult> Index(int OrderStateId = 0, int PageSize = 10)
        {
            #region Dropdown List
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
            #endregion
            #region Data
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            #endregion
            return View(soRepo.GetAll(OrderStateId, PageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            #region Dropdown List
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
            #region Model of SalesOrder
            string referenceCode = "SalesOrderDetails";
            var oReferenceType = _context.ReferenceTypes.Where(x => x.Code == referenceCode).FirstOrDefault();
            if (oReferenceType == null)
            {
                return NotFound();
            }
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            var oSalesOrder = soRepo.GetById((long)id);
            if (oSalesOrder == null)
            {
                TempData["message"] = "Sales-Order not found.";
                return NotFound();
            }
            oSalesOrder.CustomerPayment = new CustomerPaymentVM { OrderId = oSalesOrder.OrderId };
            foreach (var item in oSalesOrder.OrderDetails)
            {
                item.ReferenceToken = CodeGenerate.TextToHex(referenceCode);
                item.TransactionTypeToken = _tokenService.Encrypt("2"); // 1 for plus stock, 2 for minus stock
                var oStock = _context.StockTransactions.Where(x => x.ReferenceId == item.OrderDetailId && x.ReferenceTypeId == oReferenceType.ReferenceTypeId).FirstOrDefault();
                item.IsStockUpdated = oStock == null ? false : true;
            }
            #endregion
            return View(oSalesOrder);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            #region Dropdown list
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            //ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId");
            ViewData["OrderStateId"] = _context.SalesOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.OrderStateId.ToString(),
                    Text = x.OrderStatus
                }).ToList();
            ViewData["PaymentMethodId"] = _context.PaymentMethods.OrderBy(x => x.PaymentMethodName)
                .Select(x => new SelectListItem
                {
                    Value = x.PaymentMethodId.ToString(),
                    Text = x.PaymentMethodName
                }).ToList();
            #endregion
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesOrderVM order)
        {
            try
            {
                //var products = HttpContext.Session.GetString("Cart");
                //var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);
                var listProduct = new List<ProductVM>();
                #region order-details
                listProduct.ForEach(x =>
                {
                    order.OrderDetails.Add(new SalesOrderVM.SalesOrderDetailVM
                    {
                        ProductId = x.ProductId,
                        Qty = (int)x.Quantity,
                        UnitPrice = x.Price ?? 0,
                        TotalPrice = x.Total ?? 0
                    });
                });
                #endregion
                #region order-summary
                //PageRepository pageRepo = new PageRepository(_connectionString);
                //PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);
                //var cartPage = pageRepo.GetBySlug("cart");
                //cartPage.PageContents = pageContentRepo.GetBySlugPage("cart");
                //var delivery_charge = cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "delivery_charge").FirstOrDefault() == null ? new Aquasip.Models.PageContentVM()
                //    : cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "delivery_charge").First();
                //var gateway_charge = cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "gateway_charge").FirstOrDefault() == null ? new Aquasip.Models.PageContentVM()
                //    : cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "gateway_charge").First();
                //var vat = cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "vat").FirstOrDefault() == null ? new Aquasip.Models.PageContentVM()
                //    : cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "vat").First();

                decimal? grandTotal = 0;
                decimal? subTotal = 0;
                decimal? vatedValue = 0;
                foreach (var item in listProduct)
                {
                    subTotal += item.Total == null ? 0 : item.Total;
                }
                grandTotal += subTotal;
                //if (grandTotal > 0)
                //{
                //    if (vat.IsActive == true)
                //    {
                //        grandTotal += subTotal * (Convert.ToDecimal(vat.Header) / 100);
                //        vatedValue = subTotal * (Convert.ToDecimal(vat.Header) / 100);
                //        vatedValue = Math.Round(Convert.ToDecimal(vatedValue), 2);
                //    }
                //    if (delivery_charge.IsActive == true)
                //    {
                //        grandTotal += Convert.ToDecimal(delivery_charge.Header);
                //    }
                //    if (gateway_charge.IsActive == true)
                //    {
                //        grandTotal += Convert.ToDecimal(gateway_charge.Header);
                //    }
                //    grandTotal = Math.Round(Convert.ToDecimal(grandTotal), 2);
                //}
                order.SubTotal = subTotal ?? 0;
                //order.VatPercent = vat.IsActive == true ? Convert.ToDecimal(vat.Header) : 0;
                order.VatAmount = vatedValue ?? 0;
                //order.DeliveryCharge = delivery_charge.IsActive == true ? Convert.ToDecimal(delivery_charge.Header) : 0;
                //order.GatewayCharge = gateway_charge.IsActive == true ? Convert.ToDecimal(gateway_charge.Header) : 0;
                order.GrandTotal = grandTotal ?? 0;
                //order.Notes = string.Join(", ", listProduct.Select(x => $"{x.ProductName} (Qty: {x.Quantity})"));
                order.OrderStateId = 1; // PENDING
                #endregion
                #region save
                SalesOrderRepository orderRepo = new SalesOrderRepository(_connectionString);
                var response = orderRepo.Add(order);
                if (response != null)
                {
                    if (response.Success == true)
                    {
                        //HttpContext.Session.Remove("Cart");
                        TempData["message"] = response.Message;
                    }
                    else
                    {
                        TempData["message"] = response.Message;
                    }
                }
                #endregion
                #region Dropdown list
                ViewData["OrderStateId"] = _context.SalesOrderStates.OrderBy(x => x.Sequence)
                    .Select(x => new SelectListItem
                    {
                        Value = x.OrderStateId.ToString(),
                        Text = x.OrderStatus
                    }).ToList();
                ViewData["PaymentMethodId"] = _context.PaymentMethods.OrderBy(x => x.PaymentMethodName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.PaymentMethodId.ToString(),
                        Text = x.PaymentMethodName
                    }).ToList();
                #endregion
                TempData["message"] = "Please Add Sales-Order items";
                var id = response != null ? Convert.ToInt64(response.Status) : 0;
                return RedirectToAction("Create", "SalesOrderDetails", new { orderId = id });
            }
            catch 
            {

            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            #region Dropdown list
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            //ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            //ViewData["ShippingAddressId"] = new SelectList(_context.ShippingAddresses, "ShippingAddressId", "ShippingAddressId");
            ViewData["OrderStateId"] = _context.SalesOrderStates.OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem
                {
                    Value = x.OrderStateId.ToString(),
                    Text = x.OrderStatus
                }).ToList();
            ViewData["PaymentMethodId"] = _context.PaymentMethods.OrderBy(x => x.PaymentMethodName)
                .Select(x => new SelectListItem
                {
                    Value = x.PaymentMethodId.ToString(),
                    Text = x.PaymentMethodName
                }).ToList();
            #endregion
            SalesOrderRepository soRepo = new SalesOrderRepository(_connectionString);
            var oSalesOrder = soRepo.GetById((long)id);
            return View(oSalesOrder);
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
