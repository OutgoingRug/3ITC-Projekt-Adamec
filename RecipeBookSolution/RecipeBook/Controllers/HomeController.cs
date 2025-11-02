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
            var recipes = _dbContext.Recipes
                                    .Where(zaznam => zaznam.Title.Contains("buchty"))
                                    .OrderBy(zaznam => zaznam.CreatedAt)
                                    .ToList();

            return View(recipes);
        }

        public IActionResult Recipes(string recipeName)
        {
            var recipes = _dbContext.Recipes
                                    .Where(zaznam => zaznam.Title.Contains(recipeName))
                                    .OrderBy(zaznam => zaznam.CreatedAt)
                                    .ToList();

            return View(recipes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
