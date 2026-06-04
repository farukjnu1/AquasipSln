using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Aquasip.Repositories;
using Aquasip.Utilities;

namespace Aquasip.Fiters
{
    public class CustomerFilter : Attribute, IAuthorizationFilter //, IActionFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            #region Authentication
            string? CustomerId = context.HttpContext.Session.GetString("CustomerId");
            if (CustomerId != null)
            {
                if (Convert.ToInt64(CustomerId) > 0)
                {
                    // okay ahead
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
            #endregion
        }

    }
}
