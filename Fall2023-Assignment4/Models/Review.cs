using System;
using System.ComponentModel.DataAnnotations;

namespace Fall2023_Assignment4.Models
{
	public class Review
	{
        public string Url { get; set; } = null!;

        public string? Text { get; set; } = null!;

		public User User { get; set; } = null!;

		public string? TimeCreated { get; set; }

		public int? Rating { get; set; }
	}
}

