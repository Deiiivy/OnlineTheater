using Logic.DAL;
using Logic.Entities;
using Logic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineTheater.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly Model _context;
        private readonly CustomerService _customerService;
        public CustomersController(Model context, CustomerService customerService)
        {
            _context = context;
            _customerService = customerService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Json(customer);
        }

        [HttpGet]
        public JsonResult GetList() =>
            Json(_context.Customers.ToList());

        [HttpPost]
        public IActionResult Create([FromBody] Customer item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (_context.Customers.FirstOrDefault(c => c.Email == item.Email) != null)
                {
                    return BadRequest("Email is already in use: " + item.Email);
                }

                item.Id = 0;
                item.Status = CustomerStatus.Regular;
                _context.Add(item);
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = await _context.Customers.FindAsync(item.Id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                customer.Name = item.Name;
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("{id}/movies")]
        public async Task<IActionResult> PurchaseMovie(int id, [FromBody] int movieId)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null)
                {
                    return BadRequest("Invalid movie id: " + movieId);
                }

                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                var purchasedMovies = _context.PurchasedMovies.Where(pm => pm.Customer.Id == id).ToList();

                if (purchasedMovies.Any(x => x.Movie.Id == movie.Id && (x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow)))
                {
                    return BadRequest("The movie is already purchased: " + movie.Name);
                }

                var purchasedMovie = _customerService.PurchaseMovie(customer, movie);
                customer.MoneySpent += purchasedMovie.Price;
                _context.PurchasedMovies.Attach(purchasedMovie);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpPost]
        [Route("{id}/promotion")]
        public IActionResult PromoteCustomer(int id)
        {
            try
            {
                var customer = _context.Customers.Find(id);
                if (customer == null)
                {
                    return BadRequest("Invalid customer id: " + id);
                }

                if (customer.Status == CustomerStatus.Advanced && (customer.StatusExpirationDate == null || customer.StatusExpirationDate.Value < DateTime.UtcNow))
                {
                    return BadRequest("The customer already has the Advanced status");
                }
                var purchasedMovies = _context.PurchasedMovies.Where(pm => pm.Customer.Id == id).ToList();
                bool success = _customerService.PromoteCustomer(customer, purchasedMovies);
                customer.Status = CustomerStatus.Advanced;
                customer.StatusExpirationDate = DateTime.UtcNow.AddYears(1);
                if (!success)
                {
                    return BadRequest("Cannot promote the customer");
                }

                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }
    }
}
