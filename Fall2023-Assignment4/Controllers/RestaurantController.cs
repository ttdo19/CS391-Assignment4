using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2023_Assignment4.Data;
using Fall2023_Assignment4.Models;
using Azure.AI.OpenAI;
using static Fall2023_Assignment4.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Fall2023_Assignment4.Core.Repositories;

namespace Fall2023_Assignment4.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IConfiguration _config;

        public RestaurantController(ApplicationDbContext context, IConfiguration config, SignInManager<ApplicationUser> signInManager, IUnitOfWork unitOfWork)
        {
            _context = context;
            _config = config;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        // GET: Restaurant
        public async Task<IActionResult> Index()
        {

            var yelpApiKey = _config["Restaurant:YelpApiKey"];
            var client = new Yelp.Api.Client(yelpApiKey);

            var results = await client.SearchBusinessesAllAsync("Restaurant", 37.786882, -122.399972);

            var businesses = results.Businesses;

            foreach (var business in businesses)
            {
                var restaurant = new Restaurant();
                restaurant.Id = business.Id;
                var restaurantInDB = await _context.Restaurant.FindAsync(restaurant.Id);
                if (restaurantInDB != null)
                {
                    break;
                }
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
                //restaurant.Hours = business.Hours; 
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


            return _context.Restaurant != null ?
                          View(await _context.Restaurant.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Restaurant'  is null.");
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

        // GET: Restaurant/Create
        [Authorize(Roles = Const.Roles.Administrator)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = Const.Roles.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Rating,Price,PhoneNumber,CategoryTitle,ReviewCount,ImageUrl,City,Country,State,Address1,ZipCode,Address2,Address3,IsOpenNow,Longitude,Latitude")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        [Authorize(Roles = Const.Roles.Administrator)]
        // GET: Restaurant/Edit/5
        public async Task<IActionResult> Edit(string id)
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

        // POST: Restaurant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = Const.Roles.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Rating,Price,PhoneNumber,CategoryTitle,ReviewCount,ImageUrl,City,Country,State,Address1,ZipCode,Address2,Address3,IsOpenNow,Longitude,Latitude")] Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        [Authorize(Roles = Const.Roles.Administrator)]
        // GET: Restaurant/Delete/5
        public async Task<IActionResult> Delete(string id)
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

            return View(restaurant);
        }

        [Authorize(Roles = Const.Roles.Administrator)]
        // POST: Restaurant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Restaurant == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Restaurant'  is null.");
            }
            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurant.Remove(restaurant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(string id)
        {
            return (_context.Restaurant?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        //Favorite Restaurant
        [Authorize(Roles = Const.Roles.User)]
        [HttpPost, ActionName("Favorite")]
        public async Task<IActionResult> Favorite(string? id)
        {
            //Find user
            ApplicationUser user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

            var favoriteRestaurantIdInDb = await _context.ApplicationUserRestaurants
               .Where(u => u.ApplicationUserId == user.Id)
               .Select(r => r.RestaurantId!)
               .ToListAsync();

            var favoriteRestaurant = new List<Restaurant>();

            foreach (var restaurantId in favoriteRestaurantIdInDb)
            {
                var restaurantItem = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.Id == restaurantId);
                if (restaurantItem == null)
                {
                    continue;
                }
                else
                {
                    favoriteRestaurant.Add(restaurantItem);
                }
            }

            if (id == null) return View(favoriteRestaurant);

            var restaurant = await _context.Restaurant
              .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }


            if (id == null || _context.Restaurant == null || user == null)
            {
                return NotFound();
            }
            var favoriteUsersIdInDb = await _context.ApplicationUserRestaurants
                .Where(r => r.RestaurantId == id)
                .Select(u => u.ApplicationUserId!)
                .ToListAsync();



            restaurant.FavoriteUsers = new List<ApplicationUser>();

            var restaurantIsFavorite = 0;

            //If this restaurant has never been favorite by any user, then just add the users to the Favorite User list
            if (favoriteUsersIdInDb == null || favoriteUsersIdInDb.Count == 0)
            {
                restaurant.FavoriteCount++;
                restaurant.FavoriteUsers.Add(user);
                favoriteRestaurant.Add(restaurant);
            }
            else
            {
                foreach (var userId in favoriteUsersIdInDb)
                {
                    //if the user already favorites the restaurant, do nothing
                    if (userId == user.Id)
                    {
                        restaurantIsFavorite = 1;
                    }
                }
                if (restaurantIsFavorite == 1) return View(favoriteRestaurant);
                else
                {
                    restaurant.FavoriteCount++;
                    restaurant.FavoriteUsers.Add(user);
                    favoriteRestaurant.Add(restaurant);
                }
            }
            _context.Update(restaurant);
            await _context.SaveChangesAsync();
            return View(favoriteRestaurant);
        }

        [Authorize(Roles = Const.Roles.User)]
        [HttpGet]
        public async Task<IActionResult> SeeFavorite()
        {
            //Find user
            ApplicationUser user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

            var favoriteRestaurantIdInDb = await _context.ApplicationUserRestaurants
               .Where(u => u.ApplicationUserId == user.Id)
               .Select(r => r.RestaurantId!)
               .ToListAsync();

            var favoriteRestaurant = new List<Restaurant>();

            foreach (var restaurantId in favoriteRestaurantIdInDb)
            {
                var restaurantItem = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.Id == restaurantId);
                if (restaurantItem == null)
                {
                    continue;
                }
                else
                {
                    favoriteRestaurant.Add(restaurantItem);
                }
            }

            return View(favoriteRestaurant);

        }

        //Unfavorite a restaurant
        [Authorize(Roles = Const.Roles.User)]
        [HttpPost]
        public async Task<IActionResult> Unfavorite(string id)
        {
            //Find user
            ApplicationUser user = await _signInManager.UserManager.GetUserAsync(HttpContext.User);

            //Get the list of restaurants favored by this user
            var favoriteRestaurantIdInDb = await _context.ApplicationUserRestaurants
               .Where(u => u.ApplicationUserId == user.Id)
               .Select(r => r.RestaurantId!)
               .ToListAsync();

            var favoriteRestaurant = new List<Restaurant>();

            foreach (var restaurantId in favoriteRestaurantIdInDb)
            {
                var restaurantItem = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.Id == restaurantId);
                if (restaurantItem == null || restaurantItem.Id == id)
                {
                    continue;
                }
                else
                {
                    favoriteRestaurant.Add(restaurantItem);
                }
            }

            var restaurant = await _context.Restaurant
             .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }


            if (id == null || _context.Restaurant == null || user == null)
            {
                return NotFound();
            }
            //Get the users who like this restaurant
            _context.Remove(_context.ApplicationUserRestaurants.Single(a => a.RestaurantId == id && a.ApplicationUserId == user.Id));
            restaurant.FavoriteCount--;
            _context.Update(restaurant);

            await _context.SaveChangesAsync();
            return View(favoriteRestaurant);

        }

    }
}
