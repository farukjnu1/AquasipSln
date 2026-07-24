using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;
using Aquasip.Services.EmailServices;
using Aquasip.Services.TokenServices;
using Aquasip.Utilities;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using System.Diagnostics;

namespace Aquasip.Controllers
{
    [WebsiteFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment environment, IEmailService emailService, ITokenService tokenService)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        #region Requirement from Client
        public IActionResult Index()
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var homePage = pageRepo.GetBySlug("home");
            homePage.PageContents = pageContentRepo.GetBySlugPage("home");

            homePage.Products = new ProductRepository(_connectionString).GetAll();

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            //var siteSettingRepo = new SiteSettingRepository(_connectionString);
            //layoutPage.SiteSettings = siteSettingRepo.GetAll();

            var listPage = new List<PageVM>();
            listPage.Add(homePage);
            listPage.Add(layoutPage);

            return View(listPage);
        }

        public IActionResult Products()
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var productsPage = pageRepo.GetBySlug("products");
            productsPage.PageContents = pageContentRepo.GetBySlugPage("products");

            productsPage.Products = new ProductRepository(_connectionString).GetAll();

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            //var siteSettingRepo = new SiteSettingRepository(_connectionString);
            //layoutPage.SiteSettings = siteSettingRepo.GetAll();

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(productsPage);
            ViewData["aquasip"] = listPage;

            return View();
        }

        public IActionResult About()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var aboutPage = pageRepo.GetBySlug("about");
            aboutPage.PageContents = pageContentRepo.GetBySlugPage("about");

            var listPage = new List<PageVM>();
            listPage.Add(aboutPage);
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion
            return View();
        }

        public IActionResult Contact()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var contactPage = pageRepo.GetBySlug("contact");
            contactPage.PageContents = pageContentRepo.GetBySlugPage("contact");

            var listPage = new List<PageVM>();
            listPage.Add(contactPage);
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            #region Create
            ContactMessageVM model = new ContactMessageVM();
            #endregion

            return View(model);
        }

        [HttpPost]
        public IActionResult Contact(ContactMessageVM model)
        {
            try
            {
                ContactMessageRepository contactRepo = new ContactMessageRepository();
                TempData["message"] = contactRepo.Add(model);
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Appointments");
        }

        public IActionResult Spec(long id)
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var specPage = pageRepo.GetBySlug("spec");
            specPage.PageContents = pageContentRepo.GetBySlugPage("spec");

            //
            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(specPage);
            ViewData["aquasip"] = listPage;
            #endregion

            ProductRepository productRepo = new ProductRepository(_connectionString);
            var model = productRepo.GetById(id);

            return View(model);
        }

        public IActionResult Gallery()
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var galleryPage = pageRepo.GetBySlug("gallery");
            galleryPage.PageContents = pageContentRepo.GetBySlugPage("gallery");

            galleryPage.Galleries = new GalleryRepository(_connectionString).GetAll();

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(galleryPage);
            ViewData["aquasip"] = listPage;

            return View();
        }

        public IActionResult Cart()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var cartPage = pageRepo.GetBySlug("cart");
            cartPage.PageContents = pageContentRepo.GetBySlugPage("cart");

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(cartPage);
            ViewData["aquasip"] = listPage;
            #endregion

            var products = HttpContext.Session.GetString("Cart");
            var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);

            return View(listProduct);
        }

        [HttpPost]
        public IActionResult Cart(long productId, decimal quantity)
        {
            ProductRepository productRepo = new ProductRepository(_connectionString);
            var product = productRepo.GetById(productId);
            if (product != null)
            {
                var products = HttpContext.Session.GetString("Cart");
                var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);
                var oProduct = (from x in listProduct where x.ProductId == productId select x).FirstOrDefault();
                if (oProduct == null)
                {
                    listProduct.Add(new ProductVM
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Price = product.Price,
                        Quantity = quantity,
                        Total = product.Price * quantity,
                        ListProductMedia = product.ListProductMedia
                    });
                }
                else
                {
                    oProduct.Quantity += quantity;
                    oProduct.Total = product.Price * oProduct.Quantity;
                }
                HttpContext.Session.SetString("Cart", JsonConversion.SerializeObject(listProduct));
            }
            return RedirectToAction("Cart");
        }

        public IActionResult CartClearAll()
        {
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Cart");
        }

        public IActionResult CartClear(long id)
        {
            var products = HttpContext.Session.GetString("Cart");
            var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);
            var productToRemove = listProduct.FirstOrDefault(x => x.ProductId == id);
            if (productToRemove != null)
            {
                listProduct.Remove(productToRemove);
                HttpContext.Session.SetString("Cart", JsonConversion.SerializeObject(listProduct));
            }
            return RedirectToAction("Cart");
        }

        public IActionResult CartUpdate(long productId, int quantity)
        {
            ProductRepository productRepo = new ProductRepository(_connectionString);
            var products = HttpContext.Session.GetString("Cart");
            var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);
            var oProduct = (from x in listProduct where x.ProductId == productId select x).FirstOrDefault();
            if (oProduct != null)
            {
                oProduct.Quantity = quantity;
                oProduct.Total = oProduct.Price * oProduct.Quantity;
            }
            HttpContext.Session.SetString("Cart", JsonConversion.SerializeObject(listProduct));
            return RedirectToAction("Cart");
        }

        public IActionResult Checkout()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var cartPage = pageRepo.GetBySlug("cart");
            cartPage.PageContents = pageContentRepo.GetBySlugPage("cart");

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(cartPage);
            ViewData["aquasip"] = listPage;
            #endregion

            var products = HttpContext.Session.GetString("Cart");
            var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);

            return View(listProduct);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Checkout(SalesOrderVM order)
        {
            var products = HttpContext.Session.GetString("Cart");
            var listProduct = string.IsNullOrEmpty(products) ? new List<ProductVM>() : JsonConversion.DeserializeObject<List<ProductVM>>(products);
            
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
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);
            var cartPage = pageRepo.GetBySlug("cart");
            cartPage.PageContents = pageContentRepo.GetBySlugPage("cart");
            var delivery_charge = cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "delivery_charge").FirstOrDefault() == null ? new Aquasip.Models.PageContentVM()
                : cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "delivery_charge").First();
            var gateway_charge = cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "gateway_charge").FirstOrDefault() == null ? new Aquasip.Models.PageContentVM()
                : cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "gateway_charge").First();
            var vat = cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "vat").FirstOrDefault() == null ? new Aquasip.Models.PageContentVM()
                : cartPage.PageContents.Where(x => x.IsActive == true && x.SlugPageContent == "vat").First();

            decimal? grandTotal = 0;
            decimal? subTotal = 0;
            decimal? vatedValue = 0;
            foreach (var item in listProduct)
            {
                subTotal += item.Total == null ? 0 : item.Total;
            }
            grandTotal += subTotal;
            if (grandTotal > 0)
            {
                if (vat.IsActive == true)
                {
                    grandTotal += subTotal * (Convert.ToDecimal(vat.Header) / 100);
                    vatedValue = subTotal * (Convert.ToDecimal(vat.Header) / 100);
                    vatedValue = Math.Round(Convert.ToDecimal(vatedValue), 2);
                }
                if (delivery_charge.IsActive == true)
                {
                    grandTotal += Convert.ToDecimal(delivery_charge.Header);

                }
                if (gateway_charge.IsActive == true)
                {
                    grandTotal += Convert.ToDecimal(gateway_charge.Header);
                }
                grandTotal = Math.Round(Convert.ToDecimal(grandTotal), 2);
            }
            order.SubTotal = subTotal ?? 0;
            order.VatPercent = vat.IsActive == true ? Convert.ToDecimal(vat.Header) : 0;
            order.VatAmount = vatedValue ?? 0;
            order.DeliveryCharge = delivery_charge.IsActive == true ? Convert.ToDecimal(delivery_charge.Header) : 0;
            order.GatewayCharge = gateway_charge.IsActive == true ? Convert.ToDecimal(gateway_charge.Header) : 0;
            order.GrandTotal = grandTotal ?? 0;
            order.Notes = string.Join(", ", listProduct.Select(x => $"{x.ProductName} (Qty: {x.Quantity})"));
            #endregion
            #region save
            if (grandTotal > 0)
            {
                SalesOrderRepository orderRepo = new SalesOrderRepository(_connectionString);
                var response = orderRepo.Add(order);
                if (response != null)
                {
                    if (response.Success == true)
                    {
                        HttpContext.Session.Remove("Cart");
                        TempData["message"] = response.Message;
                    }
                    else
                    {
                        TempData["message"] = response.Message;
                    }
                }
            }
            #endregion
            return RedirectToAction("Checkout");
        }

        public IActionResult Review(int? id)
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var reviewPage = pageRepo.GetBySlug("review");
            reviewPage.PageContents = pageContentRepo.GetBySlugPage("review");

            var listPage = new List<PageVM>();
            listPage.Add(reviewPage);
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            AquasipContext _context = new AquasipContext();
            var listProduct = (from x in _context.Products where x.IsActive == true select x).ToList();
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var item in listProduct)
            {
                selectList.Add(new SelectListItem { Text = item.ProductName, Value = item.ProductId.ToString() });
            }
            ViewData["ProductId"] = selectList;
            var oReview = new ReviewVM
            {
                ProductId = id ?? 0,
                CustomerId = Convert.ToInt64(HttpContext.Session.GetString("CustomerId") ?? "0")
            };

            ReviewRepository reviewRepo = new ReviewRepository(_connectionString);
            var listReview = reviewRepo.GetAll();
            ViewData["Reviews"] = listReview;

            return View(oReview);
        }

        public IActionResult Faq()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var faqPage = pageRepo.GetBySlug("faq");
            faqPage.PageContents = pageContentRepo.GetBySlugPage("faq");

            var listPage = new List<PageVM>();
            listPage.Add(faqPage);
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            return View();
        }
        #endregion

        public IActionResult OurTeams()
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var ourTeamPage = pageRepo.GetBySlug("our_team");
            ourTeamPage.PageContents = pageContentRepo.GetBySlugPage("our_team");

            var listPage = new List<PageVM>();
            listPage.Add(ourTeamPage);

            return View(listPage);
        }

        public IActionResult OurBlogs()
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var ourBlogPage = pageRepo.GetBySlug("our_blog");
            ourBlogPage.PageContents = pageContentRepo.GetBySlugPage("our_blog");

            var listPage = new List<PageVM>();
            listPage.Add(ourBlogPage);

            return View(listPage);
        }

        public IActionResult OurBlog(int id)
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var ourBlogPage = pageRepo.GetBySlug("our_blog");
            ourBlogPage.PageContents.Add(pageContentRepo.GetById(id));

            var listPage = new List<PageVM>();
            listPage.Add(ourBlogPage);

            return View(listPage);
        }

        public IActionResult Appointments()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var appointmentPage = pageRepo.GetBySlug("appointment");
            appointmentPage.PageContents = pageContentRepo.GetBySlugPage("appointment");

            var listPage = new List<PageVM>();
            listPage.Add(appointmentPage);
            ViewData["aquasip"] = listPage;
            #endregion

            #region Create
            AppointmentVM model = new AppointmentVM();
            
            DepartmentRepository deptRepo = new DepartmentRepository();
            var listDept = deptRepo.GetAll();
            List<SelectListItem> selectDepts = new List<SelectListItem>();
            foreach (var item in listDept)
            {
                selectDepts.Add(new SelectListItem { Text = item.Name, Value = item.DepartmentId.ToString() });
            }
            model.DepartmentOptions = selectDepts;
            #endregion
            return View(model);
        }

        public IActionResult Privacy()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var privacyPolicyPage = pageRepo.GetBySlug("privacy_policy");
            privacyPolicyPage.PageContents = pageContentRepo.GetBySlugPage("privacy_policy");

            var listPage = new List<PageVM>();
            listPage.Add(privacyPolicyPage);
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            return View(listPage);
        }

        public IActionResult Terms()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var privacyPolicyPage = pageRepo.GetBySlug("terms_condition");
            privacyPolicyPage.PageContents = pageContentRepo.GetBySlugPage("terms_condition");

            var listPage = new List<PageVM>();
            listPage.Add(privacyPolicyPage);
            listPage.Add(layoutPage);
            ViewData["aquasip"] = listPage;
            #endregion

            return View(listPage);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region customer-signin-signup
        public IActionResult Signup()
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var homePage = pageRepo.GetBySlug("home");
            homePage.PageContents = pageContentRepo.GetBySlugPage("home");

            //
            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(homePage);
            ViewData["aquasip"] = listPage;
            #endregion
            return View();
        }

        [HttpPost]
        public IActionResult Signup(CustomerVM model)
        {
            try
            {
                CustomerRepository customerRepo = new CustomerRepository(_connectionString);
                TempData["message"] = customerRepo.Add(model);
                return RedirectToAction("Signin");
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Signup");
        }

        public IActionResult Signin(string callbackAction, string callbackController)
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var homePage = pageRepo.GetBySlug("home");
            homePage.PageContents = pageContentRepo.GetBySlugPage("home");

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(homePage);
            ViewData["aquasip"] = listPage;
            #endregion
            var model = new CustomerVM { CallbackAction = callbackAction, CallbackController = callbackController };
            return View(model);
        }
        

        [HttpPost]
        public IActionResult Signin([FromForm] CustomerVM model)
        {
            try
            {
                CustomerRepository customerRepo = new CustomerRepository(_connectionString);
                CustomerVM oCustomer = customerRepo.Signin(model);
                if (oCustomer != null)
                {
                    if (oCustomer.IsActive == true)
                    {
                        HttpContext.Session.SetString("CustomerId", oCustomer.CustomerId.ToString());
                        HttpContext.Session.SetString("Email", oCustomer.Email ?? "");
                        HttpContext.Session.SetString("FullName", oCustomer.FullName ?? "");
                        if (model.CallbackAction == "Review" && model.CallbackController == "Home")
                        {
                            return RedirectToAction("Review", "Home");
                        }
                        return RedirectToAction("Index", "Customers");
                    }
                    else
                    {
                        TempData["message"] = "e-mail not verified.";
                        return RedirectToAction("Signin");
                    }
                }
                else
                {
                    TempData["message"] = "User not valid.";
                    return RedirectToAction("Signin");
                }
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RequestEmailVerify(string email)
        {
            try 
            {
                CustomerRepository customerRepo = new CustomerRepository(_connectionString);
                CustomerVM oCustomer = customerRepo.GetByEmail(email);
                if(oCustomer == null)
                {
                    TempData["message"] = "Email not found.";
                    return RedirectToAction("Signin");
                }
                if (oCustomer.IsActive == true)
                {
                    TempData["message"] = "Email already verified.";
                    return RedirectToAction("Signin");
                }
                string plaintText = "email=" + email + "&minit=" + TokenValidation.VerifyEmailInMinutes + "&expir=" + TimeConversion.DateTimeToUnixTimestamp(DateTime.Now);
                string token = _tokenService.Encrypt(plaintText);
                string verificationLink = $"{GetBaseUrl()}home/verifyemail?token={token}";
                await _emailService.SendEmailAsync(
                email,
                "Customer Profile Verification",
                "<div>Dear "+oCustomer.FullName+",</div>" +
                "<div>Verify your email to activate your profile.</div>" +
                "<div>Please, click the link below.</div>" +
                "<a href='" + verificationLink + "' target='_blank'>Verify Aquasip</a>");
                TempData["message"] = "Verification link is sent to your e-mail.";
                return RedirectToAction("Signin");
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public IActionResult VerifyEmail(string token)
        {
            string plaintText = _tokenService.Decrypt(token);
            string email = TokenValidation.ParseTokenEmailVerify(plaintText, "email");
            string minit = TokenValidation.ParseTokenEmailVerify(plaintText, "minit");
            string expir = TokenValidation.ParseTokenEmailVerify(plaintText, "expir");
            var diffMinit = TimeConversion.DateDifferenceInMinutes(DateTime.Now, TimeConversion.UnixTimestampToDateTime(Convert.ToInt64(expir)));
            if(diffMinit > Convert.ToInt32(minit))
            {
                TempData["message"] = "Verification link expired.";
                return RedirectToAction("Signin");
            }
            CustomerRepository customerRepo = new CustomerRepository(_connectionString);
            CustomerVM oCustomer = customerRepo.GetByEmail(email);
            if (oCustomer == null)
            {
                TempData["message"] = "Email verification failed.";
                return RedirectToAction("Signin");
            }
            else
            {
                oCustomer.IsActive = true;
                customerRepo.UpdateEmailVerify(oCustomer);
                TempData["message"] = "Verified successfully. Sign in to check bills or write reviews.";
            }
            return RedirectToAction("Signin");
        }

        private string GetBaseUrl()
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/";
            return baseUrl;
        }

        [HttpPost]
        public async Task<IActionResult> RequestPasswordReset(string email)
        {
            try
            {
                CustomerRepository customerRepo = new CustomerRepository(_connectionString);
                CustomerVM oCustomer = customerRepo.GetByEmail(email);
                if (oCustomer == null)
                {
                    TempData["message"] = "Email not found.";
                    return RedirectToAction("Signin");
                }
                string plaintText = "email=" + email + "&minit=" + TokenValidation.VerifyEmailInMinutes + "&expir="+TimeConversion.DateTimeToUnixTimestamp(DateTime.Now);
                string token = _tokenService.Encrypt(plaintText);
                string verificationLink = $"{GetBaseUrl()}home/VerifyCustomerPass?token={token}";
                await _emailService.SendEmailAsync(
                email,
                "Password Reset",
                "<div>Dear " + oCustomer.FullName + ",</div>" +
                "<div>Verify your email to reset your password.</div>" +
                "<div>Please, click the link below.</div>" +
                "<a href='" + verificationLink + "' target='_blank'>Verify Aquasip</a>");
                TempData["message"] = "Password reset link is sent to your e-mail.";
                return RedirectToAction("Signin");
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        public IActionResult VerifyCustomerPass(string token)
        {
            string plaintText = _tokenService.Decrypt(token);
            string email = TokenValidation.ParseTokenEmailVerify(plaintText, "email");
            string minit = TokenValidation.ParseTokenEmailVerify(plaintText, "minit");
            string expir = TokenValidation.ParseTokenEmailVerify(plaintText, "expir");
            var diffMinit = TimeConversion.DateDifferenceInMinutes(DateTime.Now, TimeConversion.UnixTimestampToDateTime(Convert.ToInt64(expir)));
            if (diffMinit > Convert.ToInt32(minit))
            {
                TempData["message"] = "Verification link expired.";
                return RedirectToAction("Signin");
            }
            else
            {
                TempData["message"] = "e-mail veried successfully.";
                return RedirectToAction("PassReset", new { token = token });
            }
        }


        public IActionResult Signout(int? CustomerId)
        {
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("FullName");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SendIsHelpful([FromQuery]bool isHelpful, long reviewId, long CustomerId)
        {
            ReviewRepository reviewRepo = new ReviewRepository(_connectionString);
            ReviewVM review = reviewRepo.GetById(reviewId);
            if (review == null)
            {
                TempData["message"] = "Customer verification failed.";
                return RedirectToAction("Review");
            }
            else
            {
                ReviewVoteVM reviewVote = new ReviewVoteVM
                {
                    ReviewId = reviewId,
                    CustomerId = CustomerId,
                    IsHelpful = isHelpful
                };
                var isSave = reviewRepo.UpdateReviewVote(reviewVote);
                TempData["message"] = isSave == true ?  "Data saved successfully." : "Operation failed.";
            }
            return RedirectToAction("Review");
        }

        public IActionResult PassReset(string token)
        {
            #region Read
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var homePage = pageRepo.GetBySlug("home");
            homePage.PageContents = pageContentRepo.GetBySlugPage("home");

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(homePage);
            ViewData["aquasip"] = listPage;
            #endregion

            string plaintText = _tokenService.Decrypt(token);
            string email = TokenValidation.ParseTokenEmailVerify(plaintText, "email");
            string minit = TokenValidation.ParseTokenEmailVerify(plaintText, "minit");
            string expir = TokenValidation.ParseTokenEmailVerify(plaintText, "expir");
            var diffMinit = TimeConversion.DateDifferenceInMinutes(DateTime.Now, TimeConversion.UnixTimestampToDateTime(Convert.ToInt64(expir)));
            if (diffMinit > Convert.ToInt32(minit))
            {
                TempData["message"] = "Verification link expired.";
                return RedirectToAction("Signin");
            }
            CustomerRepository customerRepo = new CustomerRepository(_connectionString);
            CustomerVM oCustomer = customerRepo.GetByEmail(email);
            if (oCustomer == null)
            {
                TempData["message"] = "Email verification failed.";
                return RedirectToAction("Signin");
            }
            else
            {
                return View(oCustomer);
            }
        }

        [HttpPost]
        public IActionResult PassReset(CustomerVM model)
        {
            try
            {
                CustomerRepository customerRepo = new CustomerRepository(_connectionString);
                TempData["message"] = customerRepo.UpdatePassword(model);
                return RedirectToAction("Signin");
            }
            catch (Exception ex)
            {
                ErrorVM error = new ErrorVM(_environment);
                error.WriteLog(ex.StackTrace);
                TempData["message"] = "Exception!";
            }
            return RedirectToAction("Signup");
        }
        #endregion

    }
}
