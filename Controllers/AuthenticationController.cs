using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Models;
using ReachAPaw.Data;

namespace ReachAPaw.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly RapDbContext _context;

        public AuthenticationController(RapDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.email == email && u.password == password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("user_id", user.user_id);
                    HttpContext.Session.SetString("user_name", user.username);
                    HttpContext.Session.SetString("user_role", user.role);

                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Append("user_name", user.username, option);

                    var role = (user.role ?? "").Trim().ToLower();
                    if (role == "admin")
                    {
                        return RedirectToAction("AdminDash", "Admin");
                    }
                    else if (role == "shelter")
                    {
                        return RedirectToAction("ShelterDash", "Shelter");
                    }
                    else
                    {
                        return RedirectToAction("Home", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Invalid email or password";
                }
            }
            else
            {
                ViewBag.Error = "Please fill all fields";
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Authentication");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string address, string phone, string password)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            var user = new UserModel
            {
                username = username,
                email = email,
                address = address,
                phone = phone,
                password = password,
                role = "user",
                status = "active"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("user_id", user.user_id);
            HttpContext.Session.SetString("user_name", user.username);
            HttpContext.Session.SetString("user_role", user.role);

            return RedirectToAction("Home", "Home");
        }
    }
}
