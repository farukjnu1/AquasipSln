using Aquasip.Models;
using Aquasip.Repositories;
using Aquasip.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aquasip.Fiters
{
    public class CustomerFilter : Attribute, IAuthorizationFilter //, IActionFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            #region Authentication
            var customer = context.HttpContext.Session.GetObject<CustomerVM>("Customer");
            long CustomerId = customer == null ? 0 : customer.CustomerId;
            //string? CustomerId = context.HttpContext.Session.GetString("CustomerId");
            if (Convert.ToInt64(CustomerId) > 0)
            {
                // okay ahead
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
            #endregion
        }

    }
}
