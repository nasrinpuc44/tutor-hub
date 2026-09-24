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
        // ===== Classroom পেজ (DB থেকে Video + Module List + Quiz + Assignment) =====
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

            // ৪) ইউজারের completed lessons (lesson id set)
            var completedLessonIds = _dbHelper.GetCompletedLessonIds(userId, courseId);

            // ৫) Module progress (module -> (Total, Completed))
            var moduleProgress = _dbHelper.GetModuleProgress(userId, courseId);

            // ৬) Published Quizzes (module-wise lookup)
            var quizzesByModule = _dbHelper.GetQuizzesByCourse(courseId)
                .Where(q => q.IsPublished)
                .ToDictionary(q => q.ModuleNumber, q => q);

            // ৭) Published Assignments (milestone-wise lookup)
            int modulesPerMilestone = _dbHelper.GetModulesPerMilestone(courseId);
            var assignmentsByMilestone = _dbHelper.GetAssignmentsByCourse(courseId)
                .Where(a => a.IsPublished)
                .ToDictionary(a => a.MilestoneNumber, a => a);

            // ৮) Completed Milestones হিসাব
            var completedMilestones = new List<int>();
            if (modulesPerMilestone > 0 && moduleProgress.Count > 0)
            {
                // ✅ FIX: মোট কতটি মডিউল আছে সেটি moduleProgress এবং dbLessons দুটো থেকেই বের করা
                int maxModule = dbLessons.Any()
                    ? dbLessons.Max(l => l.ModuleNumber)
                    : moduleProgress.Keys.DefaultIfEmpty(0).Max();

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

            // ৯) Module list তৈরি
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

                    int totalLessons = lessons.Count;
                    int completedCount = lessons.Count(l => completedLessonIds.Contains(l.Id));
                    bool moduleCompleted = totalLessons > 0 && completedCount >= totalLessons;

                    // ✅ FIX: Quiz available চেক — module completed এবং quiz published থাকলে available
                    bool quizAvailable = false;
                    int? quizId = null;
                    string? quizTitle = null;

                    if (moduleCompleted && quizzesByModule.TryGetValue(group.Key, out var quiz))
                    {
                        quizAvailable = true;
                        quizId = quiz.Id;
                        quizTitle = quiz.Title;
                    }

                    modules.Add(new ClassroomModule
                    {
                        ModuleNumber = group.Key,
                        Title = $"Module {group.Key}",
                        TotalDuration = $"{totalDuration} min",
                        CompletedLessons = completedCount,
                        TotalLessons = totalLessons,
                        Lessons = lessons.Select(l => new ClassroomLesson
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Duration = l.Duration,
                            VideoUrl = l.VideoUrl,
                            IsCompleted = completedLessonIds.Contains(l.Id)
                        }).ToList(),

                        // ✅ নতুন ফিল্ড
                        IsModuleCompleted = moduleCompleted,
                        QuizAvailable = quizAvailable,
                        QuizId = quizId,
                        QuizTitle = quizTitle
                    });
                }
            }
            else
            {
                // DB-তে কোনো লেসন নেই — static curriculum fallback
                modules = GetCourseCurriculum(courseId);
            }

            // ১০) Available Assignments (যেগুলোর Milestone complete হয়েছে)
            var availableAssignments = new List<MilestoneAssignment>();
            foreach (var ms in completedMilestones)
            {
                if (assignmentsByMilestone.TryGetValue(ms, out var assignment))
                {
                    availableAssignments.Add(assignment);
                }
            }

            // ১১) প্রথম লেসন active করো
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
                ActiveLessonDuration = firstLesson?.Duration ?? "",

                // ✅ নতুন
                AvailableAssignments = availableAssignments,
                ModulesPerMilestone = modulesPerMilestone,
                CompletedMilestones = completedMilestones
            };

            return View("classroom", model);
        }

        // ============================================================
        // ===== Lesson Complete মার্ক করা (AJAX) =====
        // POST: /MyClass/MarkLessonComplete
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkLessonComplete(int courseId, int lessonId, int moduleNumber)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
                return Json(new { success = false, message = "User not found" });

            // Enrolled কিনা চেক
            if (!_dbHelper.IsUserEnrolled(userId, courseId))
                return Json(new { success = false, message = "You are not enrolled in this course." });

            // Lesson complete mark
            bool ok = _dbHelper.MarkLessonCompleted(userId, courseId, lessonId, moduleNumber);
            if (!ok)
            {
                // Already completed হলেও success return করি (duplicate ignore)
                return Json(new
                {
                    success = true,
                    moduleCompleted = false,
                    quizId = (int?)null,
                    message = "Already completed"
                });
            }

            // Module complete হয়েছে কিনা চেক
            var progress = _dbHelper.GetModuleProgress(userId, courseId);
            bool moduleCompleted = progress.TryGetValue(moduleNumber, out var p)
                && p.Total > 0
                && p.Completed >= p.Total;

            // Module complete হলে Quiz available কিনা দেখি
            int? quizId = null;
            string? quizTitle = null;

            if (moduleCompleted)
            {
                var quizzes = _dbHelper.GetQuizzesByCourse(courseId);
                var quiz = quizzes.FirstOrDefault(x => x.ModuleNumber == moduleNumber && x.IsPublished);
                if (quiz != null)
                {
                    quizId = quiz.Id;
                    quizTitle = quiz.Title;
                }
            }

            // Milestone complete হয়েছে কিনা চেক
            int modulesPerMilestone = _dbHelper.GetModulesPerMilestone(courseId);
            bool milestoneCompleted = false;
            int? milestoneNumber = null;

            if (modulesPerMilestone > 0 && moduleCompleted)
            {
                // এই module যে milestone-এ পড়ে
                int ms = ((moduleNumber - 1) / modulesPerMilestone) + 1;

                // Milestone-এর সব module complete কিনা
                int startMod = (ms - 1) * modulesPerMilestone + 1;
                int endMod = ms * modulesPerMilestone;

                bool allComplete = true;
                for (int m = startMod; m <= endMod; m++)
                {
                    if (!progress.TryGetValue(m, out var pr) || pr.Total == 0 || pr.Completed < pr.Total)
                    {
                        allComplete = false;
                        break;
                    }
                }

                if (allComplete)
                {
                    milestoneCompleted = true;
                    milestoneNumber = ms;
                }
            }

            return Json(new
            {
                success = true,
                moduleCompleted = moduleCompleted,
                quizId = quizId,
                quizTitle = quizTitle,
                milestoneCompleted = milestoneCompleted,
                milestoneNumber = milestoneNumber
            });
        }

        // ============================================================
        // ===== Helper: Parse duration "6 min" → 6 =====
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