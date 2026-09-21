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
                // ✅ ইউজারকে নোটিফিকেশন পাঠান
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
                // ✅ ইউজারকে নোটিফিকেশন
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

                    // ✅ ইউজারকে নোটিফিকেশন
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
                    // ✅ ইউজারকে নোটিফিকেশন
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

<<<<<<< HEAD
        // ===== Teacher ডিলিট (ইউজার অ্যাকাউন্ট + Request দুটোই) =====
        // এখানে ইউজার আইডি (id) এবং request আইডি (requestId) দুটোই পাঠানো হয়।
        // ইউজার অ্যাকাউন্ট ডিলিটের চেষ্টা করা হয় (থাকলে তার সব request-ও
        // ক্যাসকেড হয়ে যায়), এবং আলাদাভাবে এই নির্দিষ্ট request রো-টাও সরাসরি
        // ডিলিট করার চেষ্টা করা হয় — যাতে ইউজার না থাকলেও (যেমন পুরনো/টেস্ট
        // ডেটা) কার্ডটা ঠিকই লিস্ট থেকে মুছে যায়।
=======
        // ===== Teacher ডিলিট =====
>>>>>>> 9c75139 (add new update)
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
                // ✅ ইউজারকে নোটিফিকেশন
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
                // ✅ ইউজারকে নোটিফিকেশন
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
    }
}