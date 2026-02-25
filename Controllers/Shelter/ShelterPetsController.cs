using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers
{
    [ShelterAuthorize]
    public class ShelterPetsController : Controller
    {
        private readonly RapDbContext _context;

        public ShelterPetsController(RapDbContext context)
        {
            _context = context;
        }

        // GET: Pets/AddPets
        public IActionResult AddPets()
        {
            return View();
        }

        // POST: Pets/AddPets
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPets(PetModel pet, IFormFile pet_file)
        {
            // Set shelter_id from logged-in user (example)
            pet.shelter_id = 1;

            // Handle uploaded file
            if (pet_file != null && pet_file.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pets");
                Directory.CreateDirectory(uploadsFolder); // create folder if it doesn't exist

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(pet_file.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    pet_file.CopyTo(fs);
                }

                // Save relative URL to DB
                pet.image_url = "/images/pets" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Pets.Add(pet);
                _context.SaveChanges();
                return RedirectToAction("ShelterPets");
            }

            return View(pet);
        }

        // GET: Pets/ShelterPets
        public IActionResult ShelterPets()
        {
            var pets = _context.Pets.ToList();
            return View(pets);
        }
    }
}