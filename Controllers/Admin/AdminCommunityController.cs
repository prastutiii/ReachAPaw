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
                               .Include(c => c.Categories)
                               .OrderByDescending(c => c.created_at)
                               .ToList();

            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            return View(posts);
        }

        [HttpPost]
        public IActionResult ToggleLike(int community_id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return Json(new { success = false, message = "Please login first" });

            try
            {
                var existingLike = _context.Likes
                    .FirstOrDefault(l => l.community_id == community_id && l.user_id == userId.Value);

                if (existingLike != null)
                {
                    // Remove like
                    _context.Likes.Remove(existingLike);
                    _context.SaveChanges();

                    var totalLikes = _context.Likes.Count(l => l.community_id == community_id);
                    return Json(new { success = true, liked = false, totalLikes = totalLikes });
                }
                else
                {
                    // Add like
                    var like = new LikesModel
                    {
                        community_id = community_id,
                        user_id = userId.Value
                    };
                    _context.Likes.Add(like);
                    _context.SaveChanges();

                    var totalLikes = _context.Likes.Count(l => l.community_id == community_id);
                    return Json(new { success = true, liked = true, totalLikes = totalLikes });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetPostLikes(int community_id)
        {
            try
            {
                var likes = _context.Likes
                    .Include(l => l.Users)
                    .Where(l => l.community_id == community_id)
                    .Select(l => new
                    {
                        username = l.Users.username,
                        image_url = l.Users.image_url
                    })
                    .ToList();

                var totalLikes = likes.Count;
                return Json(new { success = true, totalLikes = totalLikes, likedBy = likes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddComment(int community_id, string comment)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return Json(new { success = false, message = "Please login first" });

            if (string.IsNullOrWhiteSpace(comment))
                return Json(new { success = false, message = "Comment cannot be empty" });

            try
            {
                var newComment = new CommentsModel
                {
                    community_id = community_id,
                    user_id = userId.Value,
                    comment = comment.Trim(),
                    date = DateTime.Now
                };

                _context.Comments.Add(newComment);
                _context.SaveChanges();

                var user = _context.Users.FirstOrDefault(u => u.user_id == userId.Value);

                return Json(new 
                { 
                    success = true, 
                    comment = new 
                    { 
                        comment_id = newComment.comment_id,
                        text = newComment.comment,
                        username = user?.username,
                        date = newComment.date.ToString("MMM dd, yyyy HH:mm"),
                        user_id = userId.Value
                    },
                    totalComments = _context.Comments.Count(c => c.community_id == community_id)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetComments(int community_id)
        {
            try
            {
                var comments = _context.Comments
                    .Include(c => c.Users)
                    .Where(c => c.community_id == community_id)
                    .OrderByDescending(c => c.date)
                    .Select(c => new
                    {
                        comment_id = c.comment_id,
                        text = c.comment,
                        username = c.Users.username,
                        date = c.date.ToString("MMM dd, yyyy HH:mm"),
                        user_id = c.user_id
                    })
                    .ToList();

                return Json(new { success = true, comments = comments });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteComment(int comment_id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return Json(new { success = false, message = "Please login first" });

            try
            {
                var comment = _context.Comments.FirstOrDefault(c => c.comment_id == comment_id);

                if (comment == null)
                    return Json(new { success = false, message = "Comment not found" });

                if (comment.user_id != userId.Value)
                    return Json(new { success = false, message = "You can only delete your own comments" });

                _context.Comments.Remove(comment);
                _context.SaveChanges();

                return Json(new { success = true, message = "Comment deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult IsPostLiked(int community_id)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return Json(new { liked = false, totalLikes = 0 });

            try
            {
                var isLiked = _context.Likes.Any(l => l.community_id == community_id && l.user_id == userId.Value);
                var totalLikes = _context.Likes.Count(l => l.community_id == community_id);

                return Json(new { liked = isLiked, totalLikes = totalLikes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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