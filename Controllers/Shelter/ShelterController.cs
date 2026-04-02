using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using System.Linq;

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
            var visits = _context.Visits.Where(v => v.shelter_id == shelterId).ToList();

            ViewBag.TotalPets = pets.Count;
            ViewBag.TotalAdoptions = adoptions.Count;
            ViewBag.PendingRequests = adoptions.Count(a => a.status.ToLower().Trim() == "pending");
            ViewBag.TotalRevenue = payments.Sum(p => p.amount);

            // Daily Revenue (Last 30 days)
            var dailyRevenue = payments
                .Where(p => p.date >= DateTime.Now.AddDays(-30))
                .GroupBy(p => p.date.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(p => p.amount) })
                .OrderBy(x => x.Date)
                .ToList();

            ViewBag.DailyRevenueLabels = dailyRevenue.Select(x => x.Date.ToString("MMM dd")).ToList();
            ViewBag.DailyRevenueData = dailyRevenue.Select(x => x.Amount).ToList();

            // Pet Categories
            var petCategories = pets
                .GroupBy(p => p.species)
                .Select(g => new { Species = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.PetCategoryLabels = petCategories.Select(x => x.Species ?? "Unknown").ToList();
            ViewBag.PetCategoryData = petCategories.Select(x => x.Count).ToList();

            // Adoption Status
            var adoptionStatus = adoptions
                .GroupBy(a => a.status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.AdoptionStatusLabels = adoptionStatus.Select(x => x.Status ?? "Unknown").ToList();
            ViewBag.AdoptionStatusData = adoptionStatus.Select(x => x.Count).ToList();

            // Scheduled Visits (Last 30 days)
            var scheduledVisits = visits
                .Where(v => v.date.HasValue && v.date >= DateTime.Now.AddDays(-30))
                .GroupBy(v => v.date.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            ViewBag.VisitLabels = scheduledVisits.Select(x => x.Date.ToString("MMM dd")).ToList();
            ViewBag.VisitData = scheduledVisits.Select(x => x.Count).ToList();

            return View();
        }
    }
}
