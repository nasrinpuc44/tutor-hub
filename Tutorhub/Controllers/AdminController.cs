// 📁 Controllers/AdminController.cs
// লোকেশন: Tutorbub/Controllers/AdminController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;

namespace Tutorbub.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public AdminController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
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

            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = pendingCount;
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
            ViewBag.UserCount = users.Count;
            ViewBag.PendingCount = requests.Count(r => r.Status == "Pending");

            Console.WriteLine($"Total Teacher Requests: {requests.Count}");
            foreach (var req in requests)
            {
                Console.WriteLine($"Request: {req.FullName} - {req.Status} - {req.RequestDate}");
            }

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

            if (_dbHelper.UpdateTeacherRequest(id, status, adminNote))
            {
                if (action == "Approve")
                {
                    var request = _dbHelper.GetTeacherRequestById(id);
                    if (request != null)
                    {
                        _dbHelper.MakeUserTeacher(request.UserId);
                    }
                }
                return Json(new { success = true, message = $"Teacher request {action.ToLower()}d successfully" });
            }
            return Json(new { success = false, message = "Failed to process request" });
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
    }
}