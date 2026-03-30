using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers.Admin
{
    [AdminAuthorize]
    public class AdminAdoptionsController : Controller
    {
        private readonly RapDbContext _context;

        public AdminAdoptionsController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminAdoptions()
        {
            var adoptions = _context.AdoptionApplications
                                   .Include(a => a.Users)
                                   .Include(a => a.Pets)
                                   .Include(a => a.Shelters)
                                   .OrderByDescending(a => a.applied_date)
                                   .ToList();

            return View(adoptions);
        }

        public IActionResult ViewAdoptions(int id)
        {
            var adoption = _context.AdoptionApplications
                                  .Include(a => a.Users)
                                  .Include(a => a.Pets)
                                  .Include(a => a.Shelters)
                                  .Include(a => a.ApplicationDetails)
                                  .FirstOrDefault(a => a.application_id == id);

            if (adoption == null)
                return NotFound();

            return View(adoption);
        }
    }
}

