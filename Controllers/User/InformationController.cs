using Microsoft.AspNetCore.Mvc;

namespace ReachAPaw.Controllers
{
    public class InformationController : Controller
    {
        public IActionResult HowToAdopt() => View();
        public IActionResult PrivacyPolicy() => View();
        public IActionResult Terms() => View();
        public IActionResult FAQ() => View();
    }
}