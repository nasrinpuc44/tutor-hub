// 📁 Controllers/ProgressController.cs
// লোকেশন: Tutorbub/Controllers/ProgressController.cs
// ✅ এখন সম্পূর্ণ ডেটা ডেটাবেস থেকে আসবে — TotalPoints, Course Progress, Quiz History।

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

        // ============================================================
        // ===== পুরনো Progress পেজ (সরাসরি URL দিয়ে ঢুকলে কাজ করবে) =====
        // ============================================================
        public IActionResult Progress()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // ============================================================
        // ===== API: ইউজারের প্রগ্রেস ডেটা (AJAX এর জন্য) =====
        // GET: /Progress/GetProgressData
        // ============================================================
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
                // ============================================================
                // ১) ✅ ইউজারের আসল TotalPoints (Users টেবিল থেকে)
                // ============================================================
                int totalPoints = _dbHelper.GetUserPoints(userId);

                // ============================================================
                // ২) ✅ এনরোলড কোর্সের আসল প্রগ্রেস (LessonCompletions থেকে)
                // ============================================================
                var enrolled = _dbHelper.GetUserEnrolledCourses(userId);
                var allCourses = _dbHelper.GetAllCourses();

                var courses = new List<object>();
                foreach (var e in enrolled)
                {
                    var meta = allCourses.FirstOrDefault(c => c.Id == e.CourseId);

                    // আসল lesson completion ডেটা DB থেকে
                    var moduleProgress = _dbHelper.GetModuleProgress(userId, e.CourseId);
                    int totalLessons = moduleProgress.Values.Sum(v => v.Total);
                    int completedLessons = moduleProgress.Values.Sum(v => v.Completed);

                    // যদি CourseLessons টেবিলে lesson না থাকে, তাহলে Course.Lessons fallback
                    if (totalLessons == 0 && meta != null && meta.Lessons > 0)
                    {
                        totalLessons = meta.Lessons;
                        completedLessons = 0;
                    }

                    int progressPercent = totalLessons > 0
                        ? (int)Math.Round((double)completedLessons / totalLessons * 100)
                        : 0;

                    courses.Add(new
                    {
                        id = e.CourseId,
                        title = string.IsNullOrEmpty(e.CourseName)
                                    ? (meta?.Title ?? "Course")
                                    : e.CourseName,
                        image = string.IsNullOrEmpty(e.CourseImage)
                                    ? (meta?.Image ?? "")
                                    : e.CourseImage,
                        progress = progressPercent,
                        completedLessons = completedLessons,
                        totalLessons = totalLessons
                    });
                }

                // ============================================================
                // ৩) ✅ Quiz History — সব কোর্সের আসল QuizAttempts
                // ============================================================
                var quizzes = new List<object>();
                int totalCorrect = 0;
                int totalQuestions = 0;
                int quizzesCompleted = 0;

                foreach (var e in enrolled)
                {
                    var attempts = _dbHelper.GetUserQuizAttempts(userId, e.CourseId);
                    if (attempts == null || attempts.Count == 0) continue;

                    foreach (var a in attempts)
                    {
                        quizzesCompleted++;
                        totalCorrect += a.Score;
                        totalQuestions += a.TotalMarks;

                        // Quiz title — attempt থেকে আসে (GetUserQuizAttempts JOIN করে এনেছে)
                        string quizTitle = string.IsNullOrEmpty(a.QuizTitle)
                            ? "Quiz"
                            : a.QuizTitle;

                        string courseName = string.IsNullOrEmpty(e.CourseName)
                            ? "Course"
                            : e.CourseName;

                        int pct = a.TotalMarks > 0
                            ? (int)Math.Round((double)a.Score / a.TotalMarks * 100)
                            : 0;

                        quizzes.Add(new
                        {
                            title = quizTitle,
                            subject = courseName,
                            score = a.Score,
                            total = a.TotalMarks,
                            percentage = pct,
                            passed = a.Passed,
                            date = a.AttemptedAt.ToString("yyyy-MM-dd")
                        });
                    }
                }

                // সর্বশেষ ১০টি quiz দেখাও (নতুন থেকে পুরাতন)
                quizzes = quizzes
                    .OrderByDescending(q => ((dynamic)q).date)
                    .Take(10)
                    .ToList();

                // ============================================================
                // ৪) ✅ Streak (কত দিন টানা অ্যাক্টিভ) — LessonCompletions থেকে
                // ============================================================
                int streakDays = CalculateStreakDays(userId, enrolled);

                // ============================================================
                // ৫) সফলভাবে সব ডেটা return
                // ============================================================
                return Json(new
                {
                    success = true,
                    totalPoints = totalPoints,          // ✅ আসল DB মান
                    correctAnswers = totalCorrect,      // ✅ আসল quiz score যোগফল
                    streakDays = streakDays,            // ✅ আসল streak হিসাব
                    quizzesCompleted = quizzesCompleted,// ✅ আসল quiz সংখ্যা
                    courses = courses,
                    quizzes = quizzes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetProgressData error: " + ex.Message);
                Console.WriteLine("Stack trace: " + ex.StackTrace);

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

        // ============================================================
        // ===== Helper: Streak Days হিসাব =====
        // ইউজারের LessonCompletions ও QuizAttempts থেকে
        // কত দিন টানা অ্যাক্টিভ ছিল সেটা হিসাব করে।
        // ============================================================
        private int CalculateStreakDays(int userId, List<EnrolledCourseViewModel> enrolled)
        {
            try
            {
                var activityDates = new HashSet<DateTime>();

                // ১) Lesson completion dates সংগ্রহ
                foreach (var e in enrolled)
                {
                    var completedIds = _dbHelper.GetCompletedLessonIds(userId, e.CourseId);
                    // Note: GetCompletedLessonIds শুধু ID দেয়, তারিখ দেয় না।
                    // যদি তারিখও দরকার হয়, তবে একটি নতুন মেথড দরকার।
                    // আপাতত আমরা attempt dates ব্যবহার করছি।
                }

                // ২) Quiz attempt dates সংগ্রহ
                foreach (var e in enrolled)
                {
                    var attempts = _dbHelper.GetUserQuizAttempts(userId, e.CourseId);
                    foreach (var a in attempts)
                    {
                        activityDates.Add(a.AttemptedAt.Date);
                    }
                }

                // ৩) আজ থেকে পিছনে গুনে streak বের করা
                if (activityDates.Count == 0) return 0;

                int streak = 0;
                var today = DateTime.UtcNow.Date;

                // আজ অ্যাক্টিভ থাকলে streak শুরু হবে আজ থেকে,
                // না থাকলে গতকাল থেকে (কারণ আজ এখনো দিন শেষ হয়নি)
                var checkDate = activityDates.Contains(today)
                    ? today
                    : today.AddDays(-1);

                while (activityDates.Contains(checkDate))
                {
                    streak++;
                    checkDate = checkDate.AddDays(-1);
                }

                return streak;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CalculateStreakDays error: " + ex.Message);
                return 0;
            }
        }
    }
}