using Microsoft.EntityFrameworkCore;
using DotMapApi.Models;

namespace DotMapApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Review> Reviews { get; set; }

    // "DefaultConnection": "Server=localhost;Port=5432;Database=DotMap;User Id=postgres;Password=1234;"
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
                     "Host=localhost;" +
                     "Port=5432;" +
                      "Database=DotMap;" +
                      "Username=postgres;" +
                      "Password=1234");
    }
}