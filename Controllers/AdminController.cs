using Microsoft.AspNetCore.Mvc;

namespace ReachAPaw.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDash()
        {
            return View();
        }
    }
}
