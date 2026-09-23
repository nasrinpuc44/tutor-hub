// 📁 Controllers/NoticeController.cs
// লোকেশন: Tutorbub/Controllers/NoticeController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class NoticeController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public NoticeController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== Notice পেজ (ডেটাবেস থেকে) =====
        [HttpGet]
        public IActionResult Index(string? search = null, string? category = null)
        {
            // ✅ ডেটাবেস থেকে সব Notice লোড
            var allNotices = _dbHelper.GetAllNotices();

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.ToLower().Trim();
                allNotices = allNotices.Where(n =>
                    (n.Title ?? "").ToLower().Contains(q) ||
                    (n.Content ?? "").ToLower().Contains(q) ||
                    (n.Category ?? "").ToLower().Contains(q)
                ).ToList();
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(category) && category != "all")
            {
                allNotices = allNotices
                    .Where(n => (n.Category ?? "").Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // ✅ Announcements এবং News আলাদা করা
            var announcements = allNotices.Where(n => n.IsAnnouncement).ToList();
            var news = allNotices
                .Where(n => !n.IsAnnouncement)
                .OrderByDescending(n => n.PublishedDate)
                .ToList();

            ViewBag.Announcements = announcements;
            ViewBag.SearchQuery = search;
            ViewBag.SelectedCategory = category ?? "all";

            // 🔍 ডিবাগ লগ
            Console.WriteLine($"=== Notice Index ===");
            Console.WriteLine($"Total notices: {allNotices.Count}");
            Console.WriteLine($"Announcements: {announcements.Count}");
            Console.WriteLine($"News: {news.Count}");

            return View("Notice", news);
        }

        // ===== একটি Notice এর বিস্তারিত =====
        [HttpGet]
        public IActionResult Details(int id)
        {
            var notice = _dbHelper.GetNoticeById(id);
            if (notice == null)
            {
                TempData["Error"] = "Notice not found.";
                return RedirectToAction("Index");
            }

            // More News: বর্তমান notice বাদে বাকি সব news
            var allNotices = _dbHelper.GetAllNotices();
            var moreNews = allNotices
                .Where(n => !n.IsAnnouncement && n.Id != id)
                .OrderByDescending(n => n.PublishedDate)
                .Take(6)
                .ToList();

            ViewBag.MoreNews = moreNews;

            return View(notice);
        }
    }
}