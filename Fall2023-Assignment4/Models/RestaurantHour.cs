using System;
using System.ComponentModel.DataAnnotations;

namespace Fall2023_Assignment4.Models
{
	public class RestaurantHour: Yelp.Api.Models.Hour
	{
        [Key]
        public int Day { get; set; }

        public bool IsOverNight { get; set; }

		public string Start { get; set; } = null!;

        public string End { get; set; } = null!;

    }
}

