using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Aquasip.Repositories;
using Aquasip.Utilities;

namespace Aquasip.Fiters
{
    public class AdminFilter : Attribute, IAuthorizationFilter //, IActionFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            #region Authentication
            int? UserId = context.HttpContext.Session.GetInt32("UserID");
            if (UserId != null)
            {
                if (UserId > 0)
                {
                    #region Authorization
                    //context.Result = new RedirectToActionResult("Index", "Pages", null);
                    //var Action = context.ActionDescriptor.RouteValues["Action"];
                    string? controller = context.ActionDescriptor.RouteValues["Controller"];
                    var roleRepo = new RoleRepository(WebsiteConfig.ConnectionString);
                    var userRepo = new UserRepository(WebsiteConfig.ConnectionString);
                    var oUser = userRepo.GetById((int)UserId);
                    if (oUser != null)
                    {
                        if (oUser.RoleId > 0)
                        {
                            var oRolePermission = roleRepo.GetPermissionByRole(oUser.RoleId, controller == null ? string.Empty : controller).FirstOrDefault();
                            if (oRolePermission != null)
                            {
                                // okay ahead
                            }
                            else
                            {
                                context.Result = new RedirectToActionResult("Index", "Login", null);
                            }
                        }
                        else
                        {
                            context.Result = new RedirectToActionResult("Index", "Login", null);
                        }
                    }
                    #endregion
                }
                else
                {
                    context.Result = new RedirectToActionResult("Index", "Login", null);
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
            #endregion
        }

        /*public void OnActionExecuting(ActionExecutingContext context)
        {
            var Action = context.ActionDescriptor.RouteValues["Action"];
            var Controller = context.RouteData.Values["Controller"];
            Console.WriteLine("Before action executes");
        }*/

        /*public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("After action executed");
        }*/

    }
}
