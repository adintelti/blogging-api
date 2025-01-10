using Microsoft.EntityFrameworkCore;

namespace BloggingAPI
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .Property(p => p.Tags)
                .HasConversion(
                    tags => string.Join(',', tags),
                    tags => tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        }
    }
}
