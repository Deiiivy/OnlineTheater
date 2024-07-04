namespace Logic.Entities;

public class Movie : Entity
{
    public required string Name { get; set; }
    public LicensingModel LicensingModel { get; set; }
}