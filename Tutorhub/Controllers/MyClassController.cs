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

            var enrolled = _dbHelper.GetUserEnrolledCourses(userId);

            // DB থেকে কোর্স মেটা লোড
            var allDbCourses = _dbHelper.GetAllCourses();

            var result = enrolled.Select(e =>
            {
                var meta = allDbCourses.FirstOrDefault(c => c.Id == e.CourseId);
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
        // ===== Classroom পেজ (DB থেকে Video + Module List) =====
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

            // ২) DB থেকে কোর্স মেটা লোড
            var course = _dbHelper.GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("MyClass");
            }

            // ৩) DB থেকে লেসন লোড
            var dbLessons = _dbHelper.GetLessonsByCourseId(courseId);

            List<ClassroomModule> modules;

            if (dbLessons.Count > 0)
            {
                // DB-তে লেসন আছে — সেগুলো module অনুযায়ী গ্রুপ করো
                modules = new List<ClassroomModule>();
                var grouped = dbLessons.GroupBy(l => l.ModuleNumber).OrderBy(g => g.Key);

                foreach (var group in grouped)
                {
                    var lessons = group.OrderBy(l => l.LessonNumber).ToList();
                    int totalDuration = lessons.Sum(l => ParseDuration(l.Duration));

                    modules.Add(new ClassroomModule
                    {
                        ModuleNumber = group.Key,
                        Title = $"Module {group.Key}",
                        TotalDuration = $"{totalDuration} min",
                        CompletedLessons = 0,
                        TotalLessons = lessons.Count,
                        Lessons = lessons.Select(l => new ClassroomLesson
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Duration = l.Duration,
                            VideoUrl = l.VideoUrl,
                            IsCompleted = false
                        }).ToList()
                    });
                }
            }
            else
            {
                // DB-তে কোনো লেসন নেই — static curriculum fallback
                modules = GetCourseCurriculum(courseId);
            }

            // ৪) প্রথম লেসন active করো
            var firstLesson = modules.FirstOrDefault()?.Lessons.FirstOrDefault();

            var model = new ClassroomViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                CourseImage = course.Image,
                Instructor = course.Instructor,
                Duration = course.Duration,
                Level = course.Level,
                Modules = modules,
                ActiveLessonId = firstLesson?.Id ?? 0,
                ActiveLessonTitle = firstLesson?.Title ?? "Introduction",
                ActiveLessonVideoUrl = firstLesson?.VideoUrl ?? "",
                ActiveLessonDuration = firstLesson?.Duration ?? ""
            };

            return View("classroom", model);
        }

        // ============================================================
        // ===== Helper: Parse duration "6 min" -> 6 =====
        // ============================================================
        private static int ParseDuration(string duration)
        {
            if (string.IsNullOrEmpty(duration)) return 0;
            var digits = new string(duration.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out int result) ? result : 0;
        }

        // ============================================================
        // ===== Static Curriculum (Fallback only) =====
        // ============================================================
        private static List<ClassroomModule> GetCourseCurriculum(int courseId)
        {
            const string demoVideo = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";

            return new List<ClassroomModule>
            {
                new ClassroomModule
                {
                    ModuleNumber = 1,
                    Title = "Getting Started",
                    TotalDuration = "24 min",
                    CompletedLessons = 0,
                    TotalLessons = 3,
                    Lessons = new List<ClassroomLesson>
                    {
                        new ClassroomLesson { Id = 1, Title = "1-1 Welcome & Course Overview", Duration = "6 min", VideoUrl = demoVideo, IsCompleted = false },
                        new ClassroomLesson { Id = 2, Title = "1-2 Setting Up Your Environment", Duration = "8 min", VideoUrl = demoVideo, IsCompleted = false },
                        new ClassroomLesson { Id = 3, Title = "1-3 Your First Project", Duration = "10 min", VideoUrl = demoVideo, IsCompleted = false }
                    }
                },
                new ClassroomModule
                {
                    ModuleNumber = 2,
                    Title = "Core Concepts",
                    TotalDuration = "1h 12m",
                    CompletedLessons = 0,
                    TotalLessons = 4,
                    Lessons = new List<ClassroomLesson>
                    {
                        new ClassroomLesson { Id = 4, Title = "2-1 Introduction to Fundamentals", Duration = "14 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 5, Title = "2-2 Working with Data", Duration = "18 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 6, Title = "2-3 Building Blocks", Duration = "20 min", VideoUrl = demoVideo },
                        new ClassroomLesson { Id = 7, Title = "2-4 Practice Exercise", Duration = "20 min", VideoUrl = demoVideo }
                    }
                }
            };
        }
    }
}