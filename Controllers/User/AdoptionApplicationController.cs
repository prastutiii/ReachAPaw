using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Filters;
using Microsoft.AspNetCore.Authorization;

namespace ReachAPaw.Controllers.User
{
    public class AdoptionApplicationController: Controller
    {
        private readonly RapDbContext _context;

        public AdoptionApplicationController(RapDbContext context)
        {
            _context = context;
        }

        [UserAuthorize]
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

        public IActionResult Application3(int petId)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.pet_id == petId);
            if (pet == null)
                return NotFound();

            return View(pet);
        }

        [HttpPost]
        public IActionResult SubmitApplication(
        int petId,
        string FullName,
        string Email,
        string Phone,
        string City,
        string Address,
        string HomeType,
        string Ownership,
        bool HasYard,
        bool HasChildren,
        bool CurrentlyHasPets,
        string PreviousExperience,
        string ReasonToAdopt,
        IFormFile IDProof)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.pet_id == petId);
            if (pet == null) return NotFound();

            // Save uploaded ID proof
            string fileName = null;
            if (IDProof != null && IDProof.Length > 0)
            {
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/documents");
                Directory.CreateDirectory(uploads);

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(IDProof.FileName);
                using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    IDProof.CopyTo(stream);
                }
            }

            // Generate new application_id manually
            int newApplicationId = (_context.ApplicationDetails.Any())
                ? _context.ApplicationDetails.Max(a => a.application_id) + 1
                : 1;

            // Insert ApplicationDetails
            var details = new ApplicationDetailsModel
            {
                application_id = newApplicationId,  // manually set
                full_name = FullName,
                email = Email,
                phone = Phone,
                city = City,
                address = Address,
                home_type = Enum.Parse<HomeType>(HomeType),
                own_or_rent = Enum.Parse<OwnOrRent>(Ownership),
                has_yard = HasYard,
                has_children = HasChildren,
                other_pets = CurrentlyHasPets,
                pet_experience = PreviousExperience,
                reason = ReasonToAdopt,
                valid_document = fileName
            };
            _context.ApplicationDetails.Add(details);
            _context.SaveChanges();

            // Insert AdoptionApplication using same PK as FK
            var application = new AdoptionApplicationModel
            {
                application_id = newApplicationId,  // FK to details
                status = "Pending",
                applied_date = DateTime.Now,
                user_id = 1,  
                shelter_id = pet.shelter_id,
                pet_id = petId
            };
            _context.AdoptionApplications.Add(application);
            _context.SaveChanges();

            // Redirect to Home
            return RedirectToAction("Home", "Home");
        }
    }
}
