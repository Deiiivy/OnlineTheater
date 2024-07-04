using Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logic.DAL;

public class Model : DbContext
{
    public Model(DbContextOptions<Model> options) : base(options)
    {
        
    }
    
    public DbSet<Customer> Customers { get; set; } 
    public DbSet<Movie> Movies { get; set; } 
    public DbSet<PurchasedMovie> PurchasedMovies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(c =>
        {
            c.HasIndex(i => i.Name).IsUnique();
        });
        
        modelBuilder.Entity<Movie>(c =>
        {
            c.HasIndex(i => i.Name).IsUnique();
        });
    } 
}