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
                modelBuilder.Entity<User>(entity =>
                {
                    entity.ToTable("User");
                    entity.HasKey(u => u.Id);
                    
                });

                modelBuilder.Entity<Recipe>(entity =>
                {
                    entity.ToTable("Recipe");
                    entity.HasKey(u => u.Id);

                    entity.HasOne("Category");
                });

                modelBuilder.Entity<Category>(entity =>
                {
                    entity.ToTable("Category");
                    entity.HasKey(u => u.Id);
                });

                modelBuilder.Entity<Favorite>(entity =>
                {
                    entity.ToTable("Favorite");
                    entity.HasKey(u => u.Id);
                });

                modelBuilder.Entity<Rating>(entity =>
                {
                    entity.ToTable("Rating");
                    entity.HasKey(u => u.Id);
                });

                base.OnModelCreating(modelBuilder);
            }
        }
    }

}



