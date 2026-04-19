using Microsoft.AspNetCore.Http; // Required for session access
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;
using Microsoft.AspNetCore.Identity;
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
    public IActionResult EditProfile(UserModel updatedUser, IFormFile pfpFile, string password)
    {
        var userInDb = _context.Users.FirstOrDefault(u => u.user_id == updatedUser.user_id);

        if (userInDb == null)
            return NotFound();

        if (pfpFile != null && pfpFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(pfpFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("pfpFile", "Only image files are allowed (jpg, jpeg, png, gif, webp).");
                return View(updatedUser);
            }

            string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");
            Directory.CreateDirectory(uploads);
            string fileName = Guid.NewGuid().ToString() + extension;

            using (var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                pfpFile.CopyTo(stream);

            userInDb.image_url = "/images/users/" + fileName;
        }

        userInDb.username = updatedUser.username;
        userInDb.email = updatedUser.email;
        userInDb.address = updatedUser.address;
        userInDb.phone = updatedUser.phone;

        if (!string.IsNullOrEmpty(password))
        {
            var hasher = new PasswordHasher<UserModel>();
            userInDb.password = hasher.HashPassword(null, password);
        }

        _context.SaveChanges();
        return RedirectToAction("Profile");
    }

}