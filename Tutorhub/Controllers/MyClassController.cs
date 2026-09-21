// 📁 Controllers/MyClassController.cs
// লোকেশন: Tutorbub/Controllers/MyClassController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class MyClassController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public MyClassController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ============================================================
        // ===== My Class পেজ (এনরোলড কোর্স লিস্ট) =====
        // ============================================================
        [HttpGet]
        public IActionResult MyClass()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                TempData["Error"] = "Please login to view your classes.";
                return RedirectToAction("Login", "Account");
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // ১) ইউজারের approved কোর্স (CourseOrders থেকে)
            var enrolled = _dbHelper.GetUserEnrolledCourses(userId);

            // ২) static কোর্স ডেটা merge করা
            var allCourses = GetAllCoursesStatic();

            var result = enrolled.Select(e =>
            {
                var meta = allCourses.FirstOrDefault(c => c.Id == e.CourseId);
                if (meta != null)
                {
                    e.Instructor = meta.Instructor;
                    e.Duration = meta.Duration;
                    e.Lessons = meta.Lessons;
                    e.Level = meta.Level;

                    if (string.IsNullOrEmpty(e.CourseImage))
                        e.CourseImage = meta.Image;
                    if (string.IsNullOrEmpty(e.CourseName))
                        e.CourseName = meta.Title;
                }
                else
                {
                    e.Instructor = "Smart Tutor Hub";
                    e.Duration = "Self-paced";
                    e.Lessons = 0;
                    e.Level = "All Levels";
                }
                return e;
            })
            .OrderByDescending(e => e.EnrolledDate)
            .ToList();

            ViewBag.TotalCourses = result.Count;
            return View("myclass", result);
        }

        // ============================================================
        // ===== Classroom পেজ (Video + Module List) =====
        // ============================================================
        [HttpGet]
        public IActionResult Classroom(int courseId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // ১) ইউজার এই কোর্সে enrolled কিনা চেক
            if (!_dbHelper.IsUserEnrolled(userId, courseId))
            {
                TempData["Error"] = "You are not enrolled in this course.";
                return RedirectToAction("MyClass");
            }

            // ২) Static কোর্স মেটা বের করা
            var allCourses = GetAllCoursesStatic();
            var meta = allCourses.FirstOrDefault(c => c.Id == courseId);
            if (meta == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("MyClass");
            }

            // ৩) Static modules/lessons বানানো
            var modules = GetCourseCurriculum(courseId);

            // ৪) First lesson active করা
            var firstLesson = modules.FirstOrDefault()?.Lessons.FirstOrDefault();

            var model = new ClassroomViewModel
            {
                CourseId = courseId,
                CourseTitle = meta.Title,
                CourseImage = meta.Image,
                Instructor = meta.Instructor,
                Duration = meta.Duration,
                Level = meta.Level,
                Modules = modules,
                ActiveLessonId = firstLesson?.Id ?? 0,
                ActiveLessonTitle = firstLesson?.Title ?? "Introduction",
                ActiveLessonVideoUrl = firstLesson?.VideoUrl ?? "",
                ActiveLessonDuration = firstLesson?.Duration ?? ""
            };

            return View("classroom", model);
        }

        // ============================================================
        // ===== Static কোর্স লিস্ট =====
        // ============================================================
        private static List<CourseDetailViewModel> GetAllCoursesStatic()
        {
            return new List<CourseDetailViewModel>
            {
                new CourseDetailViewModel { Id = 1, Title = "English Grammar Course",
                    Image = "https://i.ibb.co.com/KjyJyXy8/English-grammar-courses-online-with-real-certificates.jpg",
                    Instructor = "Ms. Farhana Akter", Duration = "12 Weeks", Lessons = 48, Level = "Beginner to Intermediate" },
                new CourseDetailViewModel { Id = 2, Title = "Basic WordPress Theme Development",
                    Image = "https://i.ibb.co.com/ZpRSvpsk/Basic-Word-Press-theme-development-full-course.jpg",
                    Instructor = "Mr. Rajib Hasan", Duration = "8 Weeks", Lessons = 36, Level = "Beginner" },
                new CourseDetailViewModel { Id = 3, Title = "Complete React Front-end Developer",
                    Image = "https://i.ibb.co.com/rf3RgYqG/Complete-React-Front-end-developer-course.jpg",
                    Instructor = "Dr. Sarah Ahmed", Duration = "14 Weeks", Lessons = 62, Level = "Intermediate" },
                new CourseDetailViewModel { Id = 4, Title = "Complete Web Design",
                    Image = "https://i.ibb.co.com/XZSqdW0B/Complete-Web-Design-from-Figma-to-Webflow.jpg",
                    Instructor = "Ms. Nusrat Jahan", Duration = "10 Weeks", Lessons = 40, Level = "Beginner" },
                new CourseDetailViewModel { Id = 5, Title = "Flutter Development Bootcamp",
                    Image = "https://i.ibb.co.com/qZ60Vqg/Flutter-Development-Bootcamp-with-Dart.jpg",
                    Instructor = "Mr. Kamal Hossain", Duration = "12 Weeks", Lessons = 55, Level = "Beginner to Intermediate" },
                new CourseDetailViewModel { Id = 6, Title = "The Ultimate Figma Course",
                    Image = "https://i.ibb.co.com/rGttSLJy/The-Ultimate-Figma-Course-From-Zero-to-Expert.jpg",
                    Instructor = "Ms. Farhana Akter", Duration = "6 Weeks", Lessons = 28, Level = "Beginner" }
            };
        }

        // ============================================================
        // ===== Static Curriculum (per course) =====
        // ============================================================
        private static List<ClassroomModule> GetCourseCurriculum(int courseId)
        {
            // demo video URL (Sample — যেকোনো mp4 link দিতে পারেন)
            const string demoVideo = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";

            return new List<ClassroomModule>
            {
                new ClassroomModule
                {
                    ModuleNumber = 1,
                    Title = "Getting Started",
                    TotalDuration = "24 min",
                    CompletedLessons = 2,
                    TotalLessons = 3,
                    Lessons = new List<ClassroomLesson>
                    {
                        new ClassroomLesson { Id = 1, Title = "1-1 Welcome & Course Overview", Duration = "6 min", VideoUrl = demoVideo, IsCompleted = true },
                        new ClassroomLesson { Id = 2, Title = "1-2 Setting Up Your Environment", Duration = "8 min", VideoUrl = demoVideo, IsCompleted = true },
                        new ClassroomLesson { Id = 3, Title = "1-3 Your First Project", Duration = "10 min", VideoUrl = demoVideo, IsCompleted = false }
                    }
                },
                new ClassroomModule
                {
                    ModuleNumber = 2,
                    Title = "Core Concepts",
                    TotalDuration = "1h 12m",
                    CompletedLessons = 1,
                    TotalLessons = 4,
                    Lessons = new List<ClassroomLesson>
                    {
                        new ClassroomLesson { Id = 4, Title = "2-1 Introduction to Fundamentals", Duration = "14 min", VideoUrl = demoVideo, IsCompleted = true },
                        new ClassroomLesson { Id = 5, Title = "2-2 Working with Data", Duration = "18 min", VideoUrl = demoVideo, IsCompleted = false },
                        new ClassroomLesson { Id = 6, Title = "2-3 Building Blocks", Duration = "20 min", VideoUrl = demoVideo, IsCompleted = false },
                        new ClassroomLesson { Id = 7, Title = "2-4 Practice Exercise", Duration = "20 min", VideoUrl = demoVideo, IsCompleted = false }
                    }
                },
                new ClassroomModule
                {
                    ModuleNumber = 3,
                    Title = "Advanced Techniques",
                    TotalDuration = "1h 05m",
                    CompletedLessons = 0,
                    TotalLessons = 4,
                    Lessons = new List<ClassroomLesson>
                    {
                        new ClassroomLesson { Id = 8, Title = "3-1 Advanced Patterns", Duration = "16 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 9, Title = "3-2 Performance Optimization", Duration = "14 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 10, Title = "3-3 Real-world Case Study", Duration = "20 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 11, Title = "3-4 Module Quiz", Duration = "15 min", VideoUrl = demoVideo }
                    }
                },
                new ClassroomModule
                {
                    ModuleNumber = 4,
                    Title = "Final Project & Next Steps",
                    TotalDuration = "48 min",
                    CompletedLessons = 0,
                    TotalLessons = 3,
                    Lessons = new List<ClassroomLesson>
                    {
                        new ClassroomLesson { Id = 12, Title = "4-1 Project Requirements", Duration = "12 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 13, Title = "4-2 Build & Deploy", Duration = "22 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 14, Title = "4-3 Certificate & What's Next", Duration = "14 min", VideoUrl = demoVideo }
                    }
                }
            };
        }
    }
}