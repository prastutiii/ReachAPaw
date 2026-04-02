using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{

    public class AdoptController : Controller
    {
        private readonly RapDbContext _context;

        // Dependency Injection for your Database Context
        public AdoptController(RapDbContext context)
        {
            _context = context;
        }

        // GET: /Pet/Adopt
        public async Task<IActionResult> Adopt(string search)
        {
            var petsQuery = _context.Pets
                .Where(p => p.status.ToLower().Trim() == "available");

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                petsQuery = petsQuery.Where(p =>
                    p.pet_name.ToLower().Contains(search) ||
                    (p.species != null && p.species.ToLower().Contains(search)) ||
                    (p.location != null && p.location.ToLower().Contains(search)) ||
                    (p.gender != null && p.gender.ToLower().Contains(search)) ||
                    (p.age != null && p.age.ToLower().Contains(search))
                );
            }

            var pets = await petsQuery.ToListAsync();
            return View(pets);
        }
    }
}