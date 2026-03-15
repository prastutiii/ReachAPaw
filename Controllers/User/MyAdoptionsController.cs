using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{
    [UserAuthorize]
    public class MyAdoptionsController : Controller
    {
        private readonly RapDbContext _context;

        public MyAdoptionsController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult MyAdoptions()
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var adoptions = _context.AdoptionApplications
                                    .Include(a => a.Pets)
                                    .ThenInclude(p => p.Shelters)
                                    .Where(a => a.user_id == userId)
                                    .OrderByDescending(a => a.applied_date)
                                    .ToList();

            ViewBag.Total = adoptions.Count;
            ViewBag.Pending = adoptions.Count(a => a.status.ToLower().Trim() == "pending");
            ViewBag.Approved = adoptions.Count(a => a.status.ToLower().Trim() == "approved");
            ViewBag.Rejected = adoptions.Count(a => a.status.ToLower().Trim() == "rejected");
            ViewBag.Completed = adoptions.Count(a => a.status.ToLower().Trim() == "completed");
            ViewBag.Cancelled = adoptions.Count(a => a.status.ToLower().Trim() == "cancelled");
            ViewBag.Adopted = adoptions.Count(a => a.status.ToLower().Trim() == "adopted");

            return View(adoptions);
        }

        public IActionResult ViewMyAdoptions(int id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var adoption = _context.AdoptionApplications
                                   .Include(a => a.ApplicationDetails)
                                   .Include(a => a.Pets)
                                   .ThenInclude(p => p.Shelters)
                                   .Include(a => a.Users)
                                   .FirstOrDefault(a => a.adoption_id == id && a.user_id == userId);

            if (adoption == null)
                return NotFound();

            return View(adoption);
        }
    }
}