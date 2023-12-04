using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2023_Assignment4.Data;
using Fall2023_Assignment4.Models;

namespace Fall2023_Assignment4.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Restaurant
        public async Task<IActionResult> Index()
        {
            var client = new Yelp.Api.Client("g1wbHjGxRh5TeoM1S0AugiKUj86UT4OQH0Xm1i1sifnf0gY1rDyBZHbCPzBOTgmKERqddUTrjYnAJ18a62SyDYAmYuLbaIvGplsv9urg6uLezb9gQrzUXA2g9ugcY3Yx");
           
            var results = await client.SearchBusinessesAllAsync("Cupcakes", 37.786882, -122.399972);

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

            var client = new Yelp.Api.Client("g1wbHjGxRh5TeoM1S0AugiKUj86UT4OQH0Xm1i1sifnf0gY1rDyBZHbCPzBOTgmKERqddUTrjYnAJ18a62SyDYAmYuLbaIvGplsv9urg6uLezb9gQrzUXA2g9ugcY3Yx");

            var reviews = await client.GetReviewsAsync(id);

            List<Review> reviewsList = new List<Review>();

            foreach (var reviewResult in reviews.Reviews)
            {
                var review = new Review();
                review.Url = reviewResult.Url;
                review.Rating = reviewResult.Rating;
                review.Text = reviewResult.Text;
                review.TimeCreated = reviewResult.TimeCreated;
                review.User = new User();
                review.User.Name = reviewResult.User.Name;
                review.User.ImageUrl = reviewResult.User.ImageUrl;
                reviewsList.Add(review);
            }

            var vm = new RestaurantDetailViewModel()
            {
                Restaurant = restaurant,
                Reviews = reviewsList,

            };
            return View(vm);
        }

        // GET: Restaurant/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurant/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // POST: Restaurant/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
    }
}