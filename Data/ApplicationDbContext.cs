using iSarv.Data.Tests;
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

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.TestPackages)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        builder.Entity<TestPackage>()
            .HasOne(tp => tp.CliftonTest)
            .WithOne(ct => ct.TestPackage)
            .HasForeignKey<CliftonTest>();

        builder.Entity<TestPackage>()
            .HasOne(tp => tp.HollandTest)
            .WithOne(ht => ht.TestPackage)
            .HasForeignKey<HollandTest>();

        builder.Entity<TestPackage>()
            .HasOne(tp => tp.RavenTest)
            .WithOne(rt => rt.TestPackage)
            .HasForeignKey<RavenTest>();

        builder.Entity<TestPackage>()
            .HasOne(tp => tp.NeoTest)
            .WithOne(nt => nt.TestPackage)
            .HasForeignKey<NeoTest>();
        
        base.OnModelCreating(builder);
    }

    public DbSet<TestPackage> TestPackages { get; set; }
    public DbSet<NeoTest> NeoTests { get; set; }
    public DbSet<NeoTestQuestion> NeoTestQuestions { get; set; }
    public DbSet<CliftonTest> CliftonTests { get; set; }
    public DbSet<CliftonTestQuestion> CliftonTestQuestions { get; set; }
    public DbSet<HollandTest> HollandTests { get; set; }
    public DbSet<HollandTestQuestion> HollandTestQuestions { get; set; }
    public DbSet<RavenTest> RavenTests { get; set; }
    public DbSet<ActivationCode> ActivationCodes { get; set; }
}
