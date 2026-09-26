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
        // ===== Classroom পেজ =====
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

            // ১) Enrolled check
            if (!_dbHelper.IsUserEnrolled(userId, courseId))
            {
                TempData["Error"] = "You are not enrolled in this course.";
                return RedirectToAction("MyClass");
            }

            // ২) Course meta
            var course = _dbHelper.GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("MyClass");
            }

            // ৩) Lessons from DB
            var dbLessons = _dbHelper.GetLessonsByCourseId(courseId);

            // ৪) Completed lesson IDs
            var completedLessonIds = _dbHelper.GetCompletedLessonIds(userId, courseId);

            // ৫) Module progress
            var moduleProgress = _dbHelper.GetModuleProgress(userId, courseId);

            // ৬) Published quizzes
            var quizzesByModule = _dbHelper.GetQuizzesByCourse(courseId)
                .Where(q => q.IsPublished)
                .ToDictionary(q => q.ModuleNumber, q => q);

            // ৭) Published assignments
            int modulesPerMilestone = _dbHelper.GetModulesPerMilestone(courseId);
            var assignmentsByMilestone = _dbHelper.GetAssignmentsByCourse(courseId)
                .Where(a => a.IsPublished)
                .ToDictionary(a => a.MilestoneNumber, a => a);

            // ============================================================
            // ৮) Completed Milestones
            // ============================================================
            var completedMilestones = new List<int>();

            Console.WriteLine("");
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  MILESTONE DETECTION — Course ID: {courseId}");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");

            if (modulesPerMilestone > 0 && moduleProgress.Count > 0)
            {
                int maxModule = moduleProgress.Keys.DefaultIfEmpty(0).Max();

                if (dbLessons.Any())
                {
                    int dbMaxModule = dbLessons.Max(l => l.ModuleNumber);
                    maxModule = Math.Max(maxModule, dbMaxModule);
                }

                int maxMilestone = (int)Math.Ceiling((double)maxModule / modulesPerMilestone);

                Console.WriteLine($"  Max Module: {maxModule}");
                Console.WriteLine($"  Modules Per Milestone: {modulesPerMilestone}");
                Console.WriteLine($"  Max Milestone: {maxMilestone}");
                Console.WriteLine("");

                for (int ms = 1; ms <= maxMilestone; ms++)
                {
                    int startMod = (ms - 1) * modulesPerMilestone + 1;
                    int endMod = ms * modulesPerMilestone;

                    bool allComplete = true;

                    for (int m = startMod; m <= endMod; m++)
                    {
                        if (moduleProgress.TryGetValue(m, out var prog))
                        {
                            if (prog.Total > 0 && prog.Completed < prog.Total)
                            {
                                allComplete = false;
                                break;
                            }
                        }
                        else
                        {
                            bool hasLessons = dbLessons.Any(l => l.ModuleNumber == m);
                            if (hasLessons)
                            {
                                allComplete = false;
                                break;
                            }
                        }
                    }

                    if (allComplete)
                    {
                        completedMilestones.Add(ms);
                        Console.WriteLine($"  ✅ Milestone {ms} COMPLETED");
                    }
                }
            }

            Console.WriteLine($"  FINAL Completed: [{string.Join(", ", completedMilestones)}]");
            Console.WriteLine("");

            // ৯) Module list তৈরি
            List<ClassroomModule> modules;

            if (dbLessons.Count > 0)
            {
                modules = new List<ClassroomModule>();
                var grouped = dbLessons.GroupBy(l => l.ModuleNumber).OrderBy(g => g.Key);

                foreach (var group in grouped)
                {
                    var lessons = group.OrderBy(l => l.LessonNumber).ToList();
                    int totalDuration = lessons.Sum(l => ParseDuration(l.Duration));

                    int totalLessons = lessons.Count;
                    int completedCount = lessons.Count(l => completedLessonIds.Contains(l.Id));
                    bool moduleCompleted = totalLessons > 0 && completedCount >= totalLessons;

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
                        IsModuleCompleted = moduleCompleted,
                        QuizAvailable = quizAvailable,
                        QuizId = quizId,
                        QuizTitle = quizTitle
                    });
                }
            }
            else
            {
                modules = GetCourseCurriculum(courseId);
            }

            // ১০) Available Assignments
            var availableAssignments = new List<MilestoneAssignment>();

            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  AVAILABLE ASSIGNMENTS");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");

            foreach (var ms in completedMilestones)
            {
                if (assignmentsByMilestone.TryGetValue(ms, out var assignment))
                {
                    availableAssignments.Add(assignment);
                    Console.WriteLine($"  ✅ Added: Milestone {ms} → \"{assignment.Title}\"");
                }
            }

            Console.WriteLine($"  Total Available: {availableAssignments.Count}");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine("");

            // ১১) First lesson active
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
                AvailableAssignments = availableAssignments,
                ModulesPerMilestone = modulesPerMilestone,
                CompletedMilestones = completedMilestones
            };

            return View("classroom", model);
        }

        // ============================================================
        // ===== Lesson Complete Mark (AJAX) =====
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

            if (!_dbHelper.IsUserEnrolled(userId, courseId))
                return Json(new { success = false, message = "You are not enrolled in this course." });

            bool ok = _dbHelper.MarkLessonCompleted(userId, courseId, lessonId, moduleNumber);
            if (!ok)
            {
                return Json(new
                {
                    success = true,
                    moduleCompleted = false,
                    quizId = (int?)null,
                    message = "Already completed"
                });
            }

            var progress = _dbHelper.GetModuleProgress(userId, courseId);
            bool moduleCompleted = progress.TryGetValue(moduleNumber, out var p)
                && p.Total > 0
                && p.Completed >= p.Total;

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

            // Milestone complete check
            int modulesPerMilestone = _dbHelper.GetModulesPerMilestone(courseId);
            bool milestoneCompleted = false;
            int? milestoneNumber = null;

            if (modulesPerMilestone > 0 && moduleCompleted)
            {
                int ms = ((moduleNumber - 1) / modulesPerMilestone) + 1;
                int startMod = (ms - 1) * modulesPerMilestone + 1;
                int endMod = ms * modulesPerMilestone;

                var dbLessons = _dbHelper.GetLessonsByCourseId(courseId);

                bool allComplete = true;
                for (int m = startMod; m <= endMod; m++)
                {
                    if (progress.TryGetValue(m, out var pr))
                    {
                        if (pr.Total > 0 && pr.Completed < pr.Total)
                        {
                            allComplete = false;
                            break;
                        }
                    }
                    else
                    {
                        bool hasLessons = dbLessons.Any(l => l.ModuleNumber == m);
                        if (hasLessons)
                        {
                            allComplete = false;
                            break;
                        }
                    }
                }

                if (allComplete)
                {
                    milestoneCompleted = true;
                    milestoneNumber = ms;
                    Console.WriteLine($"🎉 MILESTONE {ms} COMPLETED for user {userId}");
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
        // ===== ASSIGNMENT — GET (Submission Page) =====
        // ============================================================
        [HttpGet]
        public IActionResult SubmitAssignment(int assignmentId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return RedirectToAction("Login", "Account");

            var assignment = _dbHelper.GetAssignmentById(assignmentId);
            if (assignment == null)
            {
                TempData["Error"] = "Assignment not found.";
                return RedirectToAction("MyClass");
            }

            if (!_dbHelper.IsUserEnrolled(userId, assignment.CourseId))
            {
                TempData["Error"] = "You are not enrolled in this course.";
                return RedirectToAction("MyClass");
            }

            var existing = _dbHelper.GetUserSubmission(userId, assignmentId);

            ViewBag.Assignment = assignment;
            ViewBag.Course = _dbHelper.GetCourseById(assignment.CourseId);

            return View("SubmitAssignment", existing ?? new AssignmentSubmission
            {
                AssignmentId = assignmentId,
                UserId = userId,
                CourseId = assignment.CourseId
            });
        }

        // ============================================================
        // ===== ASSIGNMENT — POST (Submit / Resubmit) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitAssignment(int assignmentId, string driveLink, string? note)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return Json(new { success = false, message = "User not found" });

            if (string.IsNullOrWhiteSpace(driveLink))
                return Json(new { success = false, message = "Google Drive link is required." });

            if (!driveLink.Contains("drive.google.com") && !driveLink.Contains("docs.google.com"))
                return Json(new { success = false, message = "Please provide a valid Google Drive link." });

            var assignment = _dbHelper.GetAssignmentById(assignmentId);
            if (assignment == null)
                return Json(new { success = false, message = "Assignment not found." });

            if (!_dbHelper.IsUserEnrolled(userId, assignment.CourseId))
                return Json(new { success = false, message = "You are not enrolled in this course." });

            // ✅ Late check
            var (isLate, daysLate, lateCost, lateMessage) = _dbHelper.CheckLateSubmission(assignmentId, DateTime.UtcNow);
            if (isLate)
            {
                int userPoints = _dbHelper.GetUserPoints(userId);
                if (userPoints < lateCost)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Late submission requires {lateCost} points. You have {userPoints}. Take quizzes to earn more.",
                        isLate = true,
                        requiredPoints = lateCost,
                        userPoints = userPoints
                    });
                }
            }

            var submission = new AssignmentSubmission
            {
                AssignmentId = assignmentId,
                UserId = userId,
                CourseId = assignment.CourseId,
                DriveLink = driveLink.Trim(),
                Note = note?.Trim()
            };

            if (_dbHelper.CreateAssignmentSubmission(submission, out string? error))
            {
                // Notify admins
                try
                {
                    var admins = _dbHelper.GetAllUsers().Where(u => u.Role == "Admin").ToList();
                    var user = _dbHelper.GetUserById(userId);
                    foreach (var admin in admins)
                    {
                        _dbHelper.CreateNotification(
                            admin.Id,
                            isLate ? "📝 Late Assignment Submission" : "📝 New Assignment Submission",
                            $"{user?.FullName ?? "A student"} submitted \"{assignment.Title}\"" +
                                (isLate ? $" ({daysLate} days late, {lateCost} pts deducted)" : ""),
                            isLate ? "warning" : "info",
                            "/Admin/Submissions",
                            "fa-file-upload"
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Admin notification error: " + ex.Message);
                }

                // Student self notification (if late)
                if (isLate)
                {
                    try
                    {
                        _dbHelper.CreateNotification(
                            userId,
                            "⏰ Late Submission - Points Deducted",
                            $"{lateCost} points deducted for submitting \"{assignment.Title}\" {daysLate} days late.",
                            "warning",
                            null,
                            "fa-clock"
                        );
                    }
                    catch { }
                }

                return Json(new
                {
                    success = true,
                    message = isLate
                        ? $"Assignment submitted late. {lateCost} points deducted."
                        : "Assignment submitted successfully!",
                    isLate = isLate,
                    pointsDeducted = isLate ? lateCost : 0
                });
            }

            return Json(new { success = false, message = $"Failed: {error}" });
        }

        // ============================================================
        // ===== RESUBMIT WITH POINTS (50 pts) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResubmitWithPoints(int assignmentId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return Json(new { success = false, message = "User not found" });

            var assignment = _dbHelper.GetAssignmentById(assignmentId);
            if (assignment == null)
                return Json(new { success = false, message = "Assignment not found." });

            if (!_dbHelper.IsUserEnrolled(userId, assignment.CourseId))
                return Json(new { success = false, message = "You are not enrolled in this course." });

            if (_dbHelper.ResubmitAssignmentWithPoints(userId, assignmentId, out string? error))
            {
                int newPoints = _dbHelper.GetUserPoints(userId);

                // Notification (self)
                try
                {
                    _dbHelper.CreateNotification(
                        userId,
                        "🔄 Assignment Resubmit Allowed",
                        $"You spent {AssignmentSubmission.RESUBMIT_COST} points to resubmit \"{assignment.Title}\". You now have {newPoints} points. Submit your updated work!",
                        "info",
                        $"/MyClass/SubmitAssignment?assignmentId={assignmentId}",
                        "fa-redo"
                    );
                }
                catch { }

                return Json(new
                {
                    success = true,
                    message = $"Successfully spent {AssignmentSubmission.RESUBMIT_COST} points. You can now resubmit!",
                    newPoints = newPoints,
                    redirectUrl = $"/MyClass/SubmitAssignment?assignmentId={assignmentId}"
                });
            }

            return Json(new { success = false, message = error ?? "Failed to resubmit." });
        }

        // ============================================================
        // ===== REQUEST RECHECK (50 pts) =====
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestRecheck(int assignmentId, string? reason)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false, message = "Unauthorized" });

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return Json(new { success = false, message = "User not found" });

            var assignment = _dbHelper.GetAssignmentById(assignmentId);
            if (assignment == null)
                return Json(new { success = false, message = "Assignment not found." });

            if (!_dbHelper.IsUserEnrolled(userId, assignment.CourseId))
                return Json(new { success = false, message = "You are not enrolled in this course." });

            if (_dbHelper.RequestRecheckWithPoints(userId, assignmentId, reason, out string? error))
            {
                // Notify admins
                try
                {
                    var admins = _dbHelper.GetAllUsers().Where(u => u.Role == "Admin").ToList();
                    var user = _dbHelper.GetUserById(userId);
                    foreach (var admin in admins)
                    {
                        _dbHelper.CreateNotification(
                            admin.Id,
                            "🔍 Recheck Requested",
                            $"{user?.FullName ?? "A student"} requested recheck for \"{assignment.Title}\". Reason: {reason ?? "N/A"}",
                            "warning",
                            "/Admin/Submissions",
                            "fa-search"
                        );
                    }
                }
                catch { }

                int newPoints = _dbHelper.GetUserPoints(userId);

                return Json(new
                {
                    success = true,
                    message = $"Recheck requested! {AssignmentSubmission.RECHECK_COST} points deducted.",
                    newPoints = newPoints
                });
            }

            return Json(new { success = false, message = error ?? "Failed to request recheck." });
        }

        // ============================================================
        // ===== CHECK LATE STATUS (AJAX) =====
        // ============================================================
        [HttpGet]
        public IActionResult CheckLateStatus(int assignmentId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false });

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var (isLate, daysLate, cost, message) = _dbHelper.CheckLateSubmission(assignmentId, DateTime.UtcNow);

            int userPoints = userId > 0 ? _dbHelper.GetUserPoints(userId) : 0;

            return Json(new
            {
                success = true,
                isLate = isLate,
                daysLate = daysLate,
                pointsCost = cost,
                userPoints = userPoints,
                canAfford = userPoints >= cost,
                message = message
            });
        }

        // ============================================================
        // ===== GET POINTS INFO (AJAX) =====
        // ============================================================
        [HttpGet]
        public IActionResult GetPointsInfo(int assignmentId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return Json(new { success = false });

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0) return Json(new { success = false });

            int userPoints = _dbHelper.GetUserPoints(userId);
            var resubmit = _dbHelper.CheckResubmitEligibility(userId, assignmentId);
            var recheck = _dbHelper.CheckRecheckEligibility(userId, assignmentId);
            var late = _dbHelper.CheckLateSubmission(assignmentId, DateTime.UtcNow);

            return Json(new
            {
                success = true,
                userPoints = userPoints,
                resubmitCost = AssignmentSubmission.RESUBMIT_COST,
                recheckCost = AssignmentSubmission.RECHECK_COST,
                lateCost = AssignmentSubmission.LATE_SUBMIT_COST,
                canResubmit = resubmit.CanResubmit,
                resubmitReason = resubmit.Reason,
                canRecheck = recheck.CanRecheck,
                recheckReason = recheck.Reason,
                isLate = late.IsLate,
                daysLate = late.DaysLate,
                lateMessage = late.Message
            });
        }

        // ============================================================
        // ===== Helper: Parse Duration =====
        // ============================================================
        private static int ParseDuration(string duration)
        {
            if (string.IsNullOrEmpty(duration)) return 0;
            var digits = new string(duration.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out int result) ? result : 0;
        }

        // ============================================================
        // ===== Static Curriculum (Fallback) =====
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