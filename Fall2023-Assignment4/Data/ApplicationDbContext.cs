using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Fall2023_Assignment4.Models;
using Fall2023_Assignment4.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Data.Common;

namespace Fall2023_Assignment4.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Fall2023_Assignment4.Models.Restaurant> Restaurant { get; set; } = default!;
    public DbSet<Fall2023_Assignment4.Models.Review> Review { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)

    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }
}



public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>

{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
    }
}
