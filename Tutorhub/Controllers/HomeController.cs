// 📁 Controllers/HomeController.cs
// লোকেশন: Tutorbub/Controllers/HomeController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;

namespace Tutorhub.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public HomeController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== হোম পেইজ =====
        public IActionResult Index()
        {
            // ✅ সাইট সেটিংস লোড করা (হিরো ইমেজ, স্লাইডার, কালার)
            try
            {
                var settings = _dbHelper.GetSiteSettings();
                ViewBag.SiteSettings = settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading site settings: " + ex.Message);
                ViewBag.SiteSettings = new SiteSettings();
            }

            return View();
        }

        // ===== Privacy পেজ =====
        public IActionResult Privacy()
        {
            return View();
        }

        // ===== About পেজ =====
        public IActionResult About()
        {
            return View();
        }

        // ===== Contact পেজ =====
        public IActionResult Contact()
        {
            return View();
        }

        // ===== Error পেজ =====
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}