using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Tutorbub.Controllers
{
    public class PracticeController : Controller
    {
        // ===== ১০টি প্রশ্ন =====
        private static readonly List<QuizQuestion> Questions = new()
        {
            new QuizQuestion
            {
                Id = 1,
                Question = "বাংলাদেশের রাজধানী কোনটি?",
                Options = new List<string> { "ঢাকা", "চট্টগ্রাম", "রাজশাহী", "খুলনা" },
                CorrectAnswerIndex = 0,
                Level = "Easy"
            },
            new QuizQuestion
            {
                Id = 2,
                Question = "লাল গ্রহ নামে পরিচিত কোনটি?",
                Options = new List<string> { "শুক্র", "মঙ্গল", "বৃহস্পতি", "শনি" },
                CorrectAnswerIndex = 1,
                Level = "Easy"
            },
            new QuizQuestion
            {
                Id = 3,
                Question = "পানির রাসায়নিক সংকেত কি?",
                Options = new List<string> { "H2O", "CO2", "NaCl", "HCl" },
                CorrectAnswerIndex = 0,
                Level = "Medium"
            },
            new QuizQuestion
            {
                Id = 4,
                Question = "'The Great Gatsby' বইটির লেখক কে?",
                Options = new List<string> { "আর্নেস্ট হেমিংওয়ে", "এফ. স্কট ফিটজেরাল্ড", "মার্ক টোয়েন", "জেন অস্টেন" },
                CorrectAnswerIndex = 1,
                Level = "Medium"
            },
            new QuizQuestion
            {
                Id = 5,
                Question = "সবচেয়ে দ্রুতগতির স্থলজ প্রাণী কোনটি?",
                Options = new List<string> { "সিংহ", "চিতা", "চিতাবাঘ", "বাঘ" },
                CorrectAnswerIndex = 1,
                Level = "Easy"
            },
            new QuizQuestion
            {
                Id = 6,
                Question = "আর্টিফিশিয়াল ইন্টেলিজেন্সের জন্য কোন প্রোগ্রামিং ভাষা বেশি ব্যবহৃত হয়?",
                Options = new List<string> { "Python", "Java", "C++", "Ruby" },
                CorrectAnswerIndex = 0,
                Level = "Hard"
            },
            new QuizQuestion
            {
                Id = 7,
                Question = "পৃথিবীর বৃহত্তম মহাসাগর কোনটি?",
                Options = new List<string> { "আটলান্টিক", "ভারতীয়", "প্যাসিফিক", "আর্কটিক" },
                CorrectAnswerIndex = 2,
                Level = "Easy"
            },
            new QuizQuestion
            {
                Id = 8,
                Question = "১৪৪ এর বর্গমূল কত?",
                Options = new List<string> { "১০", "১১", "১২", "১৩" },
                CorrectAnswerIndex = 2,
                Level = "Medium"
            },
            new QuizQuestion
            {
                Id = 9,
                Question = "বিশ্বের সবচেয়ে বেশি জনসংখ্যার দেশ কোনটি?",
                Options = new List<string> { "চীন", "ভারত", "মার্কিন যুক্তরাষ্ট্র", "ইন্দোনেশিয়া" },
                CorrectAnswerIndex = 1,
                Level = "Medium"
            },
            new QuizQuestion
            {
                Id = 10,
                Question = "সবচেয়ে শক্ত প্রাকৃতিক পদার্থ কোনটি?",
                Options = new List<string> { "সোনা", "লোহা", "হীরক", "প্লাটিনাম" },
                CorrectAnswerIndex = 2,
                Level = "Hard"
            }
        };

        // ===== Practice পেইজ =====
        public IActionResult Index()
        {
            return View("Practice", Questions);
        }

        // ===== API: প্রশ্ন পাওয়া =====
        [HttpGet]
        public IActionResult GetQuestions()
        {
            return Json(Questions);
        }
    }

    // ===== কুইজ প্রশ্ন মডেল =====
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Level { get; set; } = "Medium";
    }
}