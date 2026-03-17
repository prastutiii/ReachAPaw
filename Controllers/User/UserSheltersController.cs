using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Data;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{
    public class UserSheltersController : Controller
    {
        private readonly RapDbContext _context;

        public UserSheltersController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult UserShelters()
        {
            var shelters = _context.Shelters
                                   .Where(s => s.status == "Active")
                                   .ToList();

            return View(shelters);
        }

        [HttpPost]
        public IActionResult ScheduleVisit(int shelter_id, string name, string phone, string date, string time, string reason, string people_no)
        {
            var visit = new VisitModel
            {
                shelter_id = shelter_id,
                name = name,
                phone = phone,
                date = DateTime.Parse(date),
                time = TimeSpan.Parse(time),
                reason = reason,
                people_no = people_no,
                status = "Scheduled"
            };

            _context.Visits.Add(visit);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}