// 📁 Controllers/PaymentController.cs
// লোকেশন: Tutorbub/Controllers/PaymentController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public PaymentController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== পেমেন্ট পেজ দেখানো =====
        [HttpGet]
        public IActionResult Checkout(int courseId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                TempData["Error"] = "Please login to enroll in this course.";
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (_dbHelper.IsUserEnrolled(userId, courseId))
            {
                TempData["Info"] = "You are already enrolled in this course!";
                return RedirectToAction("Details", "Learn", new { id = courseId });
            }

            var course = GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index", "Learn");
            }

            var model = new PaymentViewModel
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                CourseImage = course.Image,
                Price = course.Price,
                Instructor = course.Instructor,
                Duration = course.Duration
            };

            return View(model);
        }

        // ===== পেমেন্ট সাবমিট =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitPayment(PaymentViewModel model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                TempData["Error"] = "Please login to continue.";
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(model.PaymentMethod))
            {
                TempData["Error"] = "Please select a payment method.";
                return RedirectToAction("Checkout", new { courseId = model.CourseId });
            }

            if (string.IsNullOrWhiteSpace(model.TransactionId))
            {
                TempData["Error"] = "Please enter the transaction ID.";
                return RedirectToAction("Checkout", new { courseId = model.CourseId });
            }

            if (string.IsNullOrWhiteSpace(model.SenderMobileNumber))
            {
                TempData["Error"] = "Please enter the mobile number you paid from.";
                return RedirectToAction("Checkout", new { courseId = model.CourseId });
            }

            var mobileRegex = new System.Text.RegularExpressions.Regex(@"^01[3-9]\d{8}$");
            if (!mobileRegex.IsMatch(model.SenderMobileNumber))
            {
                TempData["Error"] = "Please enter a valid Bangladeshi mobile number (e.g., 01712345678).";
                return RedirectToAction("Checkout", new { courseId = model.CourseId });
            }

            if (_dbHelper.IsUserEnrolled(userId, model.CourseId))
            {
                TempData["Info"] = "You are already enrolled in this course!";
                return RedirectToAction("Details", "Learn", new { id = model.CourseId });
            }

            var course = GetCourseById(model.CourseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index", "Learn");
            }

            var order = new CourseOrder
            {
                UserId = userId,
                CourseId = model.CourseId,
                CourseName = course.Title,
                CourseImage = course.Image,
                Price = course.Price,
                PaymentMethod = model.PaymentMethod,
                TransactionId = model.TransactionId.Trim().ToUpper(),
                SenderMobileNumber = model.SenderMobileNumber.Trim(),
                PaymentStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            if (_dbHelper.CreateCourseOrder(order, out string? error))
            {
                TempData["Success"] = "Payment submitted successfully! Please wait for admin verification.";
                return RedirectToAction("PaymentStatus", new { orderId = order.Id });
            }

            TempData["Error"] = $"Failed to submit payment. {error}";
            return RedirectToAction("Checkout", new { courseId = model.CourseId });
        }

        // ===== পেমেন্ট স্ট্যাটাস পেজ =====
        [HttpGet]
        public IActionResult PaymentStatus(int orderId)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var order = _dbHelper.GetOrderById(orderId);

            if (order == null || order.UserId != userId)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        // ===== আমার অর্ডার লিস্ট =====
        [HttpGet]
        public IActionResult MyOrders()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            var orders = _dbHelper.GetUserOrders(userId);

            return View(orders);
        }

        // ===== হেল্পার: কোর্স ডেটা =====
        private CourseDetailViewModel? GetCourseById(int id)
        {
            var courses = new List<CourseDetailViewModel>
            {
                new CourseDetailViewModel
                {
                    Id = 1,
                    Title = "English Grammar Course",
                    Description = "Master English grammar with real certificates and interactive exercises.",
                    Image = "https://i.ibb.co.com/KjyJyXy8/English-grammar-courses-online-with-real-certificates.jpg",
                    Category = "Language",
                    Price = 1499,
                    Students = 12500,
                    Rating = 4.9,
                    Instructor = "Ms. Farhana Akter",
                    Duration = "12 Weeks",
                    Lessons = 48,
                    Level = "Beginner to Intermediate"
                },
                new CourseDetailViewModel
                {
                    Id = 2,
                    Title = "Basic WordPress Theme Development",
                    Description = "Full course from beginner to expert — build custom WordPress themes.",
                    Image = "https://i.ibb.co.com/ZpRSvpsk/Basic-Word-Press-theme-development-full-course.jpg",
                    Category = "Web Development",
                    Price = 1999,
                    Students = 8300,
                    Rating = 4.8,
                    Instructor = "Mr. Rajib Hasan",
                    Duration = "8 Weeks",
                    Lessons = 36,
                    Level = "Beginner"
                },
                new CourseDetailViewModel
                {
                    Id = 3,
                    Title = "Complete React Front-end Developer",
                    Description = "Master React.js with real-world projects and build modern web applications.",
                    Image = "https://i.ibb.co.com/rf3RgYqG/Complete-React-Front-end-developer-course.jpg",
                    Category = "Frontend",
                    Price = 2499,
                    Students = 15700,
                    Rating = 4.9,
                    Instructor = "Dr. Sarah Ahmed",
                    Duration = "14 Weeks",
                    Lessons = 62,
                    Level = "Intermediate"
                },
                new CourseDetailViewModel
                {
                    Id = 4,
                    Title = "Complete Web Design",
                    Description = "From beginner to professional web designer — UI/UX design mastery.",
                    Image = "https://i.ibb.co.com/XZSqdW0B/Complete-Web-Design-from-Figma-to-Webflow.jpg",
                    Category = "Design",
                    Price = 1799,
                    Students = 10200,
                    Rating = 4.7,
                    Instructor = "Ms. Nusrat Jahan",
                    Duration = "10 Weeks",
                    Lessons = 40,
                    Level = "Beginner"
                },
                new CourseDetailViewModel
                {
                    Id = 5,
                    Title = "Flutter Development Bootcamp",
                    Description = "Learn Flutter & Dart from scratch — build cross-platform mobile apps.",
                    Image = "https://i.ibb.co.com/qZ60Vqg/Flutter-Development-Bootcamp-with-Dart.jpg",
                    Category = "Mobile Development",
                    Price = 2299,
                    Students = 6900,
                    Rating = 4.8,
                    Instructor = "Mr. Kamal Hossain",
                    Duration = "12 Weeks",
                    Lessons = 55,
                    Level = "Beginner to Intermediate"
                },
                new CourseDetailViewModel
                {
                    Id = 6,
                    Title = "The Ultimate Figma Course",
                    Description = "From zero to expert in UI/UX design — master Figma like a pro.",
                    Image = "https://i.ibb.co.com/rGttSLJy/The-Ultimate-Figma-Course-From-Zero-to-Expert.jpg",
                    Category = "Design",
                    Price = 1599,
                    Students = 9400,
                    Rating = 4.9,
                    Instructor = "Ms. Farhana Akter",
                    Duration = "6 Weeks",
                    Lessons = 28,
                    Level = "Beginner"
                }
            };

            return courses.FirstOrDefault(c => c.Id == id);
        }
    }
}