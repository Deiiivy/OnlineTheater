namespace Logic.Entities;

public class Movie : Entity
{
    public required string Name { get; set; }
    public string Description { get; set; }
    public string Rating { get; set; } 
    public string Category { get; set; } 
    public bool IsActive { get; set; } = true;
    public LicensingModel LicensingModel { get; set; }
}