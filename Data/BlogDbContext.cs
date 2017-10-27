using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog.Data
{
    public class BlogDbContext: DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
        {

        }

        public DbSet<Blog.Models.Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Image> Images { get; set; }                

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Url);
            modelBuilder.Entity<Blog.Models.Blog>()
                .HasIndex(b => b.Url);
            modelBuilder.Entity<Image>()
                .HasIndex(i => i.Url)
                .IsUnique();
        }
    }
}