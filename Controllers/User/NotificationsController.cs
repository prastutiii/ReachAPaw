using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Data;
using ReachAPaw.Filters;

namespace ReachAPaw.Controllers
{
    [UserAuthorize]
    public class NotificationsController : Controller
    {
        private readonly RapDbContext _context;

        public NotificationsController(RapDbContext context)
        {
            _context = context;
        }

        public IActionResult GetNotifications()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
                return Json(new { notifications = new List<object>(), unreadCount = 0 });

            var notifications = _context.Notifications
                                        .Where(n => n.user_id == userId)
                                        .OrderByDescending(n => n.created_at)
                                        .Take(20)
                                        .Select(n => new {
                                            n.notification_id,
                                            n.message,
                                            n.is_read,
                                            date = n.created_at.ToString("MMM dd, yyyy")
                                        })
                                        .ToList();

            var unreadCount = _context.Notifications
                                      .Count(n => n.user_id == userId && !n.is_read);

            return Json(new { notifications, unreadCount });
        }

        [HttpPost]
        public IActionResult MarkAllRead()
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null) return Json(new { success = false });

            var unread = _context.Notifications
                                 .Where(n => n.user_id == userId && !n.is_read)
                                 .ToList();

            unread.ForEach(n => n.is_read = true);
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}