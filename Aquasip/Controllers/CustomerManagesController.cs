using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
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
    public class CustomerManagesController : Controller
    {
        private readonly AquasipContext _context;

        public CustomerManagesController(AquasipContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.OrderBy(x=>x.FullName).ToListAsync());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            var shippingAddress = (from x in _context.ShippingAddresses where x.CustomerId == id select 
                                   new ShippingAddressVM { 
                                       City = x.City,
                                       CountryCode =x.CountryCode,
                                       CreatedDate =x.CreatedDate,
                                       CustomerId = x.CustomerId,
                                       EmailAddress = x.EmailAddress,
                                       FullName = x.FullName,
                                       PhoneNumber = x.PhoneNumber,
                                       PostalCode = x.PostalCode,
                                       ShippingAddressId = x.ShippingAddressId,
                                       StateProvince = x.StateProvince,
                                       StreetAddress = x.StreetAddress
                                   }).FirstOrDefault();
            if (shippingAddress == null)
            {
                shippingAddress = new ShippingAddressVM();
            }
            shippingAddress.customer = new CustomerVM();
            shippingAddress.customer.CreatedAt = customer.CreatedAt;
            shippingAddress.customer.CustomerCode = customer.CustomerCode;
            shippingAddress.customer.CustomerId = customer.CustomerId;
            shippingAddress.customer.Email = customer.Email;
            shippingAddress.customer.FullName = customer.FullName;
            shippingAddress.customer.IsActive = customer.IsActive == null ? false : Convert.ToBoolean(customer.IsActive);
            shippingAddress.customer.PhoneNumber = customer.PhoneNumber;
            return View(shippingAddress);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var oCustomer = _context.Customers.Where(x => x.Email.Trim() == customer.Email.Trim()).FirstAsync();
                if (oCustomer != null)
                {
                    TempData["message"] = "Customer already saved with e-mail '" + customer.Email.Trim() + "'";
                    return View(customer);
                }
                else 
                {
                    customer.CustomerCode = CodeGenerate.CustomerNum(DateTime.Now);
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(customer);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var oCustomer = _context.Customers.Where(x => x.CustomerId == customer.CustomerId).FirstOrDefault();
                    if (oCustomer != null)
                    {
                        if (oCustomer.Email.Trim() == customer.Email.Trim())
                        {
                            _context.Update(customer);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var oCustomer2 = _context.Customers.Where(x => x.Email.Trim() == customer.Email.Trim()).FirstOrDefault();
                            if (oCustomer != null)
                            {
                                TempData["message"] = "Customer already saved with e-mail '" + customer.Email.Trim() + "'";
                                return View(customer);
                            }
                            else
                            {
                                _context.Update(customer);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShippingAddress(ShippingAddressVM shippingAddress)
        {
            #region shipping-address
            var oShippingAddress = (from x in _context.ShippingAddresses
                                    where x.CustomerId == shippingAddress.CustomerId
                                    select x).FirstOrDefault();
            if (oShippingAddress == null)
            {
                oShippingAddress = new ShippingAddress
                {
                    CustomerId = shippingAddress.CustomerId,
                    City = shippingAddress.City,
                    StateProvince = shippingAddress.StateProvince,
                    PostalCode = shippingAddress.PostalCode,
                    CountryCode = shippingAddress.CountryCode,
                    StreetAddress = shippingAddress.StreetAddress,
                    FullName = shippingAddress.customer.FullName ?? "",
                    EmailAddress = shippingAddress.customer.Email,
                    PhoneNumber = shippingAddress.customer.PhoneNumber
                };
                _context.ShippingAddresses.Add(oShippingAddress);
                _context.SaveChanges();
                TempData["message"] = "Shipping address is added.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                oShippingAddress.City = shippingAddress.City;
                oShippingAddress.StateProvince = shippingAddress.StateProvince;
                oShippingAddress.PostalCode = shippingAddress.PostalCode;
                oShippingAddress.CountryCode = shippingAddress.CountryCode;
                oShippingAddress.StreetAddress = shippingAddress.StreetAddress;
                oShippingAddress.FullName = shippingAddress.customer.FullName ?? "";
                oShippingAddress.EmailAddress = shippingAddress.customer.Email;
                oShippingAddress.PhoneNumber = shippingAddress.customer.PhoneNumber;
                _context.SaveChanges();
                TempData["message"] = "Shipping address is updated.";
                return RedirectToAction(nameof(Index));
            }
            #endregion
            return View(oShippingAddress);
        }

        private bool CustomerExists(long id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

    }
}
