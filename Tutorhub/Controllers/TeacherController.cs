// 📁 Controllers/TeacherController.cs
// লোকেশন: Tutorbub/Controllers/TeacherController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;

namespace Tutorbub.Controllers
{
    public class TeacherController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public TeacherController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== টিচার ফর্ম দেখা =====
        [HttpGet]
        public IActionResult RequestForm()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var status = _dbHelper.GetUserTeacherRequestStatus(userId);

            if (status == "Pending")
            {
                TempData["Info"] = "You have already submitted a teacher request. Please wait for admin approval.";
                return RedirectToAction("Index", "Profile");
            }
            else if (status == "Approved")
            {
                TempData["Info"] = "You are already a teacher!";
                return RedirectToAction("TeacherDashboard");
            }

            var user = _dbHelper.GetUserById(userId);
            var model = new TeacherRequest
            {
                UserId = userId,
                FullName = user?.FullName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                MobileNumber = user?.MobileNumber ?? string.Empty
            };

            return View(model);
        }

        // ===== টিচার ফর্ম সাবমিট =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestForm(TeacherRequest model)
        {
            Console.WriteLine("=== Teacher Request Form POST ===");
            Console.WriteLine($"UserId: {model.UserId}");
            Console.WriteLine($"FullName: {model.FullName}");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Education: {model.Education}");
            Console.WriteLine($"SubjectExpertise: {model.SubjectExpertise}");

            if (HttpContext.Session.GetString("UserName") == null)
            {
                Console.WriteLine("User not logged in");
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                model.RequestDate = DateTime.UtcNow;
                model.Status = "Pending";

                Console.WriteLine("Calling CreateTeacherRequest...");
                bool success = _dbHelper.CreateTeacherRequest(model, out string? dbError);
                Console.WriteLine($"CreateTeacherRequest result: {success}");

                if (success)
                {
                    TempData["Success"] = "Your teacher request has been submitted successfully! Please wait for admin approval.";
                    Console.WriteLine("Success! Rendering RequestForm view directly so the success popup shows");
                    // NOTE: We intentionally do NOT redirect back to RequestForm() here.
                    // The GET RequestForm() action checks GetUserTeacherRequestStatus() first,
                    // and since the request we just created is "Pending", it would immediately
                    // redirect again to Profile/Index before this view (and its success popup
                    // script) ever gets a chance to render. Returning View(model) directly avoids
                    // that redirect chain and lets the success modal show right away.
                    return View(model);
                }
                else
                {
                    // ডেটাবেজ থেকে আসা আসল error message দেখানো হচ্ছে, যাতে বোঝা যায় ঠিক কী কারণে
                    // insert ব্যর্থ হলো (যেমন: টেবিল নেই, কলাম টাইপ মিসম্যাচ, কানেকশন সমস্যা ইত্যাদি)।
                    ViewBag.Error = string.IsNullOrWhiteSpace(dbError)
                        ? "Failed to submit request. Please try again."
                        : $"Failed to submit request. Database error: {dbError}";
                    Console.WriteLine("Failed! Error set in ViewBag: " + ViewBag.Error);
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid");
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList();

                foreach (var error in errors)
                {
                    Console.WriteLine($"Model Error: {error}");
                }

                // আগে এই ব্র্যাঞ্চে কোনো ViewBag.Error সেট হতো না, তাই ফর্ম ভ্যালিডেশন ব্যর্থ হলেও
                // ইউজার কোনো বার্তা ছাড়াই একই ফর্মে ফিরে যেত। এখন আসল কারণ দেখানো হচ্ছে।
                ViewBag.Error = errors.Any()
                    ? "Please fix the following: " + string.Join(" ", errors)
                    : "Please fill in all required fields correctly.";
            }

            return View(model);
        }

        // ===== টিচার ড্যাশবোর্ড =====
        public IActionResult TeacherDashboard()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(userId);
            if (user?.Role != "Teacher")
            {
                TempData["Error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Profile");
            }

            var model = new TeacherDashboardViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber ?? string.Empty,
                SubjectExpertise = "Not specified",
                Experience = "Not specified",
                ProfileImageLink = user.ProfileImageLink ?? string.Empty,
                TotalStudents = 0,
                TotalCourses = 0,
                CompletedSessions = 0,
                Rating = 0
            };

            return View(model);
        }
    }
}