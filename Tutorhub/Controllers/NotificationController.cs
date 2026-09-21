// 📁 Controllers/NotificationController.cs
// লোকেশন: Tutorbub/Controllers/NotificationController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class NotificationController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public NotificationController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== ইউজারের সব নোটিফিকেশন (JSON) =====
        [HttpGet]
        public IActionResult GetNotifications()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, notifications = new object[0], unreadCount = 0 });
            }

            var notifications = _dbHelper.GetUserNotifications(userId, 20);
            var unreadCount = _dbHelper.GetUnreadNotificationCount(userId);

            return Json(new
            {
                success = true,
                unreadCount = unreadCount,
                notifications = notifications.Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.Type,
                    n.Icon,
                    n.Link,
                    n.IsRead,
                    TimeAgo = GetTimeAgo(n.CreatedAt),
                    CreatedAt = n.CreatedAt.ToString("dd MMM yyyy, hh:mm tt")
                })
            });
        }

        // ===== অপঠিত কাউন্ট =====
        [HttpGet]
        public IActionResult GetUnreadCount()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, count = 0 });
            }

            var count = _dbHelper.GetUnreadNotificationCount(userId);
            return Json(new { success = true, count });
        }

        // ===== একটি নোটিফিকেশন Read মার্ক =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsRead(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (_dbHelper.MarkNotificationAsRead(id, userId))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // ===== সব Read মার্ক =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllAsRead()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (_dbHelper.MarkAllNotificationsAsRead(userId))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // ===== সব ক্লিয়ার =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAll()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            if (_dbHelper.ClearUserNotifications(userId))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // ===== সব নোটিফিকেশন পেজ =====
        [HttpGet]
        public IActionResult All()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var notifications = _dbHelper.GetUserNotifications(userId, 100);

            // সব read মার্ক করা যখন সব দেখতে যাই
            _dbHelper.MarkAllNotificationsAsRead(userId);

            return View(notifications);
        }

        // ===== হেল্পার: Time Ago =====
        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)}w ago";

            return dateTime.ToString("dd MMM");
        }
    }
}