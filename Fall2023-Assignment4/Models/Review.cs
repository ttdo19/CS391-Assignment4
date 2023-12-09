using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2023_Assignment4.Models
{
	public class Review
	{
		public string Id { get; set; } = null!;

		[ForeignKey("Restaurant")]
		public string RestaurantId { get; set; } = null!;

        public string? Url { get; set; }

        public string Text { get; set; } = null!;

		public string? UserId { get; set; }

		public string? UserName { get; set; }

		public string? TimeCreated { get; set; }

		public double? Rating { get; set; }
	}
}

