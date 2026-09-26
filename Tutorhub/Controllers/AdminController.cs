// 📁 Controllers/AdminController.cs
// লোকেশন: Tutorbub/Controllers/AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorbub.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _dbHelper = new DatabaseHelper(configuration);
            _webHostEnvironment = webHostEnvironment;
        }

        // ============================================================
        // ===== DASHBOARD =====
        // ============================================================
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var users = _dbHelper.GetAllUsers();
            var requests = _dbHelper.GetAllTeacherRequests();
            var pendingCount = requests.Count(r => r.Status == "Pending");

            var payStats = _dbHelper.GetPaymentStats();
            var submissionStats = _dbHelper.GetSubmissionStats();

            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = pendingCount;
            ViewBag.PendingPayments = payStats.Pending;
            ViewBag.TotalRevenue = payStats.TotalRevenue;
            ViewBag.PendingSubmissions = submissionStats.Pending;

            return View(users);
        }

        // ============================================================
        // ===== USER MANAGEMENT =====
        // ============================================================

        [HttpGet]
        public IActionResult UserDetails(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = _dbHelper.GetUserById(id);
            if (user != null)
            {
                return Json(new
                {
                    success = true,
                    user = new
                    {
                        user.Id,
                        user.UserName,
                        user.FullName,
                        user.Email,
                        user.Role,
                        user.IsActive,
                        user.MobileNumber,
                        user.Gender,
                        user.AgeRange,
                        user.PrimaryDeviceType,
                        user.YearsOfExperience,
                        user.AreaType,
                        user.Country,
                        user.StreetAddress,
                        user.PermanentAddress,
                        user.EducationLevel,
                        user.CurrentStudyStatus,
                        user.ExamDegreeTitle,
                        user.InstitutionName,
                        user.PassingYear,
                        user.IsCSEStudent,
                        user.CvLink,
                        user.GithubProfile,
                        user.PortfolioLink,
                        user.LinkedInProfile,
                        user.ProfileImageLink,
                        CreatedAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                        LastLoginAt = user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm") ?? "Never"
                    }
                });
            }
            return Json(new { success = false, message = "User not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            if (currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "Cannot delete your own account" });
            }

            if (_dbHelper.DeleteUser(id))
            {
                return Json(new { success = true, message = "User deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete user" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleUserStatus(int id, bool isActive)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            if (currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "Cannot disable your own account" });
            }

            if (_dbHelper.ToggleUserStatus(id, isActive))
            {
                if (isActive)
                {
                    _dbHelper.CreateNotification(
                        id,
                        "✅ Account Activated",
                        "Your account has been activated. You can now log in.",
                        "success",
                        null,
                        "fa-check-circle"
                    );
                }
                else
                {
                    _dbHelper.CreateNotification(
                        id,
                        "⚠️ Account Deactivated",
                        "Your account has been deactivated by an administrator.",
                        "warning",
                        null,
                        "fa-exclamation-triangle"
                    );
                }

                return Json(new { success = true, message = "User status updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update user status" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(int id, string newPassword)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                return Json(new { success = false, message = "Password must be at least 6 characters" });
            }

            if (_dbHelper.UpdatePassword(id, newPassword))
            {
                _dbHelper.CreateNotification(
                    id,
                    "🔑 Password Changed",
                    "Your account password was changed by an administrator.",
                    "warning",
                    null,
                    "fa-key"
                );

                return Json(new { success = true, message = "Password updated successfully" });
            }
            return Json(new { success = false, message = "Failed to update password" });
        }

        // ============================================================
        // ===== TEACHER REQUESTS =====
        // ============================================================

        public IActionResult TeacherRequests()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var requests = _dbHelper.GetAllTeacherRequests();
            var users = _dbHelper.GetAllUsers();
            var payStats = _dbHelper.GetPaymentStats();

            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = requests.Count(r => r.Status == "Pending");
            ViewBag.PendingPayments = payStats.Pending;

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessTeacherRequest(int id, string action, string? adminNote)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (action != "Approve" && action != "Reject")
            {
                return Json(new { success = false, message = "Invalid action" });
            }

            var status = action == "Approve" ? "Approved" : "Rejected";
            var request = _dbHelper.GetTeacherRequestById(id);

            if (_dbHelper.UpdateTeacherRequest(id, status, adminNote))
            {
                if (action == "Approve" && request != null)
                {
                    _dbHelper.MakeUserTeacher(request.UserId);

                    _dbHelper.CreateNotification(
                        request.UserId,
                        "🎓 You're now a Teacher!",
                        "Congratulations! Your teacher request has been approved. Access your Teacher Dashboard now.",
                        "success",
                        "/Teacher/TeacherDashboard",
                        "fa-chalkboard-teacher"
                    );
                }
                else if (action == "Reject" && request != null)
                {
                    var msg = string.IsNullOrEmpty(adminNote)
                        ? "Your teacher request was not approved. You can try again later."
                        : $"Your teacher request was not approved. Reason: {adminNote}";

                    _dbHelper.CreateNotification(
                        request.UserId,
                        "❌ Teacher Request Rejected",
                        msg,
                        "danger",
                        "/Teacher/RequestForm",
                        "fa-times-circle"
                    );
                }

                return Json(new { success = true, message = $"Teacher request {action.ToLower()}d successfully" });
            }
            return Json(new { success = false, message = "Failed to process request" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTeacherAccount(int id, int requestId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var currentUserId = HttpContext.Session.GetString("UserId");
            if (id > 0 && currentUserId == id.ToString())
            {
                return Json(new { success = false, message = "Cannot delete your own account" });
            }

            bool userDeleted = false;
            bool requestDeleted = false;

            try
            {
                if (id > 0)
                {
                    userDeleted = _dbHelper.DeleteUser(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTeacherAccount - error deleting user: " + ex.Message);
            }

            try
            {
                requestDeleted = _dbHelper.DeleteTeacherRequestById(requestId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTeacherAccount - error deleting request: " + ex.Message);
            }

            if (userDeleted || requestDeleted)
            {
                return Json(new { success = true, message = "Teacher deleted successfully" });
            }
            return Json(new { success = false, message = "Failed to delete: no matching user or request was found" });
        }

        [HttpGet]
        public IActionResult GetTeacherRequestDetails(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var request = _dbHelper.GetTeacherRequestById(id);
            if (request != null)
            {
                return Json(new
                {
                    success = true,
                    request = new
                    {
                        request.Id,
                        request.UserId,
                        request.FullName,
                        request.Email,
                        request.MobileNumber,
                        request.Education,
                        request.Institution,
                        request.SubjectExpertise,
                        request.Experience,
                        request.TeachingStyle,
                        request.AvailableDays,
                        request.PreferredTime,
                        request.HourlyRate,
                        request.CvLink,
                        request.WhyTeach,
                        request.Status,
                        RequestDate = request.RequestDate.ToString("dd MMM yyyy, HH:mm"),
                        request.AdminNote
                    }
                });
            }
            return Json(new { success = false, message = "Request not found" });
        }

        // ============================================================
        // ===== PAYMENT MANAGEMENT =====
        // ============================================================

        public IActionResult Payments(string status = "all")
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var payments = _dbHelper.GetAllOrders(status);
            var stats = _dbHelper.GetPaymentStats();
            var subStats = _dbHelper.GetSubmissionStats();

            ViewBag.PendingCount = stats.Pending;
            ViewBag.ApprovedCount = stats.Approved;
            ViewBag.RejectedCount = stats.Rejected;
            ViewBag.TotalRevenue = stats.TotalRevenue;
            ViewBag.CurrentFilter = status;
            ViewBag.PendingPayments = stats.Pending;
            ViewBag.PendingSubmissions = subStats.Pending;

            return View(payments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApprovePayment(int id, string? adminNote)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var adminId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            if (_dbHelper.ApproveOrder(id, adminId, adminNote))
            {
                var order = _dbHelper.GetOrderById(id);
                if (order != null)
                {
                    _dbHelper.CreateNotification(
                        order.UserId,
                        "🎉 Payment Approved!",
                        $"Your payment for \"{order.CourseName}\" has been approved. You can now access the course!",
                        "success",
                        $"/Learn/Details/{order.CourseId}",
                        "fa-check-circle"
                    );
                }

                return Json(new { success = true, message = "Payment approved successfully! User now has access to the course." });
            }
            return Json(new { success = false, message = "Failed to approve payment." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectPayment(int id, string? adminNote)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var adminId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            if (_dbHelper.RejectOrder(id, adminId, adminNote))
            {
                var order = _dbHelper.GetOrderById(id);
                if (order != null)
                {
                    var reason = string.IsNullOrEmpty(adminNote)
                        ? "Please check your transaction details and try again."
                        : $"Reason: {adminNote}";

                    _dbHelper.CreateNotification(
                        order.UserId,
                        "❌ Payment Rejected",
                        $"Your payment for \"{order.CourseName}\" was rejected. {reason}",
                        "danger",
                        $"/Payment/Checkout?courseId={order.CourseId}",
                        "fa-times-circle"
                    );
                }

                return Json(new { success = true, message = "Payment rejected." });
            }
            return Json(new { success = false, message = "Failed to reject payment." });
        }

        // ============================================================
        // ===== ADD COURSE =====
        // ============================================================

        [HttpGet]
        public IActionResult AddCourse()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCourse(Course model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.IsEnrollmentOpen = true;

                if (_dbHelper.CreateCourse(model, out string? error))
                {
                    TempData["Success"] = "Course added successfully!";
                    return RedirectToAction("AddCourse");
                }

                ViewBag.Error = $"Failed to add course: {error}";
            }

            return View(model);
        }

        // ============================================================
        // ===== UPLOAD COURSE IMAGE =====
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCourseImage(IFormFile courseImage)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (courseImage == null || courseImage.Length == 0)
            {
                return Json(new { success = false, message = "Please select an image." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(courseImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Only JPG, PNG, GIF, or WEBP images are allowed." });
            }

            if (courseImage.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "Image size must be less than 5MB." });
            }

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "courses");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = $"course_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await courseImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/courses/{fileName}";
                return Json(new { success = true, message = "Image uploaded successfully!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== UPLOAD COURSE LESSON =====
        // ============================================================

        [HttpGet]
        public IActionResult UploadLesson(int courseId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var course = _dbHelper.GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("ManageCourses");
            }

            ViewBag.Course = course;
            var lessons = _dbHelper.GetLessonsByCourseId(courseId);
            ViewBag.Lessons = lessons;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadLesson(int courseId, int moduleNumber, int lessonNumber,
            string title, string duration, string? description, string? videoUrl)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(new { success = false, message = "Lesson title is required." });
            }

            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                return Json(new { success = false, message = "Video URL is required." });
            }

            var lesson = new CourseLesson
            {
                CourseId = courseId,
                ModuleNumber = moduleNumber,
                LessonNumber = lessonNumber,
                Title = title.Trim(),
                VideoUrl = videoUrl.Trim(),
                Duration = duration?.Trim() ?? "",
                Description = description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            if (_dbHelper.CreateCourseLesson(lesson, out string? error))
            {
                return Json(new { success = true, message = "Lesson uploaded successfully!" });
            }

            return Json(new { success = false, message = $"Failed to save lesson: {error}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLesson(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (_dbHelper.DeleteCourseLesson(id))
            {
                return Json(new { success = true, message = "Lesson deleted successfully." });
            }
            return Json(new { success = false, message = "Failed to delete lesson." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLessonVideo(IFormFile lessonVideo)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (lessonVideo == null || lessonVideo.Length == 0)
            {
                return Json(new { success = false, message = "Please select a video file." });
            }

            var allowedExtensions = new[] { ".mp4", ".webm", ".ogg", ".mov", ".avi", ".mkv" };
            var extension = Path.GetExtension(lessonVideo.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Only MP4, WEBM, OGG, MOV, AVI, or MKV videos are allowed." });
            }

            if (lessonVideo.Length > 500 * 1024 * 1024)
            {
                return Json(new { success = false, message = "Video size must be less than 500MB." });
            }

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "videos");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = $"lesson_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await lessonVideo.CopyToAsync(stream);
                }

                var videoUrl = $"/uploads/videos/{fileName}";
                return Json(new { success = true, message = "Video uploaded successfully!", videoUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== MANAGE COURSES =====
        // ============================================================

        [HttpGet]
        public IActionResult ManageCourses()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var courses = _dbHelper.GetAllCourses();
            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCourse(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (_dbHelper.DeleteCourse(id))
            {
                return Json(new { success = true, message = "Course deleted successfully." });
            }
            return Json(new { success = false, message = "Failed to delete course." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleEnrollment(int id, bool isOpen)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (_dbHelper.ToggleEnrollment(id, isOpen))
            {
                var status = isOpen ? "opened" : "closed";
                return Json(new { success = true, message = $"Enrollment {status} successfully." });
            }
            return Json(new { success = false, message = "Failed to update enrollment status." });
        }

        // ============================================================
        // ===== NOTICE MANAGEMENT (NEWS & ANNOUNCEMENTS) =====
        // ============================================================

        [HttpGet]
        public IActionResult ManageNotices()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var notices = _dbHelper.GetAllNotices();
            return View(notices);
        }

        [HttpGet]
        public IActionResult AddNotice()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            return View(new Notice
            {
                PublishedDate = DateTime.Now,
                IsActive = true,
                IsAnnouncement = false
            });
        }

        // ============================================================
        // 🔧 AddNotice POST — Category-ভিত্তিক IsAnnouncement Force Fix
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNotice(Notice model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ViewBag.Error = "Title is required.";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ViewBag.Error = "Content is required.";
                return View(model);
            }

            // 🔧 ULTIMATE FIX: Category="News" হলে IsAnnouncement = false force
            if (string.Equals(model.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                model.IsAnnouncement = false;
                Console.WriteLine("🔧 FIX: Category=News → IsAnnouncement forced to FALSE");
            }

            // PublishedDate validation
            if (model.PublishedDate == default(DateTime) || model.PublishedDate.Year < 2000 || model.PublishedDate.Year > 2100)
            {
                model.PublishedDate = DateTime.Now;
            }

            Console.WriteLine("========== AddNotice POST ==========");
            Console.WriteLine($"Title: {model.Title}");
            Console.WriteLine($"Category: {model.Category}");
            Console.WriteLine($"IsAnnouncement: {model.IsAnnouncement}");
            Console.WriteLine($"IsActive: {model.IsActive}");
            Console.WriteLine($"PublishedDate: {model.PublishedDate}");
            Console.WriteLine("====================================");

            if (model.IsAnnouncement)
            {
                model.ImageUrl = string.Empty;
                model.IsActive = true;
            }

            model.CreatedAt = DateTime.UtcNow;

            if (_dbHelper.CreateNotice(model, out string? error))
            {
                TempData["Success"] = model.IsAnnouncement
                    ? "Announcement added successfully!"
                    : "News added successfully!";
                return RedirectToAction("ManageNotices");
            }

            ViewBag.Error = $"Failed to add notice: {error}";
            return View(model);
        }

        [HttpGet]
        public IActionResult EditNotice(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var notice = _dbHelper.GetNoticeById(id);
            if (notice == null)
            {
                TempData["Error"] = "Notice not found.";
                return RedirectToAction("ManageNotices");
            }

            return View(notice);
        }

        // ============================================================
        // 🔧 EditNotice POST — Category-ভিত্তিক IsAnnouncement Force Fix
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNotice(Notice model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                ViewBag.Error = "Title is required.";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                ViewBag.Error = "Content is required.";
                return View(model);
            }

            // 🔧 FIX: Category="News" হলে IsAnnouncement = false
            if (string.Equals(model.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                model.IsAnnouncement = false;
                Console.WriteLine("🔧 EDIT FIX: Category=News → IsAnnouncement forced to FALSE");
            }

            if (model.PublishedDate == default(DateTime) || model.PublishedDate.Year < 2000 || model.PublishedDate.Year > 2100)
            {
                model.PublishedDate = DateTime.Now;
            }

            Console.WriteLine("========== EditNotice POST ==========");
            Console.WriteLine($"Id: {model.Id}");
            Console.WriteLine($"Category: {model.Category}");
            Console.WriteLine($"IsAnnouncement: {model.IsAnnouncement}");
            Console.WriteLine("=====================================");

            if (model.IsAnnouncement)
            {
                model.ImageUrl = string.Empty;
                model.IsActive = true;
            }

            if (_dbHelper.UpdateNotice(model, out string? error))
            {
                TempData["Success"] = model.IsAnnouncement
                    ? "Announcement updated successfully!"
                    : "News updated successfully!";
                return RedirectToAction("ManageNotices");
            }

            ViewBag.Error = $"Failed to update notice: {error}";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNotice(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.DeleteNotice(id))
                return Json(new { success = true, message = "Notice deleted successfully." });

            return Json(new { success = false, message = "Failed to delete notice." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadNoticeImage(IFormFile noticeImage)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (noticeImage == null || noticeImage.Length == 0)
            {
                return Json(new { success = false, message = "Please select an image." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(noticeImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Only JPG, PNG, GIF, or WEBP images are allowed." });
            }

            if (noticeImage.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "Image size must be less than 5MB." });
            }

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "notices");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = $"notice_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await noticeImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/notices/{fileName}";
                return Json(new { success = true, message = "Image uploaded successfully!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ============================================================
        // ===== QUIZ & ASSIGNMENT OVERVIEW =====
        // ============================================================

        [HttpGet]
        public IActionResult QuizAssignmentOverview()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var courses = _dbHelper.GetAllCourses();

            var quizCounts = new Dictionary<int, int>();
            var assignCounts = new Dictionary<int, int>();

            foreach (var c in courses)
            {
                try
                {
                    quizCounts[c.Id] = _dbHelper.GetQuizzesByCourse(c.Id).Count;
                }
                catch
                {
                    quizCounts[c.Id] = 0;
                }

                try
                {
                    assignCounts[c.Id] = _dbHelper.GetAssignmentsByCourse(c.Id).Count;
                }
                catch
                {
                    assignCounts[c.Id] = 0;
                }
            }

            ViewBag.QuizCounts = quizCounts;
            ViewBag.AssignCounts = assignCounts;

            return View(courses);
        }

        // ============================================================
        // ===== COURSE QUIZ & ASSIGNMENT MANAGEMENT =====
        // ============================================================

        [HttpGet]
        public IActionResult ManageQuizAssignment(int courseId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var course = _dbHelper.GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("ManageCourses");
            }

            var vm = new CourseQuizAssignmentViewModel
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                Quizzes = _dbHelper.GetQuizzesByCourse(courseId),
                Assignments = _dbHelper.GetAssignmentsByCourse(courseId),
                ModulesPerMilestone = _dbHelper.GetModulesPerMilestone(courseId)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateQuiz(int courseId, int moduleNumber, string title,
            string? description, int passingScore, int timeLimitMinutes, bool isPublished)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(title))
                return Json(new { success = false, message = "Title is required." });

            var quiz = new ModuleQuiz
            {
                CourseId = courseId,
                ModuleNumber = moduleNumber,
                Title = title.Trim(),
                Description = description?.Trim(),
                PassingScore = passingScore,
                TimeLimitMinutes = timeLimitMinutes,
                IsPublished = isPublished
            };

            if (_dbHelper.CreateModuleQuiz(quiz, out string? error))
                return Json(new { success = true, message = "Quiz created successfully!", quizId = quiz.Id });

            return Json(new { success = false, message = $"Failed: {error}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleQuizPublish(int id, bool isPublished)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.ToggleQuizPublish(id, isPublished))
                return Json(new { success = true, message = isPublished ? "Quiz published." : "Quiz unpublished." });

            return Json(new { success = false, message = "Failed to update." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuiz(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.DeleteModuleQuiz(id))
                return Json(new { success = true, message = "Quiz deleted." });

            return Json(new { success = false, message = "Failed to delete." });
        }

        [HttpGet]
        public IActionResult ManageQuizQuestions(int quizId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var quiz = _dbHelper.GetQuizById(quizId, includeQuestions: true);
            if (quiz == null)
            {
                TempData["Error"] = "Quiz not found.";
                return RedirectToAction("ManageCourses");
            }

            var course = _dbHelper.GetCourseById(quiz.CourseId);
            ViewBag.Course = course;
            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuizQuestion(int quizId, string questionText,
            string optionA, string optionB, string optionC, string optionD,
            string correctOption, int marks, int questionOrder)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(questionText))
                return Json(new { success = false, message = "Question text is required." });

            var q = new QuizQuestionItem
            {
                QuizId = quizId,
                QuestionText = questionText.Trim(),
                OptionA = optionA?.Trim() ?? "",
                OptionB = optionB?.Trim() ?? "",
                OptionC = optionC?.Trim() ?? "",
                OptionD = optionD?.Trim() ?? "",
                CorrectOption = correctOption?.ToUpper() ?? "A",
                Marks = marks <= 0 ? 1 : marks,
                QuestionOrder = questionOrder
            };

            if (_dbHelper.AddQuizQuestion(q, out string? error))
                return Json(new { success = true, message = "Question added successfully!" });

            return Json(new { success = false, message = $"Failed: {error}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuizQuestion(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.DeleteQuizQuestion(id))
                return Json(new { success = true, message = "Question deleted." });

            return Json(new { success = false, message = "Failed to delete." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAssignment(int courseId, int milestoneNumber, string title,
            string? description, string? instructions, int totalMarks, int dueDays, bool isPublished)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(title))
                return Json(new { success = false, message = "Title is required." });

            var a = new MilestoneAssignment
            {
                CourseId = courseId,
                MilestoneNumber = milestoneNumber,
                Title = title.Trim(),
                Description = description?.Trim(),
                Instructions = instructions?.Trim(),
                TotalMarks = totalMarks,
                DueDays = dueDays,
                IsPublished = isPublished
            };

            if (_dbHelper.CreateMilestoneAssignment(a, out string? error))
            {
                Console.WriteLine("========== CreateAssignment ==========");
                Console.WriteLine($"CourseId: {courseId}");
                Console.WriteLine($"MilestoneNumber: {milestoneNumber}");
                Console.WriteLine($"Title: {title}");
                Console.WriteLine($"IsPublished: {isPublished}");
                Console.WriteLine($"Assignment Id: {a.Id}");
                Console.WriteLine("======================================");

                return Json(new { success = true, message = "Assignment created successfully!", assignmentId = a.Id });
            }

            return Json(new { success = false, message = $"Failed: {error}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleAssignmentPublish(int id, bool isPublished)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.ToggleAssignmentPublish(id, isPublished))
                return Json(new { success = true, message = isPublished ? "Assignment published." : "Assignment unpublished." });

            return Json(new { success = false, message = "Failed to update." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAssignment(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (_dbHelper.DeleteMilestoneAssignment(id))
                return Json(new { success = true, message = "Assignment deleted." });

            return Json(new { success = false, message = "Failed to delete." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetModulesPerMilestone(int courseId, int modulesPerMilestone)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            if (modulesPerMilestone < 1 || modulesPerMilestone > 20)
                return Json(new { success = false, message = "Modules per milestone must be between 1 and 20." });

            if (_dbHelper.SetModulesPerMilestone(courseId, modulesPerMilestone))
                return Json(new { success = true, message = "Configuration saved." });

            return Json(new { success = false, message = "Failed to save." });
        }

        // ============================================================
        // ===== ASSIGNMENT SUBMISSIONS (Admin) — NEW =====
        // ============================================================

        [HttpGet]
        public IActionResult Submissions(string status = "all", int? courseId = null, int? assignmentId = null)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction("Login", "Account");

            var submissions = _dbHelper.GetAllSubmissions(courseId, assignmentId, status);
            var stats = _dbHelper.GetSubmissionStats();

            ViewBag.TotalCount = stats.Total;
            ViewBag.PendingCount = stats.Pending;
            ViewBag.GradedCount = stats.Graded;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentCourseId = courseId;
            ViewBag.CurrentAssignmentId = assignmentId;

            return View(submissions);
        }

        [HttpGet]
        public IActionResult SubmissionDetails(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            var submission = _dbHelper.GetSubmissionById(id);
            if (submission == null)
                return Json(new { success = false, message = "Submission not found" });

            return Json(new
            {
                success = true,
                submission = new
                {
                    submission.Id,
                    submission.AssignmentId,
                    submission.UserId,
                    submission.CourseId,
                    submission.AssignmentTitle,
                    submission.MilestoneNumber,
                    submission.TotalMarks,
                    submission.CourseName,
                    submission.UserFullName,
                    submission.UserEmail,
                    submission.UserName,
                    submission.DriveLink,
                    submission.Note,
                    SubmittedAt = submission.SubmittedAt.ToString("dd MMM yyyy, hh:mm tt"),
                    submission.Marks,
                    submission.Feedback,
                    GradedAt = submission.GradedAt?.ToString("dd MMM yyyy, hh:mm tt"),
                    submission.Status
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GradeSubmission(int id, int marks, string? feedback)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Json(new { success = false, message = "Unauthorized" });

            var adminId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

            var submission = _dbHelper.GetSubmissionById(id);
            if (submission == null)
                return Json(new { success = false, message = "Submission not found" });

            if (marks < 0 || marks > submission.TotalMarks)
                return Json(new { success = false, message = $"Marks must be between 0 and {submission.TotalMarks}." });

            if (_dbHelper.GradeSubmission(id, marks, feedback, adminId))
            {
                // ============================================================
                // ✅ Student-কে Notification পাঠানো
                // ============================================================
                try
                {
                    var percentage = submission.TotalMarks > 0
                        ? (int)Math.Round((double)marks / submission.TotalMarks * 100)
                        : 0;

                    var passed = percentage >= 60;
                    var icon = passed ? "fa-trophy" : "fa-clipboard-check";
                    var type = passed ? "success" : "info";

                    var message = $"Your assignment \"{submission.AssignmentTitle}\" has been graded. " +
                                  $"You received {marks}/{submission.TotalMarks} ({percentage}%).";

                    if (!string.IsNullOrWhiteSpace(feedback))
                    {
                        message += $" Feedback: {feedback}";
                    }

                    _dbHelper.CreateNotification(
                        submission.UserId,
                        passed ? "🎉 Assignment Graded — Great Job!" : "📋 Assignment Graded",
                        message,
                        type,
                        $"/MyClass/SubmitAssignment?assignmentId={submission.AssignmentId}",
                        icon
                    );

                    Console.WriteLine($"✅ Notification sent to user {submission.UserId} for assignment {submission.AssignmentId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Notification error: " + ex.Message);
                }

                return Json(new { success = true, message = "Submission graded successfully!" });
            }

            return Json(new { success = false, message = "Failed to grade submission." });
        }

        // ============================================================
        // ===== SITE SETTINGS =====
        // ============================================================

        [HttpGet]
        public IActionResult Settings()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToAction("Login", "Account");

            var settings = _dbHelper.GetSiteSettings();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSettings(SiteSettings model)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (model.SliderIntervalSeconds < 1)
                model.SliderIntervalSeconds = 5;
            if (model.SliderIntervalSeconds > 60)
                model.SliderIntervalSeconds = 60;

            if (_dbHelper.SaveSiteSettings(model, out string? error))
            {
                return Json(new { success = true, message = "Settings saved successfully!" });
            }

            return Json(new { success = false, message = $"Failed: {error}" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHeroImage(IFormFile heroImage)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            if (heroImage == null || heroImage.Length == 0)
                return Json(new { success = false, message = "Please select an image." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(heroImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "Only JPG, PNG, GIF, or WEBP images are allowed." });

            if (heroImage.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "Image size must be less than 5MB." });

            try
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "hero");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = $"hero_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await heroImage.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/hero/{fileName}";
                return Json(new { success = true, message = "Image uploaded!", imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}