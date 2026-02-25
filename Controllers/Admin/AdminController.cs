using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers.Admin
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        public IActionResult AdminDash()
        {
            return View();
        }
    }
}
