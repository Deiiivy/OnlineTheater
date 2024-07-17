namespace Logic.Entities;

public class Movie : Entity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public LicensingModel LicensingModel { get; set; }
    public Category Category { get; set; }
    public Audience Audience { get; set; }
}