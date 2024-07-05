using Microsoft.AspNetCore.Mvc;
using Logic.DAL;
using Logic.Entities;
using Logic.Services;

namespace OnlineTheater.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {

        private readonly Model _context;
        private readonly MovieService _movieService;

        public MoviesController(Model context, MovieService movieService)
        {
            _context = context;
            _movieService = movieService;
        }


        [HttpGet]
        [Route("{id}")]
        public async Task <IActionResult> Get(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Json(movie);
        }

        [HttpGet]

        public async Task <IActionResult> GetAll()
        {
            var movies = _context.Movies.ToList();

            return Json(movies);
        }

        [HttpPost]

        public async Task <IActionResult> Create(Movie movie)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if(string.IsNullOrEmpty(movie.Name) || movie.LicensingModel == null)
                {
                    return StatusCode(400, "los campos no pueden estar vacios");   
                }

                movie.Id = 0;
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                return Ok(movie);

            } catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPut]
        [Route("{id}")]

        public async Task <IActionResult> Update(Movie Movie, int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movie =  await _context.Movies.FindAsync(id);
            if(movie == null)
            {
              
                return NotFound();
            }

            movie.Name = Movie.Name;
            movie.LicensingModel = Movie.LicensingModel;
            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
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

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok($"id de pelicula eliminada: {id}"); 
        }
    }
}
