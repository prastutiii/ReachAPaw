using Microsoft.AspNetCore.Mvc;

namespace ReachAPaw.Controllers.User
{
    public class HomeController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }

}
