// 📁 Controllers/FindTutorController.cs
// লোকেশন: Tutorbub/Controllers/FindTutorController.cs

using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorbub.Controllers
{
    public class FindTutorController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public FindTutorController(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        // ===== মেইন পেজ — শুধু Approved Tutors দেখাবে =====
        public IActionResult Index()
        {
            List<TutorProfile> tutors = new List<TutorProfile>();

            try
            {
                tutors = _dbHelper.GetApprovedTutors();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FindTutorController.Index - Error: " + ex.Message);
                tutors = new List<TutorProfile>();
            }

            // Debug info (পরে সরিয়ে ফেলতে পারেন)
            ViewBag.TutorCount = tutors.Count;

            return View("FindTutor", tutors);
        }

        // ===== API: নির্দিষ্ট টিউটর =====
        [HttpGet]
        public IActionResult GetTutor(int id)
        {
            try
            {
                var tutors = _dbHelper.GetApprovedTutors();
                var tutor = tutors.FirstOrDefault(t => t.Id == id);
                if (tutor == null)
                    return Json(new { success = false, message = "Tutor not found" });
                return Json(new { success = true, tutor });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===== API: Tutor ID দিয়ে খোঁজা =====
        [HttpGet]
        public IActionResult GetTutorByTutorId(string tutorId)
        {
            if (string.IsNullOrWhiteSpace(tutorId))
                return Json(new { success = false, message = "Tutor ID is required" });

            try
            {
                var tutors = _dbHelper.GetApprovedTutors();
                var tutor = tutors.FirstOrDefault(t =>
                    t.TutorId.Equals(tutorId, StringComparison.OrdinalIgnoreCase));
                if (tutor == null)
                    return Json(new { success = false, message = "Tutor not found with this ID" });
                return Json(new { success = true, tutorId = tutor.TutorId, fullName = tutor.FullName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}