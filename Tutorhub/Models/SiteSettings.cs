// 📁 Models/SiteSettings.cs
// লোকেশন: Tutorbub/Models/SiteSettings.cs

using System;
using System.Collections.Generic;

namespace Tutorbub.Models
{
    public class SiteSettings
    {
        public int Id { get; set; }

        // ===== Hero Images (Comma-separated URLs) =====
        public string HeroImageUrls { get; set; } = string.Empty;

        // ===== Slider Settings =====
        public bool SliderEnabled { get; set; } = false;
        public int SliderIntervalSeconds { get; set; } = 5;

        // ===== Theme Colors =====
        public string PrimaryColor { get; set; } = "#1F3B2C";
        public string SecondaryColor { get; set; } = "#F3F1E7";
        public string AccentColor { get; set; } = "#F4C744";
        public string TextColor { get; set; } = "#4B5648";
        public string NavbarBgColor { get; set; } = "#F3F1E7";
        public string FooterBgColor { get; set; } = "#F3F1E7";

        // ===== Meta =====
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ===== Helper =====
        public List<string> GetHeroImageList()
        {
            if (string.IsNullOrWhiteSpace(HeroImageUrls))
                return new List<string>();

            return HeroImageUrls
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }
    }
}