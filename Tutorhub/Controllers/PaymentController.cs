// 📁 Controllers/PaymentController.cs
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

            var course = _dbHelper.GetCourseById(courseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index", "Learn");
            }

            // ✅ Enrollment চেক
            if (!course.IsEnrollmentOpen)
            {
                TempData["Error"] = "Enrollment for this course is currently closed.";
                return RedirectToAction("Details", "Learn", new { id = courseId });
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

            var course = _dbHelper.GetCourseById(model.CourseId);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index", "Learn");
            }

            // ✅ Enrollment চেক
            if (!course.IsEnrollmentOpen)
            {
                TempData["Error"] = "Enrollment for this course has been closed by the admin.";
                return RedirectToAction("Details", "Learn", new { id = model.CourseId });
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
    }
}