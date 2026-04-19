using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{
    [UserAuthorize]
    public class PaymentController : Controller
    {
        private readonly RapDbContext _context;

        public PaymentController(RapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CompleteAdoption(int id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var adoption = _context.AdoptionApplications
                                   .Include(a => a.Pets)
                                   .ThenInclude(p => p.Shelters)
                                   .FirstOrDefault(a => a.adoption_id == id && a.user_id == userId);

            if (adoption == null)
                return NotFound();

            return View(adoption);
        }

        public IActionResult CancelAdoption(int id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var adoption = _context.AdoptionApplications
                                   .FirstOrDefault(a => a.adoption_id == id && a.user_id == userId);

            if (adoption != null)
            {
                adoption.status = "Cancelled";

                var pet = _context.Pets.Find(adoption.pet_id);
                if (pet != null)
                    pet.status = "Available";

                _context.SaveChanges();
            }

            return RedirectToAction("MyAdoptions", "MyAdoptions");
        }

        [HttpPost]
        public IActionResult CompleteAdoption(int id, string method, string card_number, string card_name, string expiry, string cvv)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var adoption = _context.AdoptionApplications
                                   .Include(a => a.Pets)
                                   .FirstOrDefault(a => a.adoption_id == id && a.user_id == userId);

            if (adoption == null)
                return NotFound();

            var fee = double.TryParse(adoption.Pets?.fee, out var f) ? f : 0;
            var total = fee + 250 + 100;

            var payment = new PaymentModel
            {
                amount = total,
                method = method,
                date = DateTime.Now,
                adoption_id = id
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();

            var certificateNumber = $"PAW-{DateTime.Now.Year}-{id:D5}";
            var adoptionRecord = new AdoptionModel
            {
                date = DateTime.Now,
                certificate_number = certificateNumber,
                application_id = adoption.application_id,
                payment_id = payment.payment_id
            };
            _context.Adoptions.Add(adoptionRecord);

            adoption.status = "Completed";

            var pet = _context.Pets.Find(adoption.pet_id);
            if (pet != null)
                pet.status = "Adopted";

            _context.SaveChanges();

            return RedirectToAction("Certificate", new { id });
        }

        public IActionResult Certificate(int id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var adoption = _context.AdoptionApplications
                                   .Include(a => a.Pets)
                                   .ThenInclude(p => p.Shelters)
                                   .Include(a => a.Users)
                                   .FirstOrDefault(a => a.adoption_id == id && a.user_id == userId);

            if (adoption == null)
                return NotFound();

            var adoptionRecord = _context.Adoptions
                                         .FirstOrDefault(a => a.application_id == adoption.application_id);

            ViewBag.CertificateNumber = adoptionRecord?.certificate_number ?? $"PAW-{DateTime.Now.Year}-{id:D5}";
            ViewBag.AdoptionDate = adoptionRecord?.date ?? DateTime.Now;

            return View(adoption);
        }
    }
}