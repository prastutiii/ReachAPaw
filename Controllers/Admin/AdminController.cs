using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Data;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private readonly RapDbContext _context;

        public AdminController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminDash()
        {
            ViewBag.TotalUsers = _context.Users.Count(u => u.role == "user");
            ViewBag.TotalPets = _context.Pets.Count();
            ViewBag.TotalShelters = _context.Shelters.Count();
            ViewBag.TotalAdoptions = _context.AdoptionApplications.Count();

            // Monthly Adoptions
            ViewBag.MonthlyAdoptions = _context.AdoptionApplications
                .GroupBy(a => a.applied_date.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToList();

            // Adoptions by Status
            ViewBag.AdoptionsByStatus = _context.AdoptionApplications
                .GroupBy(a => a.status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Pets by Species
            ViewBag.PetsBySpecies = _context.Pets
                .GroupBy(p => p.species)
                .Select(g => new { Species = g.Key, Count = g.Count() })
                .ToList();

            // Pets by Status
            ViewBag.PetsByStatus = _context.Pets
                .GroupBy(p => p.status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            return View();
        }
    }
}