// 📁 Controllers/ProfileController.cs
using Microsoft.AspNetCore.Mvc;
using Tutorbub.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Tutorbub.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _dbHelper = new DatabaseHelper(configuration);
            _webHostEnvironment = webHostEnvironment;
        }

        // ===== প্রোফাইল দেখানো =====
        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(int.Parse(userId));

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Role = user.Role,
                MobileNumber = user.MobileNumber ?? string.Empty,
                Gender = user.Gender ?? string.Empty,
                AgeRange = user.AgeRange ?? string.Empty,
                PrimaryDeviceType = user.PrimaryDeviceType ?? string.Empty,
                YearsOfExperience = user.YearsOfExperience ?? string.Empty,
                AreaType = user.AreaType ?? string.Empty,
                Country = user.Country ?? string.Empty,
                StreetAddress = user.StreetAddress ?? string.Empty,
                PermanentAddress = user.PermanentAddress ?? string.Empty,
                EducationLevel = user.EducationLevel ?? string.Empty,
                CurrentStudyStatus = user.CurrentStudyStatus ?? "No",
                ExamDegreeTitle = user.ExamDegreeTitle ?? string.Empty,
                InstitutionName = user.InstitutionName ?? string.Empty,
                PassingYear = user.PassingYear ?? string.Empty,
                IsCSEStudent = user.IsCSEStudent ?? false,
                CvLink = user.CvLink ?? string.Empty,
                GithubProfile = user.GithubProfile ?? string.Empty,
                PortfolioLink = user.PortfolioLink ?? string.Empty,
                LinkedInProfile = user.LinkedInProfile ?? string.Empty,
                ProfileImageLink = user.ProfileImageLink ?? string.Empty,
                Skills = new System.Collections.Generic.List<UserSkill>(),
                CourseOrders = new System.Collections.Generic.List<CourseOrder>(),
                Certificates = new System.Collections.Generic.List<Certificate>()
            };

            ViewBag.IsProfileComplete = profile.IsProfileComplete();
            return View(profile);
        }

        // ===== Basic Information Update =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateBasic(UserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // এই ফর্মে শুধু FullName, Email, MobileNumber পোস্ট হয় — বাকি সব
            // UserProfileViewModel প্রপার্টি (Gender, Country, EducationLevel ইত্যাদি)
            // এই request-এ থাকেই না। আগে পুরো মডেলের ModelState.IsValid চেক করা হতো,
            // যেটার ফলে ওই "অনুপস্থিত" ফিল্ডগুলোর কারণে validation সবসময় fail করত
            // এবং Mobile Number ঠিকভাবে দিলেও নীরবে সেভ না হয়ে ফিরে যেত।
            // তাই এখন শুধু এই সেকশনের প্রাসঙ্গিক ফিল্ডগুলো ম্যানুয়ালি ভ্যালিডেট করা হচ্ছে।
            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                TempData["Error"] = "Full Name is required.";
                return RedirectToAction("Index", new { section = "basic" });
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                TempData["Error"] = "Email is required.";
                return RedirectToAction("Index", new { section = "basic" });
            }

            var user = _dbHelper.GetUserById(model.Id);
            if (user != null)
            {
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.MobileNumber = model.MobileNumber;

                if (_dbHelper.UpdateUserProfile(user))
                {
                    HttpContext.Session.SetString("UserFullName", user.FullName);
                    TempData["Success"] = "Basic information updated successfully!";
                    return RedirectToAction("Index", new { section = "basic" });
                }

                TempData["Error"] = "Failed to update profile.";
            }
            else
            {
                TempData["Error"] = "User not found.";
            }

            return RedirectToAction("Index", new { section = "basic" });
        }

        // ===== Additional Information Update =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAdditional(UserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(model.Id);
            if (user != null)
            {
                user.Gender = model.Gender;
                user.AgeRange = model.AgeRange;
                user.PrimaryDeviceType = model.PrimaryDeviceType;
                user.YearsOfExperience = model.YearsOfExperience;
                user.AreaType = model.AreaType;

                if (_dbHelper.UpdateUserProfile(user))
                {
                    TempData["Success"] = "Additional information updated successfully!";
                    return RedirectToAction("Index", new { section = "additional" });
                }
            }
            ViewBag.Error = "Failed to update additional information.";
            return RedirectToAction("Index", new { section = "additional" });
        }

        // ===== Address Update =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(UserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(model.Id);
            if (user != null)
            {
                user.Country = model.Country;
                user.StreetAddress = model.StreetAddress;
                user.PermanentAddress = model.PermanentAddress;

                if (_dbHelper.UpdateUserProfile(user))
                {
                    TempData["Success"] = "Address updated successfully!";
                    return RedirectToAction("Index", new { section = "address" });
                }
            }
            ViewBag.Error = "Failed to update address.";
            return RedirectToAction("Index", new { section = "address" });
        }

        // ===== Education Update =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEducation(UserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(model.Id);
            if (user != null)
            {
                user.EducationLevel = model.EducationLevel;
                user.CurrentStudyStatus = model.CurrentStudyStatus;
                user.ExamDegreeTitle = model.ExamDegreeTitle;
                user.InstitutionName = model.InstitutionName;
                user.PassingYear = model.PassingYear;
                user.IsCSEStudent = model.IsCSEStudent;

                if (_dbHelper.UpdateUserProfile(user))
                {
                    TempData["Success"] = "Education information updated successfully!";
                    return RedirectToAction("Index", new { section = "education" });
                }
            }
            ViewBag.Error = "Failed to update education information.";
            return RedirectToAction("Index", new { section = "education" });
        }

        // ===== Important Links Update =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLinks(UserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _dbHelper.GetUserById(model.Id);
            if (user != null)
            {
                user.CvLink = model.CvLink;
                user.GithubProfile = model.GithubProfile;
                user.PortfolioLink = model.PortfolioLink;
                user.LinkedInProfile = model.LinkedInProfile;

                if (_dbHelper.UpdateUserProfile(user))
                {
                    TempData["Success"] = "Important links updated successfully!";
                    return RedirectToAction("Index", new { section = "links" });
                }
            }
            ViewBag.Error = "Failed to update important links.";
            return RedirectToAction("Index", new { section = "links" });
        }

        // ===== Profile Image Upload =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (profileImage == null || profileImage.Length == 0)
            {
                return Json(new { success = false, message = "Please select an image" });
            }

            try
            {
                var fileName = $"{userId}_{DateTime.Now.Ticks}{Path.GetExtension(profileImage.FileName)}";
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                var user = _dbHelper.GetUserById(int.Parse(userId));
                if (user != null)
                {
                    user.ProfileImageLink = $"/uploads/profiles/{fileName}";

                    if (_dbHelper.UpdateUserProfile(user))
                    {
                        return Json(new { success = true, message = "Profile image uploaded successfully!", imageUrl = user.ProfileImageLink });
                    }
                }

                return Json(new { success = false, message = "Failed to update profile image" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // ===== Get Profile Data =====
        [HttpGet]
        public IActionResult GetProfileData()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not found" });
            }

            var user = _dbHelper.GetUserById(int.Parse(userId));
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            return Json(new
            {
                success = true,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.FullName,
                    user.Email,
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
                    user.Role
                }
            });
        }

        // ===== Get Teacher Request Status =====
        [HttpGet]
        public IActionResult GetTeacherRequestStatus()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return Json(new { success = false, isTeacher = false, status = "None" });
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
            if (userId == 0)
            {
                return Json(new { success = false, isTeacher = false, status = "None" });
            }

            var user = _dbHelper.GetUserById(userId);
            if (user == null)
            {
                return Json(new { success = false, isTeacher = false, status = "None" });
            }

            if (user.Role == "Teacher")
            {
                return Json(new { success = true, isTeacher = true, status = "Approved" });
            }

            var requestStatus = _dbHelper.GetUserTeacherRequestStatus(userId);
            return Json(new { success = true, isTeacher = false, status = requestStatus });
        }
    }
}