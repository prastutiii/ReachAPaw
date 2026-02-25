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
        public async Task<IActionResult> Adopt()
        {
            // Fetch the list of pets from the database
            var pets = await _context.Pets.ToListAsync();

            // Pass the list (IEnumerable) to the View
            return View(pets);
        }
    }
}