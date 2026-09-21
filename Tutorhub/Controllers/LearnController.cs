// 📁 Controllers/LearnController.cs
using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class LearnController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public LearnController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== লার্ন পেজ (ডেটাবেস থেকে সমস্ত কোর্স) =====
        public IActionResult Index()
        {
            var courses = _dbHelper.GetAllCourses();
            ViewBag.AllCourses = courses; // View-এ পাঠানোর জন্য
            return View("Learn");
        }

        // ===== কোর্স ডিটেলস (এনরোলমেন্ট চেক সহ) =====
        public IActionResult Details(int id)
        {
            var course = _dbHelper.GetCourseById(id);
            if (course == null)
            {
                return NotFound();
            }

            // ইউজার লগইন করা থাকলে এনরোলমেন্ট চেক
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                try
                {
                    ViewBag.IsEnrolled = _dbHelper.IsUserEnrolled(userId, id);
                }
                catch
                {
                    ViewBag.IsEnrolled = false;
                }
            }
            else
            {
                ViewBag.IsEnrolled = false;
            }

            return View(course);
        }

        // ===== API: সমস্ত কোর্স (AJAX এর জন্য) =====
        [HttpGet]
        public IActionResult GetAllCourses()
        {
            var courses = _dbHelper.GetAllCourses();
            return Json(courses);
        }

        // ===== API: একটি কোর্স (AJAX এর জন্য) =====
        [HttpGet]
        public IActionResult GetCourse(int id)
        {
            var course = _dbHelper.GetCourseById(id);
            if (course == null)
            {
                return Json(new { success = false, message = "Course not found" });
            }
            return Json(new { success = true, course });
        }
    }
}