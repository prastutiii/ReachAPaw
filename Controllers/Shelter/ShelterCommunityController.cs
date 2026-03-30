using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{
    [ShelterAuthorize]
    public class ShelterCommunityController : Controller
    {
        private readonly RapDbContext _context;

        public ShelterCommunityController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult ShelterCommunity()
        {
            var posts = _context.Community
                                .Include(p => p.Users)
                                .Include(p => p.Categories)
                                .OrderByDescending(p => p.created_at)
                                .ToList();

            ViewBag.Categories = _context.Categories.ToList();

            return View(posts);
        }

        [HttpPost]
        public IActionResult CreatePost(string title, string content, int category_id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var post = new CommunityModel
            {
                title = title,
                post_content = content,
                category_id = category_id,
                user_id = userId.Value,
                created_at = DateTime.Now
            };

            _context.Community.Add(post);
            _context.SaveChanges();

            return RedirectToAction("ShelterCommunity");
        }
    }
}