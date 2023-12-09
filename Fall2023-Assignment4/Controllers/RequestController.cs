using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fall2023_Assignment4.Data;
using Fall2023_Assignment4.Models;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Fall2023_Assignment4.Controllers
{
    //[Authorize(Roles = Const.Role.Admin + "," + Const.Role.Manager)]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _config;

        public RequestController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config; 
        }

        [HttpPost]
        public async Task<IActionResult> RequestAsync (Request r)
        {
            var googleMapApiKey = _config["Restaurant:GoogleMapApiKey"];
            var locationService = new GoogleLocationService(apikey: googleMapApiKey);

            var point = locationService.GetLatLongFromAddress(r.Location);

            var yelpApiKey = _config["Restaurant:YelpApiKey"];
            var client = new Yelp.Api.Client(yelpApiKey);

            var results = await client.SearchBusinessesAllAsync(r.Category, point.Latitude, point.Longitude);

            var businesses = results.Businesses;

            List<Restaurant> RestaurantList = new List<Restaurant>();

            foreach (var business in businesses)
            {
                var restaurant = new Restaurant();
                restaurant.Id = business.Id;
                
                restaurant.ImageUrl = business.ImageUrl;
                restaurant.IsOpenNow = !business.IsClosed;
                restaurant.Name = business.Name;
                restaurant.PhoneNumber = business.DisplayPhone;
                restaurant.Price = business.Price;
                restaurant.Rating = business.Rating;
                restaurant.ReviewCount = business.ReviewCount;
                restaurant.Address1 = business.Location.Address1;
                restaurant.Address2 = business.Location.Address2;
                restaurant.Address3 = business.Location.Address3;
                restaurant.CategoryTitle = business.Categories[0].Title;
                restaurant.City = business.Location.City;
                restaurant.Country = business.Location.Country;
                restaurant.Latitude = business.Coordinates.Latitude;
                restaurant.Longitude = business.Coordinates.Longitude;
                restaurant.State = business.Location.State;
                restaurant.ZipCode = business.Location.ZipCode;

                RestaurantList.Add(restaurant);

                var restaurantInDB = await _context.Restaurant.FindAsync(restaurant.Id);
                if (restaurantInDB != null)
                {
                    continue;
                }
                _context.Restaurant.Add(restaurant);

                var reviews = await client.GetReviewsAsync(business.Id);

                List<Review> reviewsList = new List<Review>();

                foreach (var reviewResult in reviews.Reviews)
                {
                    string Id = Guid.NewGuid().ToString();
                    var review = new Review();
                    review.Id = Id;
                    review.RestaurantId = restaurant.Id;
                    review.Rating = reviewResult.Rating;
                    review.Text = reviewResult.Text;
                    review.TimeCreated = reviewResult.TimeCreated;
                    review.Url = reviewResult.Url;
                    review.UserId = null;
                    review.UserName = reviewResult.User.Name;
                    reviewsList.Add(review);
                    _context.Review.Add(review);
                }

            }

            await _context.SaveChangesAsync();

            return View(RestaurantList); 
        }

        // GET: Restaurant/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Restaurant == null)
            {
                return NotFound();
            }
            var restaurant = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var vm = new RestaurantDetailViewModel()
            {
                Restaurant = restaurant,
                Reviews = await _context.Review
                    .Where(cs => cs.RestaurantId == id)
                    .ToListAsync()
            };   

            return View(vm);

        }
    }
}