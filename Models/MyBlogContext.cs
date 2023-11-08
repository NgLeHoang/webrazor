using Microsoft.EntityFrameworkCore;

namespace webrazorapp.models 
{
    public class MyBlogContext : DbContext {

        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options) {
            //....
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Article> articles {get; set;}
    }
}