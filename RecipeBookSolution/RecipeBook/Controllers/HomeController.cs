using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Models;
using RecipeBook.Models.RecipeBook.Models;
using System.Diagnostics;

namespace RecipeBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _dbContext;

        public HomeController(ILogger<HomeController> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Categories()
        {
            var categories = _dbContext.Categories
                                    .Where(zaznam => zaznam.ParentCategoryId == null)
                                    .OrderBy(zaznam => zaznam.Name)
                                    .ToList();

            return View(categories);
        }

        public IActionResult RecipeList(int id)    //id je kategorie
        {
            var recipes = _dbContext.Recipes.Include(x => x.Category)
                                    .Where(zaznam => zaznam.CategoryId == id)
                                    .OrderBy(zaznam => zaznam.Title)
                                    .ToList();

            return View(recipes);
        }

        public IActionResult CreateRecipe()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Bookmarks()
        {
            return View();
        }
        public IActionResult Account()
        {
            return View();
        }
        public IActionResult SignOut()
        {
            return View();
        }

        public IActionResult RecipeDetail(int id)
        {
            var recipe = _dbContext.Recipes.Where(r => r.Id == id).FirstOrDefault();

            return View(recipe);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
