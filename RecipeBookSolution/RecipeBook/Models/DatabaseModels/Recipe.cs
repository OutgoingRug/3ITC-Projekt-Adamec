namespace RecipeBook.Models.DatabaseModels
{
    public class Recipe
    {
        public int Id { get; set; }
        public int Title { get; set; }
        public int Description { get; set; }
        public int Instructions { get; set; }
        public int Ingredients { get; set; }
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
