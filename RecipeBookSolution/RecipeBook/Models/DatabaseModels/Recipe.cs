using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBook.Models.DatabaseModels
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string Ingredients { get; set; }
        public DateTime? CreatedAt { get; set; }


        [ForeignKey("User")]
        public int? UserId { get; set; }

        [ValidateNever]
        public User User { get; set; }


        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; }


    }
}
