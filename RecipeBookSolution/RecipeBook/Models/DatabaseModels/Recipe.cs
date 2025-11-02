namespace RecipeBook.Models.DatabaseModels
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string Ingredients { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }


        //public ICollection<Category> Categories { get; set; }
        //public ICollection<Favorite> Favorites { get; set; }
        //public ICollection<Rating> Ratings { get; set; }
        //public ICollection<User> Users { get; set; }





        //public User User { get; set; }
    }
}
