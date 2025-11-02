namespace RecipeBook.Models
{
    using global::RecipeBook.Models.DatabaseModels;
    using Microsoft.EntityFrameworkCore;

    namespace RecipeBook.Models
    {
        public class DatabaseContext : DbContext
        {
            public DatabaseContext(DbContextOptions<DatabaseContext> options)
                : base(options)
            {
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Recipe> Recipes { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Favorite> Favorites { get; set; }
            public DbSet<Rating> Ratings { get; set; }

            /* protected override void OnModelCreating(ModelBuilder modelBuilder)
             {
                 modelBuilder.Entity<Favorite>()
                     .HasKey(f => new { f.UserId, f.RecipeId });

                 modelBuilder.Entity<Favorite>()
                     .HasOne(f => f.User)
                     .WithMany(u => u.Favorites)
                     .HasForeignKey(f => f.UserId);

                 modelBuilder.Entity<Favorite>()
                     .HasOne(f => f.Recipe)
                     .WithMany(r => r.Favorites)
                     .HasForeignKey(f => f.RecipeId);

                 modelBuilder.Entity<Rating>()
                     .HasOne(r => r.User)
                     .WithMany(u => u.Ratings)
                     .HasForeignKey(r => r.UserId);

                 modelBuilder.Entity<Rating>()
                     .HasOne(r => r.Recipe)
                     .WithMany(rec => rec.Ratings)
                     .HasForeignKey(r => r.RecipeId);

                 modelBuilder.Entity<Recipe>()
                     .HasOne(r => r.Category)
                     .WithMany(c => c.Recipes)
                     .HasForeignKey(r => r.CategoryId);

                 modelBuilder.Entity<Recipe>()
                     .HasOne(r => r.Author)
                     .WithMany(u => u.Recipes)
                     .HasForeignKey(r => r.AuthorId);

                 modelBuilder.Entity<Category>()
                     .HasOne(c => c.ParentCategory)
                     .WithMany(c => c.SubCategories)
                     .HasForeignKey(c => c.ParentCategoryId)
                     .IsRequired(false);  */
        }
    }
}




