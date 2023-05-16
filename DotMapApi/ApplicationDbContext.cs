using Microsoft.EntityFrameworkCore;
using DotMapApi.Models;

namespace DotMapApi;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Place> Places { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
}