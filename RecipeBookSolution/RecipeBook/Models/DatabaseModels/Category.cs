namespace RecipeBook.Models.DatabaseModels
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }


        public ICollection<Category> SubCategories { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
    }
}
