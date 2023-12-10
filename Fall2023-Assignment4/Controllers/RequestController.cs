using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using VaderSharp2;
using Fall2023_Assignment4.Data;
using Fall2023_Assignment4.Models;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;
using Azure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Newtonsoft.Json;
using System.Windows.Forms.DataVisualization.Charting;

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

            var prices = RestaurantList.Select(r => r.Price).ToList();

            var count = new Dictionary<string, int>
            {
                { "$", prices.Count(p => p == "$") },
                { "$$", prices.Count(p => p == "$$") },
                { "$$$", prices.Count(p => p == "$$$") }
            };
            
            await _context.SaveChangesAsync();

            var vm = new RequestDetailViewModel()
            {
                Restaurants = RestaurantList,
                PriceData = JsonConvert.SerializeObject(count)
            };

            return View(vm);
        }


        [Authorize(Roles = Const.Roles.User)]
        public async Task<IActionResult> AddReview(string id)
        {
            if (id == null || _context.Restaurant == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Const.Roles.User}")]
        public async Task<IActionResult> AddReview(string id, string text, int rating)
        {
            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            string Id = Guid.NewGuid().ToString();
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss");

            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            string userName = User.Identity?.Name;

            var review = new Review();
            review.Id = Id;
            review.RestaurantId = id;
            review.Rating = rating;
            review.Text = text;
            review.TimeCreated = formattedTime;
            review.Url = null;
            review.UserId = userId;
            review.UserName = userName;

            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
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

           
            var reviewTexts = await _context.Review
                .Where(cs => cs.RestaurantId == id)
                .Select(review => review.Text)
                .ToListAsync();
            var formattedReviews = reviewTexts
                .Select((text, index) => $"{index + 1}. \"{text}\"")
                .ToList();

            string.Join("\n", formattedReviews);

            var GptApiKey = _config["Restaurant:GptApiKey"];
            var client = new OpenAIClient(GptApiKey);

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, $"You majored in Creative Writing and you are a restaurant critic. You will be given a list of reviews, and you will generate a phrase or a few words about the vibe or atmosphere"),
                    new ChatMessage(ChatRole.User, $"generate a phrase or a few words about the vibe or atmosphere for restaurant {restaurant.Name} given the following reviews: {formattedReviews}."),
                },
                ChoiceCount = 1,
            };

            var request = await client.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionsOptions);
            var response = request.Value.Choices[0].Message.Content;

            var vm = new RestaurantDetailViewModel()
            {
                Restaurant = restaurant,
                Vibe = response,
                Reviews = await _context.Review
                    .Where(cs => cs.RestaurantId == id)
                    .ToListAsync()
            };   

            return View(vm);

        }
    }
}