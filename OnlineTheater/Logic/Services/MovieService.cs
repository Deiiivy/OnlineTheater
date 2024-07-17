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

      

        public double AverageRatingMovie(int movieId)
        {
            return _context.RatingMovies
                .Where(rm => rm.Movie.Id == movieId)
                .Average(rm => rm.Rating);
        }


    }

}
