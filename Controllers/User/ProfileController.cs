using Microsoft.AspNetCore.Http; // Required for session access
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;
using System.Linq;

[UserAuthorize]
public class ProfileController : Controller
{
    private readonly RapDbContext _context;

    // Dependency Injection for your Database Context
    public ProfileController(RapDbContext context)
    {
        _context = context;
    }
    public IActionResult Profile()
    {
        int? userId = HttpContext.Session.GetInt32("user_id");

        var user = _context.Users.FirstOrDefault(u => u.user_id == userId);

        if (user == null)
            return NotFound();

        //User's community posts
        var posts = _context.Community
            .Where(c => c.user_id == userId)
            .Include(p => p.Categories)
            .OrderByDescending(c => c.created_at)
            .ToList();

        ViewBag.UserPosts = posts;

        return View(user);
    }

    [HttpGet]
    public IActionResult EditProfile()
    {
        int? userId = HttpContext.Session.GetInt32("user_id");
        var user = _context.Users.FirstOrDefault(u => u.user_id == userId);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    public IActionResult EditProfile(UserModel updatedUser, IFormFile pfpFile)
    {
        var userInDb = _context.Users.FirstOrDefault(u => u.user_id == updatedUser.user_id);

        if (userInDb != null)
        {
            // Handle PFP upload using your pet image logic
            if (pfpFile != null && pfpFile.Length > 0)
            {
                // Define path: wwwroot/images/users
                string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
                Directory.CreateDirectory(uploads);

                // Generate unique filename
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(pfpFile.FileName);
                string fullPath = Path.Combine(uploads, fileName);

                try
                {
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        pfpFile.CopyTo(stream);
                    }

                    // Store the relative path in the database
                    userInDb.image_url = "/images/users/" + fileName;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception while saving PFP: " + ex.Message);
                }
            }

            // Update other fields
            userInDb.username = updatedUser.username;
            userInDb.email = updatedUser.email;
            userInDb.address = updatedUser.address;
            userInDb.phone = updatedUser.phone;
            userInDb.password = updatedUser.password;

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }

        return NotFound();
    }
}