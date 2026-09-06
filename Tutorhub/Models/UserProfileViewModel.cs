using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tutorbub.Models
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        // ===== বেসিক ইনফো =====
        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Mobile Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string MobileNumber { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;

        // ===== অ্যাডিশনাল ইনফো =====
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Age Range")]
        public string AgeRange { get; set; } = string.Empty;

        [Display(Name = "Primary Device")]
        public string PrimaryDeviceType { get; set; } = string.Empty;

        [Display(Name = "Years of Experience")]
        public string YearsOfExperience { get; set; } = string.Empty;

        [Display(Name = "Area Type")]
        public string AreaType { get; set; } = string.Empty;

        // ===== অ্যাড্রেস =====
        [Display(Name = "Country")]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; } = string.Empty;

        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; } = string.Empty;

        // ===== এডুকেশন =====
        [Display(Name = "Education Level")]
        public string EducationLevel { get; set; } = string.Empty;

        [Display(Name = "Currently Studying")]
        public string CurrentStudyStatus { get; set; } = "No";

        [Display(Name = "Exam/Degree Title")]
        public string ExamDegreeTitle { get; set; } = string.Empty;

        [Display(Name = "Institution Name")]
        public string InstitutionName { get; set; } = string.Empty;

        [Display(Name = "Passing Year")]
        public string PassingYear { get; set; } = string.Empty;

        [Display(Name = "CSE/CS Student")]
        public bool IsCSEStudent { get; set; }

        // ===== ইম্পোর্ট্যান্ট লিংক =====
        [Display(Name = "CV Link")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string CvLink { get; set; } = string.Empty;

        [Display(Name = "Github Profile")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string GithubProfile { get; set; } = string.Empty;

        [Display(Name = "Portfolio Link")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string PortfolioLink { get; set; } = string.Empty;

        [Display(Name = "LinkedIn Profile")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string LinkedInProfile { get; set; } = string.Empty;

        [Display(Name = "Profile Image Link")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string ProfileImageLink { get; set; } = string.Empty;

        // ===== স্কিল সেট =====
        public List<UserSkill> Skills { get; set; } = new List<UserSkill>();

        // ===== কোর্স অর্ডার হিস্ট্রি =====
        public List<CourseOrder> CourseOrders { get; set; } = new List<CourseOrder>();

        // ===== সার্টিফিকেট =====
        public List<Certificate> Certificates { get; set; } = new List<Certificate>();

        // ===== প্রোফাইল সম্পূর্ণ কিনা =====
        public bool IsProfileComplete()
        {
            return !string.IsNullOrEmpty(FullName) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(MobileNumber) &&
                   !string.IsNullOrEmpty(Gender) &&
                   !string.IsNullOrEmpty(AgeRange) &&
                   !string.IsNullOrEmpty(PrimaryDeviceType) &&
                   !string.IsNullOrEmpty(Country) &&
                   !string.IsNullOrEmpty(EducationLevel);
        }
    }
}