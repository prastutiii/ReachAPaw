using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{
    [ShelterAuthorize]
    public class ShelterAdoptionsController : Controller
    {
        private readonly RapDbContext _context;

        public ShelterAdoptionsController(RapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ShelterAdoptions()
        {
            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (shelterId == null)
                return RedirectToAction("Login", "Authentication");

            var adoptions = _context.AdoptionApplications
                                    .Include(a => a.ApplicationDetails)
                                    .Include(a => a.Pets)
                                    .Include(a => a.Users)
                                    .Where(a => a.shelter_id == shelterId)
                                    .OrderByDescending(a => a.applied_date)
                                    .ToList();

            return View(adoptions);
        }

        public IActionResult ViewAdoptions(int id)
        {
            var adoption = _context.AdoptionApplications
                                   .Include(a => a.ApplicationDetails)
                                   .Include(a => a.Pets)
                                   .Include(a => a.Users)
                                   .FirstOrDefault(a => a.adoption_id == id);

            if (adoption == null)
                return NotFound();


            return View(adoption);
        }

        public IActionResult ApproveAdoption(int id)
        {
            var adoption = _context.AdoptionApplications.Find(id);
            if (adoption != null)
            {
                adoption.status = "Approved";

                var pet = _context.Pets.Find(adoption.pet_id);
                if (pet != null)
                {
                    pet.status = "Pending";
                }

                _context.Notifications.Add(new NotificationModel
                {
                    message = $"Your adoption application for {pet?.pet_name} has been Approved!",
                    is_read = false,
                    created_at = DateTime.Now,
                    user_id = adoption.user_id
                });

                _context.SaveChanges();
            }
            return RedirectToAction("ViewAdoptions", new { id });
        }

        public IActionResult RejectAdoption(int id)
        {
            var adoption = _context.AdoptionApplications.Find(id);
            if (adoption != null)
            {
                adoption.status = "Rejected";

                var pet = _context.Pets.Find(adoption.pet_id);
                if (pet != null) pet.status = "Available";

                _context.Notifications.Add(new NotificationModel
                {
                    message = $"Your adoption application for {pet?.pet_name} has been Rejected.",
                    is_read = false,
                    created_at = DateTime.Now,
                    user_id = adoption.user_id
                });

                _context.SaveChanges();
            }
            return RedirectToAction("ViewAdoptions", new { id });
        }

        public IActionResult MarkAdopted(int id)
        {
            var adoption = _context.AdoptionApplications.Find(id);
            if (adoption != null)
            {
                adoption.status = "Adopted";

                var pet = _context.Pets.Find(adoption.pet_id);
                if (pet != null) pet.status = "Adopted";

                _context.Notifications.Add(new NotificationModel
                {
                    message = $"Congratulations! You have successfully Adopted {pet?.pet_name}.",
                    is_read = false,
                    created_at = DateTime.Now,
                    user_id = adoption.user_id
                });

                _context.SaveChanges();
            }
            return RedirectToAction("ViewAdoptions", new { id });
        }

        public IActionResult CancelAdoption(int id)
        {
            var adoption = _context.AdoptionApplications.Find(id);
            if (adoption != null)
            {
                adoption.status = "Cancelled";
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAdoptions", new { id });
        }
    }
}