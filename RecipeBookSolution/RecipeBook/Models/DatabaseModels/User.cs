using RecipeBook.Models.RecipeBook.Models;

namespace RecipeBook.Models.DatabaseModels
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }


        public ICollection<Recipe> Recipes { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<Rating> Ratings { get; set; }
    }
}
