using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2023_Assignment4.Models;

namespace Fall2023_Assignment4.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Fall2023_Assignment4.Models.Restaurant> Restaurant { get; set; } = default!;
}

