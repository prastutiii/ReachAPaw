using Microsoft.AspNetCore.Mvc;

namespace ReachAPaw.Controllers
{
    public class ShelterController : Controller
    {
        public IActionResult ShelterDash()
        {
            return View();
        }
    }
}
