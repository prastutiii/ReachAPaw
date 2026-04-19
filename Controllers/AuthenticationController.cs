using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReachAPaw.Data;
using ReachAPaw.Models;
using ReachAPaw.Services;

namespace ReachAPaw.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly RapDbContext _context;
        private readonly EmailService _emailService;

        public AuthenticationController(RapDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                    .FirstOrDefault(u => u.email == email);

                if (user != null)
                {
                    var hasher = new PasswordHasher<UserModel>();
                    var result = hasher.VerifyHashedPassword(user, user.password, password);
                    if (result == PasswordVerificationResult.Failed)
                    {
                        ViewBag.Error = "Invalid email or password";
                        return View();
                    }

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
                        var shelter = _context.Shelters.FirstOrDefault(s => s.user_id == user.user_id);
                        if (shelter != null)
                        {
                            HttpContext.Session.SetInt32("shelter_id", shelter.shelter_id);
                        }
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
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("otp", otp);
            HttpContext.Session.SetString("otp_email", email);
            HttpContext.Session.SetString("otp_username", username);
            HttpContext.Session.SetString("otp_password", password);
            HttpContext.Session.SetString("otp_address", address);
            HttpContext.Session.SetString("otp_phone", phone);
            HttpContext.Session.SetString("otp_expiry", DateTime.Now.AddMinutes(10).ToString());

            _emailService.SendOtp(email, otp);

            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            if (HttpContext.Session.GetString("otp_email") == null)
                return RedirectToAction("Register");

            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            var savedOtp = HttpContext.Session.GetString("otp");
            var expiry = HttpContext.Session.GetString("otp_expiry");
            var email = HttpContext.Session.GetString("otp_email");
            var username = HttpContext.Session.GetString("otp_username");
            var password = HttpContext.Session.GetString("otp_password");
            var address = HttpContext.Session.GetString("otp_address");
            var phone = HttpContext.Session.GetString("otp_phone");

            if (savedOtp == null || email == null)
            {
                ViewBag.Error = "Session expired. Please register again.";
                return View();
            }

            if (DateTime.Now > DateTime.Parse(expiry))
            {
                ViewBag.Error = "OTP has expired. Please register again.";
                return RedirectToAction("Register");
            }

            if (otp != savedOtp)
            {
                ViewBag.Error = "Invalid OTP. Please try again.";
                return View();
            }

            var hasher = new PasswordHasher<UserModel>();

            var user = new UserModel
            {
                username = username,
                email = email,
                address = address,
                phone = phone,
                password = hasher.HashPassword(null, password),
                role = "user",
                status = "active"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.Remove("otp");
            HttpContext.Session.Remove("otp_email");
            HttpContext.Session.Remove("otp_username");
            HttpContext.Session.Remove("otp_password");
            HttpContext.Session.Remove("otp_address");
            HttpContext.Session.Remove("otp_phone");
            HttpContext.Session.Remove("otp_expiry");

            HttpContext.Session.SetInt32("user_id", user.user_id);
            HttpContext.Session.SetString("user_name", user.username);
            HttpContext.Session.SetString("user_role", user.role);

            return RedirectToAction("Home", "Home");
        }

        public IActionResult ResendOtp()
        {
            var email = HttpContext.Session.GetString("otp_email");
            if (email == null)
                return RedirectToAction("Register");

            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("otp", otp);
            HttpContext.Session.SetString("otp_expiry", DateTime.Now.AddMinutes(10).ToString());

            _emailService.SendOtp(email, otp);

            ViewBag.Success = "A new OTP has been sent to your email.";
            return View("VerifyOtp");
        }
    }
}