namespace RecipeBook.Models.DatabaseModels
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public DateTime CreatedAt { get; set; }



        //public ICollection<User> Users { get; set; }
        //public ICollection<Recipe> Recipes { get; set; }
    }
}
