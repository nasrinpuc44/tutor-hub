// 📁 Controllers/AdminController.cs
// লোকেশন: Tutorbub/Controllers/AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
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

        // ===== ড্যাশবোর্ড =====
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

            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = pendingCount;
            ViewBag.PendingPayments = payStats.Pending;
            ViewBag.TotalRevenue = payStats.TotalRevenue;

            return View(users);
        }

        // ===== ইউজার ডিটেইলস =====
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

        // ===== ইউজার ডিলিট =====
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

        // ===== ইউজার স্ট্যাটাস টগল =====
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

        // ===== পাসওয়ার্ড আপডেট =====
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

        // ===== Teacher Request দেখা =====
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

        // ===== Teacher Request অ্যাপ্রুভ/রিজেক্ট =====
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

        // ===== Teacher ডিলিট =====
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

        // ===== Teacher Request ডিটেইলস =====
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

            ViewBag.PendingCount = stats.Pending;
            ViewBag.ApprovedCount = stats.Approved;
            ViewBag.RejectedCount = stats.Rejected;
            ViewBag.TotalRevenue = stats.TotalRevenue;
            ViewBag.CurrentFilter = status;
            ViewBag.PendingPayments = stats.Pending;

            return View(payments);
        }

        // ===== পেমেন্ট অ্যাপ্রুভ =====
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

        // ===== পেমেন্ট রিজেক্ট =====
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
        // ===== UPLOAD COURSE LESSON VIDEO (নতুন) =====
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

        // ===== Upload Video File =====
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

        // ===== কোর্স ডিলিট =====
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

        // ===== Enrollment Open/Close =====
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
    }
}