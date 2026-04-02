using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;

namespace ReachAPaw.Controllers.User
{
    public class HomeController : Controller
    {
        private readonly RapDbContext _context;

        public HomeController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult Home()
        {
            //Pets waiting
            var availablePets = _context.Pets
                .Where(p => p.status.ToLower().Trim() == "available")
                .ToList();

            ViewBag.AvailablePetsCount = availablePets.Count;

            //Pets adopted
            var adoptedCount = _context.Adoptions.Count();
            ViewBag.AdoptedCount = adoptedCount;

            //Happy families
            ViewBag.HappyFamilies = adoptedCount;

            //Shelters
            ViewBag.ShelterCount = _context.Shelters.Count();

            //Users
            ViewBag.UserCount = _context.Users.Count();

            //Featured pets
            var featuredPets = _context.Pets
                .Where(p => p.status.ToLower().Trim() == "available")
                .OrderByDescending(p => p.pet_id)
                .Take(3)
                .ToList();

            ViewBag.FeaturedPets = featuredPets;

            //Events
            var events = _context.Community
            .Include(c => c.Categories)
            .Where(c => c.category_id == 4)
            .OrderByDescending(c => c.created_at)
            .Take(3)
            .ToList();

            ViewBag.Events = events;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }

}
