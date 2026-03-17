using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers
{
    [ShelterAuthorize]
    public class ShelterVisitsController : Controller
    {
        private readonly RapDbContext _context;

        public ShelterVisitsController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult ShelterVisits()
        {
            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (shelterId == null)
                return RedirectToAction("Login", "Authentication");

            var visits = _context.Visits
                                 .Where(v => v.shelter_id == shelterId)
                                 .OrderByDescending(v => v.date)
                                 .ToList();

            return View(visits);
        }

        public IActionResult CompleteVisit(int id)
        {
            var visit = _context.Visits.Find(id);
            if (visit != null)
            {
                visit.status = "Completed";
                _context.SaveChanges();
            }
            return RedirectToAction("ShelterVisits");
        }

        public IActionResult CancelVisit(int id)
        {
            var visit = _context.Visits.Find(id);
            if (visit != null)
            {
                visit.status = "Cancelled";
                _context.SaveChanges();
            }
            return RedirectToAction("ShelterVisits");
        }
    }
}