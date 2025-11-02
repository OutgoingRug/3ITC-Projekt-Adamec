namespace RecipeBook.Models.DatabaseModels
{
    public class Favorite
    {
        public int user_id { get; set; }
        public int recipe_id { get; set; }
        public DateTime CreatedAt { get; set; }



        public ICollection<User> Users { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
    }
}
