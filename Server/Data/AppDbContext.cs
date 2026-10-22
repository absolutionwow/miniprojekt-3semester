using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Miniprojekt.Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(e =>
        {
            e.Property(p => p.Title).HasMaxLength(200).IsRequired();
            e.Property(p => p.AuthorName).HasMaxLength(50).IsRequired();
            e.Property(p => p.Text).HasMaxLength(10000);
            e.Property(p => p.Url).HasMaxLength(2000);
            e.HasIndex(p => p.CreatedAt); // front page sorts on this

            e.HasMany(p => p.Comments)
             .WithOne(c => c.Post)
             .HasForeignKey(c => c.PostId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.Property(c => c.Text).HasMaxLength(5000).IsRequired();
            e.Property(c => c.AuthorName).HasMaxLength(50).IsRequired();
        });
    }

    // SQLite has no real date type, so values come back with Kind = Unspecified.
    // This makes every DateTime we read be treated as UTC (and serialised with a "Z").
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }

    private class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}
