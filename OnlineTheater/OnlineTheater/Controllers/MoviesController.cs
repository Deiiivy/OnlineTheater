using Microsoft.AspNetCore.Mvc;
using Logic.DAL;
using Logic.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Logic.Services;

namespace OnlineTheater.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly Model _context;



        public MoviesController(Model context)
        {
            _context = context;
        }

     

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null || !movie.IsActive)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _context.Movies
                .Where(m => m.IsActive)
                .ToListAsync();

            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Movie movie)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }



                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error al crear película: " + ex.Message);
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
            movie.Rating = Movie.Rating;
            movie.Category = Movie.Category;
            movie.IsActive = Movie.IsActive;
            movie.LicensingModel = Movie.LicensingModel;

            await _context.SaveChangesAsync();

            return NoContent();
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
    }
}

