using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class LearnController : Controller
    {
        // ডামি ডেটা (পরে ডেটাবেস থেকে আনা হবে)
        private static readonly List<CourseDetailViewModel> Courses = new()
        {
            new CourseDetailViewModel
            {
                Id = 1,
                Title = "English Grammar Course",
                Description = "Master English grammar with real certificates and interactive exercises. This comprehensive course covers all aspects of English grammar from basic to advanced level.",
                Image = "https://i.ibb.co.com/KjyJyXy8/English-grammar-courses-online-with-real-certificates.jpg",
                Category = "Language",
                Price = 1499,
                Students = 12500,
                Rating = 4.9,
                Instructor = "Ms. Farhana Akter",
                Duration = "12 Weeks",
                Lessons = 48,
                Level = "Beginner to Intermediate",
                IsNew = false,
                IsPopular = true,
                Features = new List<string> { "Interactive Quizzes", "Certificate Included", "Lifetime Access", "24/7 Support" }
            },
            new CourseDetailViewModel
            {
                Id = 2,
                Title = "Basic WordPress Theme Development",
                Description = "Full course from beginner to expert — build custom WordPress themes from scratch. Learn PHP, WordPress hooks, and theme development best practices.",
                Image = "https://i.ibb.co.com/ZpRSvpsk/Basic-Word-Press-theme-development-full-course.jpg",
                Category = "Web Development",
                Price = 1999,
                Students = 8300,
                Rating = 4.8,
                Instructor = "Mr. Rajib Hasan",
                Duration = "8 Weeks",
                Lessons = 36,
                Level = "Beginner",
                IsNew = false,
                IsPopular = true,
                Features = new List<string> { "Live Projects", "Source Code", "Certificate", "PHP Basics" }
            },
            new CourseDetailViewModel
            {
                Id = 3,
                Title = "Complete React Front-end Developer",
                Description = "Master React.js with real-world projects and build modern web applications. Covers hooks, state management, routing, and deployment.",
                Image = "https://i.ibb.co.com/rf3RgYqG/Complete-React-Front-end-developer-course.jpg",
                Category = "Frontend",
                Price = 2499,
                Students = 15700,
                Rating = 4.9,
                Instructor = "Dr. Sarah Ahmed",
                Duration = "14 Weeks",
                Lessons = 62,
                Level = "Intermediate",
                IsNew = true,
                IsPopular = true,
                Features = new List<string> { "React Hooks", "Redux", "Tailwind CSS", "Firebase", "REST APIs" }
            },
            new CourseDetailViewModel
            {
                Id = 4,
                Title = "Complete Web Design",
                Description = "From beginner to professional web designer — UI/UX design mastery with Figma and Webflow.",
                Image = "https://i.ibb.co.com/XZSqdW0B/Complete-Web-Design-from-Figma-to-Webflow.jpg",
                Category = "Design",
                Price = 1799,
                Students = 10200,
                Rating = 4.7,
                Instructor = "Ms. Nusrat Jahan",
                Duration = "10 Weeks",
                Lessons = 40,
                Level = "Beginner",
                IsNew = false,
                IsPopular = false,
                Features = new List<string> { "Figma", "Webflow", "Portfolio Project", "Design Principles" }
            },
            new CourseDetailViewModel
            {
                Id = 5,
                Title = "Flutter Development Bootcamp",
                Description = "Learn Flutter & Dart from scratch — build cross-platform mobile apps for iOS and Android.",
                Image = "https://i.ibb.co.com/qZ60Vqg/Flutter-Development-Bootcamp-with-Dart.jpg",
                Category = "Mobile Development",
                Price = 2299,
                Students = 6900,
                Rating = 4.8,
                Instructor = "Mr. Kamal Hossain",
                Duration = "12 Weeks",
                Lessons = 55,
                Level = "Beginner to Intermediate",
                IsNew = true,
                IsPopular = false,
                Features = new List<string> { "Dart", "Flutter Widgets", "Firebase", "App Deployment", "State Management" }
            },
            new CourseDetailViewModel
            {
                Id = 6,
                Title = "The Ultimate Figma Course",
                Description = "From zero to expert in UI/UX design — master Figma like a pro with real-world projects.",
                Image = "https://i.ibb.co.com/rGttSLJy/The-Ultimate-Figma-Course-From-Zero-to-Expert.jpg",
                Category = "Design",
                Price = 1599,
                Students = 9400,
                Rating = 4.9,
                Instructor = "Ms. Farhana Akter",
                Duration = "6 Weeks",
                Lessons = 28,
                Level = "Beginner",
                IsNew = false,
                IsPopular = true,
                Features = new List<string> { "UI/UX Principles", "Prototyping", "Design Systems", "Collaboration" }
            }
        };

        // ===== লার্ন পেজ (সমস্ত কোর্স) =====
        public IActionResult Index()
        {
            return View("Learn");  // ← Learn.cshtml দেখাবে
        }

        // ===== কোর্স ডিটেলস =====
        public IActionResult Details(int id)
        {
            var course = Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // ===== API: সমস্ত কোর্স (AJAX এর জন্য) =====
        [HttpGet]
        public IActionResult GetAllCourses()
        {
            return Json(Courses);
        }

        // ===== API: একটি কোর্স (AJAX এর জন্য) =====
        [HttpGet]
        public IActionResult GetCourse(int id)
        {
            var course = Courses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                return Json(new { success = false, message = "Course not found" });
            }
            return Json(new { success = true, course });
        }
    }
}