namespace OnlineTheater.Models
{
    public class MovieDTO
    {     
        
            public required string Name { get; set; }
            public required string Description { get; set; }
            public required string Rating { get; set; }
            public required string Category { get; set; }
            public bool IsActive { get; set; }
            public decimal Price { get; set; }
            public string LicensingModel { get; set; }
        }

}
