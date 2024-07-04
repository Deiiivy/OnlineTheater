namespace Logic.Entities;

public class PurchasedMovie : Entity
{
    public Movie Movie { get; set; }
    public Customer Customer { get; set; }
    public decimal Price { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}