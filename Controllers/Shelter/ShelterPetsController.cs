using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;
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

        public IActionResult ShelterPets()
        {
            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (shelterId == null)
                return RedirectToAction("Login", "Authentication");

            var pets = _context.Pets
                               .Where(p => p.shelter_id == shelterId)
                               .ToList();

            return View(pets);
        }

        [HttpGet]
        public IActionResult AddPets()
        {
            return View();
        }

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
            string fileName = null;
            if (pet_file != null && pet_file.Length > 0)
            {
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pets");
                Debug.WriteLine("Uploads folder: " + uploads);
                Directory.CreateDirectory(uploads);

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(pet_file.FileName);

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

            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (shelterId == null)
                return RedirectToAction("Login", "Authentication");

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
                shelter_id = shelterId.Value
            };

            _context.Pets.Add(pet);
            _context.SaveChanges();

            return RedirectToAction("ShelterPets", "ShelterPets");
        }

        public IActionResult ViewPets(int id)
        {
            var pet = _context.Pets
                      .Include(p => p.Shelters)
                      .FirstOrDefault(p => p.pet_id == id);

            if (pet == null)
                return NotFound();

            return View(pet);
        }

        [HttpGet]
        public IActionResult EditPet(int id)
        {
            System.Diagnostics.Debug.WriteLine($"EditPet called with id: {id}");

            var pet = _context.Pets.Find(id);

            System.Diagnostics.Debug.WriteLine($"Pet found: {pet?.pet_name ?? "NULL"}");

            if (pet == null)
                return NotFound();

            return View(pet);
        }

        [HttpPost]
        public IActionResult EditPet(int id,
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
            var pet = _context.Pets.Find(id);
            if (pet == null)
                return NotFound();

            if (pet_file != null && pet_file.Length > 0)
            {
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pets");
                Directory.CreateDirectory(uploads);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(pet_file.FileName);
                using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    pet_file.CopyTo(stream);
                }
                pet.image_url = "/images/pets/" + fileName;
            }

            pet.pet_name = pet_name;
            pet.species = species;
            pet.gender = gender;
            pet.age = age;
            pet.location = location;
            pet.description = description;
            pet.ideal_home = ideal_home;
            pet.health_status = health_status;
            pet.is_vaccinated = Convert.ToBoolean(is_vaccinated);
            pet.is_neutered = Convert.ToBoolean(is_neutered);
            pet.is_microchipped = Convert.ToBoolean(is_microchipped);
            pet.status = status;
            pet.fee = fee;

            _context.SaveChanges();

            return RedirectToAction("ShelterPets");
        }

        public IActionResult DeletePet(int id)
        {
            var pet = _context.Pets.Find(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                _context.SaveChanges();
            }
            return RedirectToAction("ShelterPets");
        }

    }
}