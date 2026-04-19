using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;
using Microsoft.AspNetCore.Identity;

namespace ReachAPaw.Controllers
{
    [ShelterAuthorize]
    public class ShelterProfileController : Controller
    {
        private readonly RapDbContext _context;

        public ShelterProfileController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult ShelterProfile()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (userId == null || shelterId == null)
                return RedirectToAction("Login", "Authentication");

            var user = _context.Users.Find(userId);
            var shelter = _context.Shelters.Find(shelterId);

            if (user == null || shelter == null)
                return NotFound();

            ViewBag.User = user;
            ViewBag.Shelter = shelter;

            return View();
        }

        [HttpPost]
        public IActionResult ShelterProfile(
            string username,
            string login_email,
            string password,
            string shelter_name,
            string display_email,
            string phone,
            string website,
            string hours,
            string pan_number,
            string description,
            string address,
            string city,
            IFormFile shelter_img)
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            var shelterId = HttpContext.Session.GetInt32("shelter_id");

            if (userId == null || shelterId == null)
                return RedirectToAction("Login", "Authentication");

            var user = _context.Users.Find(userId);
            var shelter = _context.Shelters.Find(shelterId);

            if (user == null || shelter == null)
                return NotFound();

            // update user
            user.username = username;
            user.email = login_email;
            if (!string.IsNullOrEmpty(password))
            {
                var hasher = new PasswordHasher<UserModel>();
                user.password = hasher.HashPassword(null, password);
            }

            // update shelter
            shelter.shelter_name = shelter_name;
            shelter.email = display_email;
            shelter.phone = phone;
            shelter.website = website;
            shelter.hours = hours;
            shelter.pan_number = pan_number;
            shelter.description = description;
            shelter.address = address;
            shelter.city = city;

            // update shelter image
            if (shelter_img != null && shelter_img.Length > 0)
            {
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/shelters");
                Directory.CreateDirectory(uploads);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(shelter_img.FileName);
                using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                {
                    shelter_img.CopyTo(stream);
                }
                shelter.shelter_img = "/images/shelters/" + fileName;
            }

            _context.SaveChanges();

            HttpContext.Session.SetString("user_name", username);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("ShelterProfile");
        }
    }
}