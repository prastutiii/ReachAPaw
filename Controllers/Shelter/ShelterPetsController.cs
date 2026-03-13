using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Filters;
using System.Diagnostics;

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

        // GET: AddPets
        public IActionResult AddPets()
        {
            return View();
        }

        // POST: AddPets
        [HttpPost]

        [HttpPost]
        public IActionResult AddPets(
        string pet_name,
        string species,
        string gender,
        string age,
        string location,
        string description,
        string ideal_home,
        string health_status,
        string is_vaccinated,
        string is_neutered,
        string is_microchipped,
        string status,
        string fee,
        IFormFile pet_file)
        {
            // Save uploaded pet image
            string fileName = null;
            if (pet_file != null && pet_file.Length > 0)
            {
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pets");
                Debug.WriteLine("Uploads folder: " + uploads);
                Directory.CreateDirectory(uploads);

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(pet_file.FileName);
                using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    pet_file.CopyTo(stream);
                }

                try
                {
                    using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        pet_file.CopyTo(stream);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception while saving file: " + ex.Message);
                }

                fileName = "/images/pets/" + fileName;

            }

            // Map form data to PetModel
            var pet = new PetModel
            {
                pet_name = pet_name,
                species = species,
                gender = gender,
                age = age,
                location = location,
                description = description,
                ideal_home = ideal_home,
                health_status = health_status,
                is_vaccinated = Convert.ToBoolean(is_vaccinated),
                is_neutered = Convert.ToBoolean(is_neutered),
                is_microchipped = Convert.ToBoolean(is_microchipped),
                fee = fee,
                status = status,
                image_url = fileName,
                shelter_id = 1
            };

            // Save to database
            _context.Pets.Add(pet);
            _context.SaveChanges();


            // Redirect to pet list or home
            return RedirectToAction("ShelterPets", "ShelterPets");

            
        }

        // GET: ShelterPets
        public IActionResult ShelterPets()
        {
            var pets = _context.Pets.ToList();
            return View(pets);
        }
    }
}