using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Filters;
using System.Diagnostics;

namespace ReachAPaw.Controllers.Admin
{
    [AdminAuthorize]
    public class AdminCommunityController : Controller
    {
        private readonly RapDbContext _context;

        public AdminCommunityController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminCommunity()
        {
            var posts = _context.Community
                               .Include(c => c.Users)
                               .OrderByDescending(c => c.created_at)
                               .ToList();

            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            return View(posts);
        }

        public IActionResult ViewPost(int id)
        {
            var post = _context.Community
                              .Include(c => c.Users)
                              .FirstOrDefault(c => c.community_id == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpGet]
        public IActionResult GetPostData(int id)
        {
            var post = _context.Community
                              .Include(c => c.Users)
                              .FirstOrDefault(c => c.community_id == id);

            if (post == null)
                return NotFound();

            return Json(new
            {
                community_id = post.community_id,
                title = post.title,
                post_content = post.post_content,
                created_at = post.created_at,
                users = new
                {
                    username = post.Users?.username,
                    image_url = post.Users?.image_url
                }
            });
        }

        [HttpPost]
        public IActionResult AddPost(string title, string post_content, int category_id = 1)
        {
            Debug.WriteLine($"=== AddPost POST called ===");
            Debug.WriteLine($"Title: {title}");
            Debug.WriteLine($"Content: {post_content}");

            try
            {
                // Get current admin user from session
                var adminUserId = HttpContext.Session.GetInt32("user_id");
                if (!adminUserId.HasValue)
                {
                    return Unauthorized();
                }

                var post = new CommunityModel
                {
                    title = title,
                    post_content = post_content,
                    user_id = adminUserId.Value,
                    category_id = category_id,
                    created_at = DateTime.Now
                };

                Debug.WriteLine($"Adding post: {post.title}");
                _context.Community.Add(post);
                _context.SaveChanges();
                Debug.WriteLine($"Post created successfully with ID: {post.community_id}");

                return RedirectToAction(nameof(AdminCommunity));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ERROR in AddPost ===");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return RedirectToAction(nameof(AdminCommunity));
            }
        }

        [HttpPost]
        public IActionResult DeletePost(int id)
        {
            Debug.WriteLine($"=== DeletePost POST called ===");
            Debug.WriteLine($"Post ID: {id}");

            try
            {
                var post = _context.Community.FirstOrDefault(c => c.community_id == id);

                if (post == null)
                    return NotFound();

                Debug.WriteLine($"Deleting post: {post.title}");
                _context.Community.Remove(post);
                _context.SaveChanges();
                Debug.WriteLine($"Post deleted successfully");

                return RedirectToAction(nameof(AdminCommunity));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ERROR in DeletePost ===");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return RedirectToAction(nameof(AdminCommunity));
            }
        }
    }
}



