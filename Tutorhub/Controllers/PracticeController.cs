// 📁 Controllers/PracticeController.cs
// লোকেশন: Tutorbub/Controllers/PracticeController.cs
// ⚠️ এই Controller এখন Course-ভিত্তিক Quiz সিস্টেম handle করে।
// ✅ ইউজার একটি কুইজে শুধুমাত্র একবারই অংশগ্রহণ করতে পারবে — Retake নেই।
// ✅ প্রতি সঠিক উত্তরে ১ পয়েন্ট করে ইউজার পাবে এবং Users টেবিলের TotalPoints-এ যোগ হবে।

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class PracticeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public PracticeController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ============================================================
        // ===== পুরনো Practice URL → My Class-এ পাঠানো =====
        // ============================================================
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            // Practice এখন My Class-এর ভিতরে — সরাসরি redirect
            return RedirectToAction("MyClass", "MyClass");
        }

        // ============================================================
        // ===== নির্দিষ্ট Course-এর সব Quiz ও Assignment দেখানো =====
        // GET: /Practice/CourseQuizzes?courseId=5
        // ============================================================
        [HttpGet]
        public IActionResult CourseQuizzes(int courseId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login", "Account");

            // ১) ইউজার এই কোর্সে enrolled কিনা চেক
            if (!_dbHelper.IsUserEnrolled(userId, courseId))
            {
                TempData["Error"] = "You are not enrolled in this course.";
                return RedirectToAction("MyClass", "MyClass");
            }

            // ২) কোর্স লোড
            var course = _dbHelper.GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("MyClass", "MyClass");
            }

            ViewBag.Course = course;
            ViewBag.CourseId = courseId;

            // ৩) সব published quiz ও assignment
            var quizzes = _dbHelper.GetQuizzesByCourse(courseId)
                .Where(q => q.IsPublished)
                .ToList();

            var assignments = _dbHelper.GetAssignmentsByCourse(courseId)
                .Where(a => a.IsPublished)
                .ToList();

            ViewBag.Quizzes = quizzes;
            ViewBag.Assignments = assignments;

            // ৪) ইউজারের module progress
            var moduleProgress = _dbHelper.GetModuleProgress(userId, courseId);
            var completedModules = moduleProgress
                .Where(kvp => kvp.Value.Total > 0 && kvp.Value.Completed >= kvp.Value.Total)
                .Select(kvp => kvp.Key)
                .ToList();

            ViewBag.ModuleProgress = moduleProgress;
            ViewBag.CompletedModules = completedModules;

            // ৫) Quiz Attempts
            var attempts = _dbHelper.GetUserQuizAttempts(userId, courseId);
            ViewBag.QuizAttempts = attempts;

            // ৬) Milestone config (admin define করেছে কতটি module = 1টি milestone)
            int modulesPerMilestone = _dbHelper.GetModulesPerMilestone(courseId);
            ViewBag.ModulesPerMilestone = modulesPerMilestone;

            // ৭) Completed Milestones হিসাব
            var completedMilestones = new List<int>();
            if (modulesPerMilestone > 0 && moduleProgress.Count > 0)
            {
                int maxModule = moduleProgress.Keys.DefaultIfEmpty(0).Max();
                int maxMilestone = maxModule / modulesPerMilestone;

                for (int ms = 1; ms <= maxMilestone; ms++)
                {
                    int startMod = (ms - 1) * modulesPerMilestone + 1;
                    int endMod = ms * modulesPerMilestone;

                    bool allComplete = true;
                    for (int m = startMod; m <= endMod; m++)
                    {
                        if (!moduleProgress.TryGetValue(m, out var prog)
                            || prog.Total == 0
                            || prog.Completed < prog.Total)
                        {
                            allComplete = false;
                            break;
                        }
                    }

                    if (allComplete) completedMilestones.Add(ms);
                }
            }
            ViewBag.CompletedMilestones = completedMilestones;

            // ৮) ইউজারের মোট পয়েন্ট (নতুন)
            ViewBag.UserTotalPoints = _dbHelper.GetUserPoints(userId);

            return View("CourseQuizzes");
        }

        // ============================================================
        // ===== Quiz নেওয়ার পেজ =====
        // GET: /Practice/TakeQuiz?quizId=5
        //
        // ✅ নতুন নিয়ম: ইউজার আগে এই কুইজে attempt করলে সরাসরি Result পেজে
        //    redirect করা হবে। একবার attempt = চিরতরে লক।
        // ============================================================
        [HttpGet]
        public IActionResult TakeQuiz(int quizId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return RedirectToAction("Login", "Account");

            // ১) Quiz লোড (Questions সহ)
            var quiz = _dbHelper.GetQuizById(quizId, includeQuestions: true);
            if (quiz == null || !quiz.IsPublished)
            {
                TempData["Error"] = "Quiz not found or not published.";
                return RedirectToAction("MyClass", "MyClass");
            }

            // ২) ইউজার Enrolled কিনা
            if (!_dbHelper.IsUserEnrolled(userId, quiz.CourseId))
            {
                TempData["Error"] = "You are not enrolled in this course.";
                return RedirectToAction("MyClass", "MyClass");
            }

            // ৩) Module Complete কিনা চেক
            var moduleProgress = _dbHelper.GetModuleProgress(userId, quiz.CourseId);
            bool moduleComplete = moduleProgress.TryGetValue(quiz.ModuleNumber, out var prog)
                && prog.Total > 0
                && prog.Completed >= prog.Total;

            if (!moduleComplete)
            {
                TempData["Error"] = "You must complete all lessons in this module first.";
                return RedirectToAction("CourseQuizzes", new { courseId = quiz.CourseId });
            }

            // ✅ ৪) NEW: ইউজার আগে এই কুইজে attempt করেছে কিনা চেক
            //    attempt থাকলে আর কুইজ দিতে পারবে না — সরাসরি Result পেজে পাঠানো হবে।
            var previousAttempt = _dbHelper.GetBestQuizAttempt(userId, quiz.Id);
            if (previousAttempt != null)
            {
                TempData["Info"] = "You have already attempted this quiz. Multiple attempts are not allowed.";
                return RedirectToAction("QuizResult", new { attemptId = previousAttempt.Id });
            }

            // ৫) Quiz-এ কোনো Question আছে কিনা
            if (quiz.Questions == null || quiz.Questions.Count == 0)
            {
                TempData["Error"] = "This quiz has no questions yet. Please contact admin.";
                return RedirectToAction("CourseQuizzes", new { courseId = quiz.CourseId });
            }

            // ৬) ViewModel তৈরি
            var vm = new TakeQuizViewModel
            {
                QuizId = quiz.Id,
                CourseId = quiz.CourseId,
                ModuleNumber = quiz.ModuleNumber,
                QuizTitle = quiz.Title,
                Description = quiz.Description,
                PassingScore = quiz.PassingScore,
                TimeLimitMinutes = quiz.TimeLimitMinutes,
                Questions = quiz.Questions,
                PreviousAttempt = null    // ✅ সবসময় null — attempt থাকলে এখানে আসতেই পারে না
            };

            return View(vm);
        }

        // ============================================================
        // ===== Quiz Submit =====
        // POST: /Practice/SubmitQuiz
        //
        // ✅ পয়েন্ট সিস্টেম:
        //    - প্রতি সঠিক উত্তরে ১ পয়েন্ট
        //    - Quiz submit হলে স্বয়ংক্রিয়ভাবে User.TotalPoints-এ যোগ হবে
        //
        // ✅ একবারই attempt — server-side double-submit চেক সহ
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitQuiz(int quizId, Dictionary<string, string> answers)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return RedirectToAction("Login", "Account");

            // ১) Quiz লোড
            var quiz = _dbHelper.GetQuizById(quizId, includeQuestions: true);
            if (quiz == null)
            {
                TempData["Error"] = "Quiz not found.";
                return RedirectToAction("MyClass", "MyClass");
            }

            // ২) Enrolled চেক
            if (!_dbHelper.IsUserEnrolled(userId, quiz.CourseId))
            {
                TempData["Error"] = "You are not enrolled in this course.";
                return RedirectToAction("MyClass", "MyClass");
            }

            // ✅ ৩) NEW: আগে attempt করেছে কিনা চেক — থাকলে submit করতে দেবে না
            var existingAttempt = _dbHelper.GetBestQuizAttempt(userId, quiz.Id);
            if (existingAttempt != null)
            {
                TempData["Info"] = "You have already attempted this quiz.";
                return RedirectToAction("QuizResult", new { attemptId = existingAttempt.Id });
            }

            // ৪) Score হিসাব
            int score = 0;              // মোট প্রাপ্ত মার্ক
            int correctCount = 0;       // সঠিক উত্তরের সংখ্যা (points award-এর জন্য)
            int totalMarks = 0;
            var submittedAnswers = answers ?? new Dictionary<string, string>();

            foreach (var q in quiz.Questions)
            {
                totalMarks += q.Marks;

                // form key format: "q_{questionId}"
                string key = $"q_{q.Id}";
                if (submittedAnswers.TryGetValue(key, out var ans))
                {
                    if (!string.IsNullOrEmpty(ans)
                        && ans.Equals(q.CorrectOption, StringComparison.OrdinalIgnoreCase))
                    {
                        score += q.Marks;
                        correctCount++;
                    }
                }
            }

            decimal pct = totalMarks > 0
                ? Math.Round((decimal)score / totalMarks * 100, 2)
                : 0;

            bool passed = pct >= quiz.PassingScore;

            // ৫) Attempt সেভ + পয়েন্ট অ্যাওয়ার্ড
            var attempt = new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                CourseId = quiz.CourseId,
                Score = score,
                TotalMarks = totalMarks,
                Percentage = pct,
                Passed = passed,
                AttemptedAt = DateTime.UtcNow
            };

            bool saved = false;
            string? saveError = null;

            try
            {
                saved = _dbHelper.SaveQuizAttempt(attempt, out saveError);

                if (saved && correctCount > 0)
                {
                    // প্রতি সঠিক উত্তরে ১ পয়েন্ট যোগ
                    _dbHelper.AddPoints(userId, correctCount);
                }
            }
            catch (Exception ex)
            {
                saveError = ex.Message;
                saved = false;
            }

            if (!saved)
            {
                TempData["Error"] = $"Failed to save quiz attempt. {saveError}";
                return RedirectToAction("CourseQuizzes", new { courseId = quiz.CourseId });
            }

            // ৬) ইউজারকে notification পাঠানো
            try
            {
                string title = passed ? "🎉 Quiz Passed!" : "📝 Quiz Attempt Recorded";
                string message = $"{quiz.Title} (Module {quiz.ModuleNumber}): {score}/{totalMarks} ({pct}%) " +
                                 $"• +{correctCount} points earned";
                string type = passed ? "success" : "warning";
                string icon = passed ? "fa-check-circle" : "fa-question-circle";

                _dbHelper.CreateNotification(
                    userId,
                    title,
                    message,
                    type,
                    $"/Practice/CourseQuizzes?courseId={quiz.CourseId}",
                    icon
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Notification error (non-fatal): " + ex.Message);
            }

            // ৭) Result পেজে যাওয়া
            TempData["QuizResult"] = passed ? "passed" : "failed";
            TempData["PointsEarned"] = correctCount;
            return RedirectToAction("QuizResult", new { attemptId = attempt.Id });
        }

        // ============================================================
        // ===== Quiz Result =====
        // GET: /Practice/QuizResult?attemptId=5
        // ============================================================
        [HttpGet]
        public IActionResult QuizResult(int attemptId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return RedirectToAction("Login", "Account");

            var attempt = _dbHelper.GetQuizAttemptById(attemptId, userId);
            if (attempt == null)
            {
                TempData["Error"] = "Result not found.";
                return RedirectToAction("MyClass", "MyClass");
            }

            // Quiz ও Course-এর info load করা
            var quiz = _dbHelper.GetQuizById(attempt.QuizId);
            if (quiz != null)
            {
                attempt.QuizTitle = quiz.Title;
                attempt.ModuleNumber = quiz.ModuleNumber;
            }
            else
            {
                attempt.QuizTitle = "Quiz";
            }

            // ইউজারের মোট পয়েন্ট (Result পেজে দেখানোর জন্য)
            ViewBag.UserTotalPoints = _dbHelper.GetUserPoints(userId);

            return View(attempt);
        }
    }
}