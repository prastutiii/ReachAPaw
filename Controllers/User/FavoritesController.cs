using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReachAPaw.Data;
using ReachAPaw.Filters;
using ReachAPaw.Models;

namespace ReachAPaw.Controllers
{
    [UserAuthorize]
    public class FavoritesController : Controller
    {
        private readonly RapDbContext _context;

        public FavoritesController(RapDbContext context)
        {
            _context = context;
        }
        public IActionResult Favorites()
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var favorites = _context.Favorites
                                    .Include(f => f.Pets)
                                    .Where(f => f.user_id == userId)
                                    .ToList();

            return View(favorites);
        }

        [HttpPost]
        public IActionResult ToggleFavorite(int petId)
        {
            var userId = HttpContext.Session.GetInt32("user_id");

            if (userId == null)
                return Json(new { success = false, message = "Not logged in" });

            var existing = _context.Favorites
                                   .FirstOrDefault(f => f.user_id == userId && f.pet_id == petId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                _context.SaveChanges();
                return Json(new { success = true, isFavorited = false });
            }
            else
            {
                var favorite = new FavoriteModel
                {
                    user_id = userId.Value,
                    pet_id = petId
                };
                _context.Favorites.Add(favorite);
                _context.SaveChanges();
                return Json(new { success = true, isFavorited = true });
            }
        }
    }
}