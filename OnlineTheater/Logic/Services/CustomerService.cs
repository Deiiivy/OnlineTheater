using Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class CustomerService
    {
        private readonly MovieService _movieService;

        public CustomerService(MovieService movieService)
        {
            _movieService = movieService;
        }

        public decimal CalculatePrice(CustomerStatus status, DateTime? statusExpirationDate, LicensingModel licensingModel)
        {
            decimal price;
            switch (licensingModel)
            {
                case LicensingModel.TwoDays:
                    price = 4;
                    break;

                case LicensingModel.LifeLong:
                    price = 8;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (status == CustomerStatus.Advanced && (statusExpirationDate == null || statusExpirationDate.Value >= DateTime.UtcNow))
            {
                price = price * 0.75m;
            }

            return price;
        }


        public PurchasedMovie PurchaseMovie(Customer customer, Movie movie)
        {
            DateTime? expirationDate = _movieService.GetExpirationDate(movie.LicensingModel);
            decimal price = CalculatePrice(customer.Status, customer.StatusExpirationDate, movie.LicensingModel);

            var purchasedMovie = new PurchasedMovie
            {
                Movie = movie,
                Customer = customer,
                ExpirationDate = expirationDate,
                Price = price
            };

            return purchasedMovie;
        }

        public bool PromoteCustomer(Customer customer, List<PurchasedMovie> purchasedMovies)
        {
            if (purchasedMovies.Count(x => x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow.AddDays(-30)) < 2)
                return false;

            if (purchasedMovies.Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1)).Sum(x => x.Price) < 100m)
                return false;

            return true;
        }
    }
}
