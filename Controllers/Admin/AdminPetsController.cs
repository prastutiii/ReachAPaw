using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers
{
    [AdminAuthorize]
    public class AdminPetsController : Controller
    {
        private readonly RapDbContext _context;

        public AdminPetsController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminPets()
        {
            var pets = _context.Pets
                               .Include(p => p.Shelters)
                               .OrderByDescending(p => p.pet_id)
                               .ToList();

            return View(pets);
        }

        public IActionResult ViewPet(int id)
        {
            var pet = _context.Pets
                              .Include(p => p.Shelters)
                              .FirstOrDefault(p => p.pet_id == id);

            if (pet == null)
                return NotFound();

            return View(pet);
        }
    }
}