using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Filters;
using System.Diagnostics;

namespace ReachAPaw.Controllers.Admin
{
    [AdminAuthorize]
    public class AdminSheltersController : Controller
    {
        private readonly RapDbContext _context;

        public AdminSheltersController(RapDbContext context)
        {
            _context = context;
        }

        // GET: Admin Shelters List
        public IActionResult AdminShelters()
        {
            var shelters = _context.Shelters
                                   .Include(s => s.Pets)
                                   .OrderByDescending(s => s.shelter_id)
                                   .ToList();

            return View(shelters);
        }

        // GET: View Shelter Details
        public IActionResult ViewShelter(int id)
        {
            var shelter = _context.Shelters
                                  .Include(s => s.Pets)
                                  .Include(s => s.Users)
                                  .FirstOrDefault(s => s.shelter_id == id);

            if (shelter == null)
                return NotFound();

            return View(shelter);
        }

        // GET: Add Shelter Page
        public IActionResult AddShelter()
        {
            return View();
        }

        // POST: Add Shelter
        [HttpPost]
        public IActionResult AddShelter(
            string shelter_name,
            string email,
            string phone,
            string address,
            string city,
            string pan_number,
            string website,
            string hours,
            string description,
            string status,
            IFormFile shelterImage,
            string username,
            string password,
            string userEmail,
            string userPhone,
            string userAddress,
            IFormFile userImage)
        {
            Debug.WriteLine($"=== AddShelter POST called ===");
            Debug.WriteLine($"Shelter Name: {shelter_name}");
            Debug.WriteLine($"Username: {username}");
            Debug.WriteLine($"Password: {password}");

            try
            {
                // Handle shelter image upload
                string shelterFileName = null;
                if (shelterImage != null && shelterImage.Length > 0)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/shelters");
                    Debug.WriteLine("Uploads folder: " + uploads);
                    Directory.CreateDirectory(uploads);

                    shelterFileName = Guid.NewGuid().ToString() + Path.GetExtension(shelterImage.FileName);

                    try
                    {
                        using (var stream = new FileStream(Path.Combine(uploads, shelterFileName), FileMode.Create))
                        {
                            shelterImage.CopyTo(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception while saving shelter image: " + ex.Message);
                    }

                    shelterFileName = "/images/shelters/" + shelterFileName;
                }

                // Create user first
                int userId = 0;
                string userFileName = null;
                if (userImage != null && userImage.Length > 0)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                    Debug.WriteLine("User Uploads folder: " + uploads);
                    Directory.CreateDirectory(uploads);

                    userFileName = Guid.NewGuid().ToString() + Path.GetExtension(userImage.FileName);

                    try
                    {
                        using (var stream = new FileStream(Path.Combine(uploads, userFileName), FileMode.Create))
                        {
                            userImage.CopyTo(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception while saving user image: " + ex.Message);
                    }

                    userFileName = "/images/users/" + userFileName;
                }

                var user = new UserModel
                {
                    username = username,
                    password = password,
                    email = userEmail,
                    phone = userPhone,
                    address = userAddress,
                    image_url = userFileName,
                    role = "shelter",
                    status = "active"
                };

                Debug.WriteLine($"Adding user: {user.username}");
                _context.Users.Add(user);
                _context.SaveChanges();
                userId = user.user_id;
                Debug.WriteLine($"User created with ID: {userId}");

                // Create shelter
                var shelter = new ShelterModel
                {
                    shelter_name = shelter_name,
                    email = email,
                    phone = phone,
                    address = address,
                    city = city,
                    pan_number = pan_number,
                    website = website,
                    hours = hours,
                    description = description,
                    status = string.IsNullOrEmpty(status) ? "active" : status,
                    shelter_img = shelterFileName,
                    user_id = userId,
                    Users = user
                };

                Debug.WriteLine($"Adding shelter: {shelter.shelter_name}");
                _context.Shelters.Add(shelter);
                _context.SaveChanges();
                Debug.WriteLine($"Shelter created successfully with ID: {shelter.shelter_id}");

                return RedirectToAction(nameof(AdminShelters));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                ModelState.AddModelError("", "Error adding shelter: " + ex.Message);
                return View();
            }
        }

        // GET: Edit Shelter Page
        public IActionResult EditShelter(int id)
        {
            var shelter = _context.Shelters
                                  .Include(s => s.Users)
                                  .FirstOrDefault(s => s.shelter_id == id);

            if (shelter == null)
                return NotFound();

            return View(shelter);
        }

        // POST: Edit Shelter
        [HttpPost]
        public IActionResult EditShelter(int id,
            string shelter_name,
            string email,
            string phone,
            string address,
            string city,
            string pan_number,
            string website,
            string hours,
            string description,
            string status,
            IFormFile shelterImage,
            string username,
            string password,
            string userEmail,
            string userPhone,
            string userAddress,
            string userStatus,
            IFormFile userImage)
        {
            try
            {
                var shelter = _context.Shelters
                                      .Include(s => s.Users)
                                      .FirstOrDefault(s => s.shelter_id == id);

                if (shelter == null)
                    return NotFound();

                // Update shelter image
                if (shelterImage != null && shelterImage.Length > 0)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/shelters");
                    Directory.CreateDirectory(uploads);
                    string shelterFileName = Guid.NewGuid().ToString() + Path.GetExtension(shelterImage.FileName);
                    using (var stream = new FileStream(Path.Combine(uploads, shelterFileName), FileMode.Create))
                    {
                        shelterImage.CopyTo(stream);
                    }
                    shelter.shelter_img = "/images/shelters/" + shelterFileName;
                }

                // Update shelter fields
                shelter.shelter_name = shelter_name;
                shelter.email = email;
                shelter.phone = phone;
                shelter.address = address;
                shelter.city = city;
                shelter.pan_number = pan_number;
                shelter.website = website;
                shelter.hours = hours;
                shelter.description = description;
                shelter.status = string.IsNullOrEmpty(status) ? "Active" : status;

                // Update user if exists
                if (shelter.Users != null)
                {
                    var user = shelter.Users;
                    user.username = username;
                    user.email = userEmail;
                    user.phone = userPhone;
                    user.address = userAddress;
                    user.status = string.IsNullOrEmpty(userStatus) ? "Active" : userStatus;

                    // Update password only if provided
                    if (!string.IsNullOrEmpty(password))
                    {
                        user.password = password;
                    }

                    // Update user image
                    if (userImage != null && userImage.Length > 0)
                    {
                        string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                        Directory.CreateDirectory(uploads);
                        string userFileName = Guid.NewGuid().ToString() + Path.GetExtension(userImage.FileName);
                        using (var stream = new FileStream(Path.Combine(uploads, userFileName), FileMode.Create))
                        {
                            userImage.CopyTo(stream);
                        }
                        user.image_url = "/images/users/" + userFileName;
                    }

                    _context.Users.Update(user);
                }

                _context.Shelters.Update(shelter);
                _context.SaveChanges();

                return RedirectToAction(nameof(AdminShelters));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while editing the shelter: " + ex.Message);
                return View();
            }
        }

        // POST: Delete Shelter
        [HttpPost]
        public IActionResult DeleteShelter(int id)
        {
            try
            {
                var shelter = _context.Shelters
                                      .Include(s => s.Pets)
                                      .FirstOrDefault(s => s.shelter_id == id);

                if (shelter == null)
                    return NotFound();

                _context.Shelters.Remove(shelter);
                _context.SaveChanges();

                return RedirectToAction(nameof(AdminShelters));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return RedirectToAction(nameof(ViewShelter), new { id });
            }
        }
    }
}

