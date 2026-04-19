using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Filters;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace ReachAPaw.Controllers.Admin
{
    [AdminAuthorize]
    public class AdminUsersController : Controller
    {
        private readonly RapDbContext _context;

        public AdminUsersController(RapDbContext context)
        {
            _context = context;
        }

        // GET: Admin Users List
        public IActionResult AdminUsers()
        {
            var users = _context.Users
                               .OrderByDescending(u => u.user_id)
                               .ToList();

            return View(users);
        }

        // GET: View User Details
        public IActionResult ViewUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.user_id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // GET: Add User Page
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: Add User
        [HttpPost]
        public IActionResult AddUser(
            string username,
            string email,
            string phone,
            string address,
            string password,
            string role,
            string status,
            IFormFile userImage)
        {
            Debug.WriteLine($"=== AddUser POST called ===");
            Debug.WriteLine($"Username: {username}");
            Debug.WriteLine($"Email: {email}");
            Debug.WriteLine($"Role: {role}");

            try
            {
                // Handle user image upload
                string userFileName = null;
                if (userImage != null && userImage.Length > 0)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                    Debug.WriteLine("Uploads folder: " + uploads);
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

                var hasher = new PasswordHasher<UserModel>();

                var user = new UserModel
                {
                    username = username,
                    email = email,
                    phone = phone,
                    address = address,
                    password = hasher.HashPassword(null, password),
                    role = string.IsNullOrEmpty(role) ? "User" : role,
                    status = string.IsNullOrEmpty(status) ? "Active" : status,
                    image_url = userFileName
                };

                Debug.WriteLine($"Adding user: {user.username}");
                _context.Users.Add(user);
                _context.SaveChanges();
                Debug.WriteLine($"User created successfully with ID: {user.user_id}");

                return RedirectToAction(nameof(AdminUsers));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ERROR in AddUser ===");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return RedirectToAction(nameof(AddUser));
            }
        }

        // GET: Edit User Page
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.user_id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Edit User
        [HttpPost]
        public IActionResult EditUser(int id,
            string username,
            string email,
            string phone,
            string address,
            string password,
            string role,
            string status,
            IFormFile userImage)
        {
            Debug.WriteLine($"=== EditUser POST called ===");
            Debug.WriteLine($"User ID: {id}");

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.user_id == id);

                if (user == null)
                    return NotFound();

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

                // Update user fields
                user.username = username;
                user.email = email;
                user.phone = phone;
                user.address = address;
                user.role = string.IsNullOrEmpty(role) ? "User" : role;
                user.status = string.IsNullOrEmpty(status) ? "Active" : status;

                // Update password only if provided
                if (!string.IsNullOrEmpty(password))
                {
                    var hasher = new PasswordHasher<UserModel>();
                    user.password = hasher.HashPassword(null, password);
                }

                Debug.WriteLine($"Updating user: {user.username}");
                _context.Users.Update(user);
                _context.SaveChanges();
                Debug.WriteLine($"User updated successfully");

                return RedirectToAction(nameof(AdminUsers));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ERROR in EditUser ===");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return RedirectToAction(nameof(EditUser), new { id });
            }
        }

        // POST: Delete User
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            Debug.WriteLine($"=== DeleteUser POST called ===");
            Debug.WriteLine($"User ID: {id}");

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.user_id == id);

                if (user == null)
                    return NotFound();

                Debug.WriteLine($"Deleting user: {user.username}");
                _context.Users.Remove(user);
                _context.SaveChanges();
                Debug.WriteLine($"User deleted successfully");

                return RedirectToAction(nameof(AdminUsers));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ERROR in DeleteUser ===");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return RedirectToAction(nameof(ViewUser), new { id });
            }
        }
    }
}
