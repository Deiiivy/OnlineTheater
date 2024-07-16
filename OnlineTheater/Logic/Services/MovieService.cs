using Logic.DAL;
using Logic.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class MovieService
    {

        private readonly Model _context;

        public MovieService(Model context)
        {
            _context = context;
        }


        public DateTime? GetExpirationDate(LicensingModel licensingModel)
        {
            DateTime? result;

            switch (licensingModel)
            {
                case LicensingModel.TwoDays:
                    result = DateTime.UtcNow.AddDays(2);
                    break;

                case LicensingModel.LifeLong:
                    result = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

      

        public RatingMovie RatingMovie(Customer customer, Movie movie, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "calificacion fuera del rango de 1 a 5");

            var existingRating = _context.RatingMovies
                .FirstOrDefault(rm => rm.Movie.Id == movie.Id && rm.Customer.Id == customer.Id);

            if (existingRating != null)
                throw new InvalidOperationException("el cliente ya califico esta movie");

            var ratingMovie = new RatingMovie
            {
                Customer = customer,
                Movie = movie,
                Rating = rating
            };

            _context.RatingMovies.Add(ratingMovie);
            _context.SaveChanges();

            return ratingMovie;
        }

        public double AverageRatingMovie(int movieId)
        {
            return _context.RatingMovies
                .Where(rm => rm.Movie.Id == movieId)
                .Average(rm => rm.Rating);
        }


    }

}
