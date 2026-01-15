using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Aquasip.Repositories;

namespace Aquasip.Fiters
{
    public class WebsiteFilter : Attribute , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var Action = context.ActionDescriptor.RouteValues["Action"];
            var Controller = context.RouteData.Values["Controller"];
            string urL = "/" + Controller + "/" + Action;
            urL = (Convert.ToString(Controller) == "Home" && Convert.ToString(Action) == "Index") ? "/" : "/" + Controller + "/" + Action;
            context.HttpContext.Session.SetString("urL", urL);
        }

    }
}
