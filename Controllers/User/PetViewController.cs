using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers.User
{
    public class PetViewController : Controller
    {
        private readonly RapDbContext _context;

        // Dependency Injection for your Database Context
        public PetViewController(RapDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PetView(int id)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.pet_id == id);
            if (pet == null) return NotFound();

            return View(pet);
        }

    }
}
