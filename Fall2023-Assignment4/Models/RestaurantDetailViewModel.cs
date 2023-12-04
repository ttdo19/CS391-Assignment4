using System;
namespace Fall2023_Assignment4.Models
{
	public class RestaurantDetailViewModel
	{
		public Restaurant Restaurant { get; set; } = null!;

		public List<Review> Reviews { get; set; } = new List<Review>();

	}
}

