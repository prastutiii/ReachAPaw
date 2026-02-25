using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace ReachAPaw.Filters
{
    public class ShelterAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Get the role of the currently logged-in user from session
            var role = context.HttpContext.Session.GetString("user_role")?.Trim().ToLower();

            // If the role is not admin, redirect to login page
            if (role != "shelter")
            {
                context.Result = new RedirectToActionResult("Login", "Authentication", null);
            }

            base.OnActionExecuting(context);
        }
    }
}