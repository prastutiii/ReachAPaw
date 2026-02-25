using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers.Shelter
{
    [ShelterAuthorize]
    public class ShelterController : Controller
    {
        public IActionResult ShelterDash()
        {
            return View();
        }

    }
}
