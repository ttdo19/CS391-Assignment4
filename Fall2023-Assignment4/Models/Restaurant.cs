using System;
using System.ComponentModel.DataAnnotations;
using Fall2023_Assignment4.Data;
namespace Fall2023_Assignment4.Models
{
    public class Restaurant
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public double? Rating { get; set; }

        public string? Price { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Category Title")]
        public string? CategoryTitle { get; set; }

        [Display(Name = "Reviews")]
        public int? ReviewCount { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? State { get; set; }

        public string? Address1 { get; set; }

        public string? ZipCode { get; set; }

        public string? Address2 { get; set; }

        public string? Address3 { get; set; }

        [Display(Name = "Open Now?")]
        public bool? IsOpenNow { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public List<ApplicationUser>? FavoriteUsers { get; set; } = new();

        public int FavoriteCount { get; set; } = 0;
    }
}

