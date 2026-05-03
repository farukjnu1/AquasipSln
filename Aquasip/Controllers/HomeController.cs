using Aquasip.EF;
using Aquasip.Fiters;
using Aquasip.Models;
using Aquasip.Repositories;
using Aquasip.Utilities;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Aquasip.Controllers
{
    [WebsiteFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("AquasipContext");
            _environment = environment;
        }
        #region Requirement from Client
        public IActionResult Index()
        {
            PageRepository pageRepo = new PageRepository(_connectionString);
            PageContentRepository pageContentRepo = new PageContentRepository(_connectionString);

            var homePage = pageRepo.GetBySlug("home");
            homePage.PageContents = pageContentRepo.GetBySlugPage("home");

            homePage.Products = new ProductRepository(_connectionString).GetAll();

            //var servicesPage = pageRepo.GetBySlug("services");
            //servicesPage.PageContents = pageContentRepo.GetBySlugPage("services");

            //var aboutPage = pageRepo.GetBySlug("about");
            //aboutPage.PageContents = pageContentRepo.GetBySlugPage("about");

            //var ourTeamPage = pageRepo.GetBySlug("our_team");
            //ourTeamPage.PageContents = pageContentRepo.GetBySlugPage("our_team");

            //var testimonialPage = pageRepo.GetBySlug("testimonial");
            //testimonialPage.PageContents = pageContentRepo.GetBySlugPage("testimonial");

            //var ourBlogPage = pageRepo.GetBySlug("our_blog");
            //ourBlogPage.PageContents = pageContentRepo.GetBySlugPage("our_blog");

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var siteSettingRepo = new SiteSettingRepository(_connectionString);
            layoutPage.SiteSettings = siteSettingRepo.GetAll();

            var listPage = new List<PageVM>();
            listPage.Add(homePage);
            //listPage.Add(servicesPage);
            //listPage.Add(aboutPage);
            //listPage.Add(appointmentPage);
            //listPage.Add(ourTeamPage);
            //listPage.Add(testimonialPage);
            //listPage.Add(ourBlogPage);
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

            //var servicesPage = pageRepo.GetBySlug("services");
            //servicesPage.PageContents = pageContentRepo.GetBySlugPage("services");

            //var aboutPage = pageRepo.GetBySlug("about");
            //aboutPage.PageContents = pageContentRepo.GetBySlugPage("about");

            //var ourTeamPage = pageRepo.GetBySlug("our_team");
            //ourTeamPage.PageContents = pageContentRepo.GetBySlugPage("our_team");

            //var testimonialPage = pageRepo.GetBySlug("testimonial");
            //testimonialPage.PageContents = pageContentRepo.GetBySlugPage("testimonial");

            //var ourBlogPage = pageRepo.GetBySlug("our_blog");
            //ourBlogPage.PageContents = pageContentRepo.GetBySlugPage("our_blog");

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var siteSettingRepo = new SiteSettingRepository(_connectionString);
            layoutPage.SiteSettings = siteSettingRepo.GetAll();

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(productsPage);
            ViewData["aquasip"] = listPage;

            //listPage.Add(servicesPage);
            //listPage.Add(aboutPage);
            //listPage.Add(appointmentPage);
            //listPage.Add(ourTeamPage);
            //listPage.Add(testimonialPage);
            //listPage.Add(ourBlogPage);
            //return View(listPage);

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

            #region Create
            //ContactMessageVM model = new ContactMessageVM();
            #endregion

            //return View(model);
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

            galleryPage.Products = new ProductRepository(_connectionString).GetAll();

            var layoutPage = pageRepo.GetBySlug("layout");
            layoutPage.PageContents = pageContentRepo.GetBySlugPage("layout");

            var siteSettingRepo = new SiteSettingRepository(_connectionString);
            layoutPage.SiteSettings = siteSettingRepo.GetAll();

            var listPage = new List<PageVM>();
            listPage.Add(layoutPage);
            listPage.Add(galleryPage);
            ViewData["aquasip"] = listPage;

            //listPage.Add(servicesPage);
            //listPage.Add(aboutPage);
            //listPage.Add(appointmentPage);
            //listPage.Add(ourTeamPage);
            //listPage.Add(testimonialPage);
            //listPage.Add(ourBlogPage);
            //return View(listPage);

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
                listProduct.Add(new ProductVM
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity,
                    Total = product.Price * quantity,
                    ListProductMedia = product.ListProductMedia
                });
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

        public IActionResult Review()
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

            //return View(model);
            return View();
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
    }
}
