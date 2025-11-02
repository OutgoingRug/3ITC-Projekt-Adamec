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


            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>()
                    .ToTable("User")
                    .HasKey(u => u.Id);

                modelBuilder.Entity<Recipe>()
                    .ToTable("Recipe")
                    .HasKey(u => u.Id);

                modelBuilder.Entity<Category>()
                    .ToTable("Category")
                    .HasKey(u => u.Id);

                modelBuilder.Entity<Favorite>()
                    .ToTable("Favorite")
                    .HasKey(u => u.Id);

                modelBuilder.Entity<Rating>()
                    .ToTable("Rating")
                 .HasKey(u => u.Id);

                base.OnModelCreating(modelBuilder);
            }
        }
    }

}



