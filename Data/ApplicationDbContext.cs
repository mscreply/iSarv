using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace iSarv.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.TestPackages)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        builder.Entity<TestPackage>()
            .HasMany(p => p.Tests);

        base.OnModelCreating(builder);
    }

    public DbSet<TestPackage> TestPackages { get; set; }
    public DbSet<CliftonTest> CliftonTests { get; set; }
    public DbSet<RavensTest> RavensTests { get; set; }
    public DbSet<HollandsTest> HollandsTests { get; set; }
    public DbSet<NeoTest> NeoTests { get; set; }
}
