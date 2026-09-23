// 📁 Controllers/ProgressController.cs
// লোকেশন: Tutorbub/Controllers/ProgressController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class ProgressController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ProgressController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== পুরনো Progress পেজ (এখনো রাখা হলো, কেউ সরাসরি URL দিয়ে ঢুকলে কাজ করবে) =====
        public IActionResult Progress()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // ===== API: ইউজারের প্রগ্রেস ডেটা (AJAX এর জন্য) =====
        [HttpGet]
        public IActionResult GetProgressData()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            try
            {
                // ===== ১) এনরোলড কোর্সের প্রগ্রেস =====
                var enrolled = _dbHelper.GetUserEnrolledCourses(userId);
                var allCourses = _dbHelper.GetAllCourses();

                var courses = new List<object>();
                foreach (var e in enrolled)
                {
                    var meta = allCourses.FirstOrDefault(c => c.Id == e.CourseId);
                    int totalLessons = meta?.Lessons ?? 0;

                    // এখানে আমরা ডামি হিসেবে 40% progress ধরছি।
                    // ভবিষ্যতে ইউজারের lesson completion ট্র্যাক করার টেবিল থাকলে সেখান থেকে আসল ডেটা আনতে হবে।
                    int completedLessons = (int)Math.Round(totalLessons * 0.4);
                    int progressPercent = totalLessons > 0
                        ? (int)Math.Round((double)completedLessons / totalLessons * 100)
                        : 0;

                    courses.Add(new
                    {
                        id = e.CourseId,
                        title = string.IsNullOrEmpty(e.CourseName) ? (meta?.Title ?? "Course") : e.CourseName,
                        image = string.IsNullOrEmpty(e.CourseImage) ? (meta?.Image ?? "") : e.CourseImage,
                        progress = progressPercent,
                        completedLessons = completedLessons,
                        totalLessons = totalLessons
                    });
                }

                // ===== ২) কুইজ হিস্ট্রি =====
                // আপাতত ডামি ডেটা। ভবিষ্যতে ডেটাবেসে QuizAttempts টেবিল থাকলে সেখান থেকে আসল ডেটা আনুন।
                var quizzes = new List<object>
                {
                    new { title = "General Knowledge Quiz", subject = "Mixed",    score = 8, total = 10, date = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd") },
                    new { title = "Physics Fundamentals",   subject = "Physics",  score = 6, total = 10, date = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd") },
                    new { title = "Programming Basics",     subject = "CS",       score = 4, total = 10, date = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd") },
                    new { title = "English Grammar",        subject = "Language", score = 9, total = 10, date = DateTime.UtcNow.AddDays(-4).ToString("yyyy-MM-dd") },
                    new { title = "Chemistry Quiz",         subject = "Science",  score = 7, total = 10, date = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd") }
                };

                // ===== ৩) Stats হিসাব =====
                int totalCorrect = 0;
                int totalQuestions = 0;
                foreach (var q in quizzes)
                {
                    dynamic dq = q;
                    totalCorrect += (int)dq.score;
                    totalQuestions += (int)dq.total;
                }

                int totalPoints = totalCorrect * 5;      // ধরা হলো প্রতি সঠিক উত্তরে ৫ পয়েন্ট
                int quizzesCompleted = quizzes.Count;
                int streakDays = 6;                       // ডামি - পরে আসল ডেটা

                return Json(new
                {
                    success = true,
                    totalPoints = totalPoints,
                    correctAnswers = totalCorrect,
                    streakDays = streakDays,
                    quizzesCompleted = quizzesCompleted,
                    courses = courses,
                    quizzes = quizzes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetProgressData error: " + ex.Message);
                return Json(new
                {
                    success = false,
                    message = "Failed to load progress data.",
                    totalPoints = 0,
                    correctAnswers = 0,
                    streakDays = 0,
                    quizzesCompleted = 0,
                    courses = new object[0],
                    quizzes = new object[0]
                });
            }
        }
    }
}