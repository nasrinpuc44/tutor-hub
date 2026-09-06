using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;

namespace Tutorbub.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public AccountController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== লগইন পেজ দেখানো =====
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // ===== লগইন করা =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _dbHelper.AuthenticateUser(username, password);

            if (user != null)
            {
                // লাস্ট লগইন আপডেট
                _dbHelper.UpdateLastLogin(username);

                // সেশন সেট করা
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("UserFullName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);

                // এডমিন হলে ড্যাশবোর্ডে পাঠানো
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        // ===== রেজিস্টার পেজ দেখানো =====
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // ===== রেজিস্টার করা =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string password, string fullName, string email, string confirmPassword)
        {
            // পাসওয়ার্ড মিল চেক
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            // পাসওয়ার্ডের দৈর্ঘ্য চেক
            if (password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters";
                return View();
            }

            // ইউজারনেম আছে কিনা চেক
            if (_dbHelper.UsernameExists(username))
            {
                ViewBag.Error = "Username already exists";
                return View();
            }

            // ইমেইল আছে কিনা চেক
            if (_dbHelper.EmailExists(email))
            {
                ViewBag.Error = "Email already registered";
                return View();
            }

            // নতুন ইউজার তৈরি
            var user = new User
            {
                UserName = username,
                Password = password,
                FullName = fullName,
                Email = email
            };

            if (_dbHelper.RegisterUser(user))
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration failed. Please try again.";
            return View();
        }

        // ===== লগআউট =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}