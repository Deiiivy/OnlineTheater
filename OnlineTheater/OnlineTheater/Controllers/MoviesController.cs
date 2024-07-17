using Microsoft.AspNetCore.Mvc;
using Logic.DAL;
using Logic.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Logic.Services;
using OnlineTheater.Models;

namespace OnlineTheater.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly Model _context;
        private readonly CustomerService _customerService;
        private readonly MovieService _movieService;


        public MoviesController(Model context, CustomerService customerService, MovieService movieService)
        {
            _context = context;
            _customerService = customerService;
            _movieService = movieService;
        }

        [HttpGet("/AllActiveMovies")]
        public async Task<IActionResult> getMoviesActive()
        {
            try
            {
                var movies = await _context.Movies
        .Where(m => m.IsActive)
        .Select(m => new MovieDTO
        {
            Name = m.Name,
            Description = m.Description,
            Rating = m.Audience.ToString(),
            Category = m.Category.ToString(),
            IsActive = m.IsActive,
            LicensingModel = m.LicensingModel.ToString()
        })
        .ToListAsync();

                if(movies.Count == 0)
                {
                    return NotFound("no hay peliculas activas");
                }
                

                return Ok(movies);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "error" + ex);
            }
        }

        [HttpGet("/GetAllMovies")]
        public async Task<IActionResult> GetAllMovies()
        {
            try
            {
                var movies = await _context.Movies
                    .Select(m => new MovieDTO
                    {
                        Name = m.Name,
                        Description = m.Description,
                        Rating = m.Audience.ToString(),
                        Category = m.Category.ToString(),
                        IsActive = m.IsActive,
                        LicensingModel = m.LicensingModel.ToString(),
                        AverageRating = _movieService.AverageRatingMovie(m.Id)
                    })
                    .ToListAsync();

                if (movies == null || !movies.Any())
                {
                    return NotFound("no hay peliculas");
                }

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error" + ex.Message);
            }
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound("movie not found");
            }

            var averageRating = _movieService.AverageRatingMovie(id);
            var movieDto = new MovieDTO
            {
                Name = movie.Name,
                Description = movie.Description,
                Rating = movie.Audience.ToString(),
                Category = movie.Category.ToString(),
                IsActive = movie.IsActive,
                LicensingModel = movie.LicensingModel.ToString(),
                AverageRating = averageRating
            };

            return Ok(movieDto);
        }




        [HttpGet("{customerId}/All")]

        public async Task<IActionResult> GetAllMoviesForPriceTheClient(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if(customer == null)
            {
                return BadRequest("customer not found");
            }

            var movies = await _context.Movies
     .Select(m => new MovieDTO
     {
         Name = m.Name,
         Description = m.Description,
         Rating = m.Audience.ToString(),
         Category = m.Category.ToString(),
         Price = _customerService.CalculatePrice(customer.Status, customer.StatusExpirationDate, m.LicensingModel),
         IsActive = m.IsActive,
         LicensingModel = m.LicensingModel.ToString(),
         AverageRating = _movieService.AverageRatingMovie(m.Id)
     })
     .ToListAsync();

            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovieDTO movieDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var movie = new Movie
                {
                    Name = movieDto.Name,
                    Description = movieDto.Description,
                    IsActive = movieDto.IsActive,
                    LicensingModel = Enum.TryParse<LicensingModel>(movieDto.LicensingModel, out var licensingModelEnum) ? licensingModelEnum : Enum.Parse<LicensingModel>(movieDto.LicensingModel),
                    Category = Enum.TryParse<Category>(movieDto.Category, out var categoryEnum) ? categoryEnum : Enum.Parse<Category>(movieDto.Category),
                    Audience = Enum.TryParse<Audience>(movieDto.Rating, out var audienceEnum) ? audienceEnum : Enum.Parse<Audience>(movieDto.Rating)
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear película: " + ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Movie Movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();   
            }

            movie.Name = Movie.Name;
            movie.Description = Movie.Description;
            movie.Audience = Movie.Audience;
            movie.Category = Movie.Category;
            movie.IsActive = Movie.IsActive; 
            movie.LicensingModel = Movie.LicensingModel;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if(movie == null)
            {
                return NotFound($"pelicula con id: {id} no encontrada");
            }

            if (movie.IsActive)
            {
                return BadRequest($"pelicula con id {id} ya esta activada");
            }

            movie.IsActive = true;
            await _context.SaveChangesAsync();

            var averageRating = _movieService.AverageRatingMovie(id);

            var movieDto = new MovieDTO
            {
                Name = movie.Name,
                Description = movie.Description,
                Rating = movie.Audience.ToString(),
                Category = movie.Category.ToString(),
                IsActive = movie.IsActive,
                LicensingModel = movie.LicensingModel.ToString(),
                AverageRating = averageRating
            };

            return Ok(movie);

        }


        [HttpPatch("{id}/inactivate")]
        public async Task<IActionResult> Inactivate(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            movie.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"pelicula con id {id} inactivada" });
        }

        [HttpPost("{id}/RatingMovie")]
        public IActionResult RateMovie(int id, int rating, int customerId)
        {
            try
            {
                var movie = _context.Movies.Find(id);
                var customer = _context.Customers.Find(customerId);

                if (movie == null)
                    return NotFound("pelicula no encontrada");

                if (customer == null)
                    return NotFound("cliente no encontrada");

                if (rating < 1 || rating > 5)
                    return BadRequest("calificacion fuera del rango (1-5)");

                var ratingMovie = new RatingMovie
                {
                    Customer = customer,
                    Movie = movie,
                    Rating = rating
                };

                _context.RatingMovies.Add(ratingMovie);
                _context.SaveChanges();

             
                return Ok("pelicula calificada exitoxamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"error: {ex.Message}");
            }
        }


    }
}
