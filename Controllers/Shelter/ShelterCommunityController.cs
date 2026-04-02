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
    }
}
