using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers.User
{
    public class AdoptionApplicationController: Controller
    {
        private readonly RapDbContext _context;

        // Dependency Injection for your Database Context
        public AdoptionApplicationController(RapDbContext context)
        {
            _context = context;
        }
        public IActionResult Application1(int petId)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.pet_id == petId);
            if (pet == null)
                return NotFound();

            return View(pet); 
        }

        public IActionResult Application2(int petId)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.pet_id == petId);
            if (pet == null)
                return NotFound();

            return View(pet);
        }
    }
}
