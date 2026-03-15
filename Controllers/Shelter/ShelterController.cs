using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers.Shelter
{
    [ShelterAuthorize]
    public class ShelterController : Controller
    {
        private readonly RapDbContext _context;

        public ShelterController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult ShelterDash()
        {
            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (shelterId == null)
                return RedirectToAction("Login", "Authentication");

            var pets = _context.Pets.Where(p => p.shelter_id == shelterId).ToList();
            var adoptions = _context.AdoptionApplications.Where(a => a.shelter_id == shelterId).ToList();
            var payments = _context.Payments
                                    .Include(p => p.AdoptionApplications)
                                    .Where(p => p.AdoptionApplications.shelter_id == shelterId)
                                    .ToList();

            ViewBag.TotalPets = pets.Count;
            ViewBag.TotalAdoptions = adoptions.Count;
            ViewBag.PendingRequests = adoptions.Count(a => a.status.ToLower().Trim() == "pending");
            ViewBag.TotalRevenue = payments.Sum(p => p.amount);

            return View();
        }
    }
}