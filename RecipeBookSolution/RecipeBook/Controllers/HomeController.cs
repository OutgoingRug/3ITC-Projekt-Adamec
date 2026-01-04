using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RecipeBook.Models;
using RecipeBook.Models.RecipeBook.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RecipeBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, DatabaseContext dbContext, IWebHostEnvironment env)
        {
            _logger = logger;
            _dbContext = dbContext;
            _env = env;
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

        private SelectList GeMainCategoriesSelectList()
        {
            var seznamKategorií = _dbContext.Categories
                                        .Where(c => c.ParentCategoryId == null)
                                        .OrderBy(c => c.Name)
                                        .ToList();
            return new SelectList(seznamKategorií, "Id", "Name");
        }

        public IActionResult CreateRecipe()
        {
            ViewBag.CategoryList = GeMainCategoriesSelectList();
            return View();
        }


        [HttpPost]
        public IActionResult CreateRecipe(Models.DatabaseModels.Recipe recipe, IFormFile? image)
        {
            // ulozit do db pomoci entity frameworku
            if (recipe is null)
            {
                return BadRequest();
            }

            ViewBag.CategoryList = GeMainCategoriesSelectList();

            if (image != null && image.Length > 0)
            {
                var permittedExtensions = new[] { ".jpg", ".jpeg" };
                var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Image", "Nepodporovaný formát obrázku. Povolené formáty: .jpg, .jpeg");
                }
            }


            if (!ModelState.IsValid)
            {
                return View(recipe);
            }

            // duplikat?
            var duplicateExists = _dbContext.Recipes
                                        .Where(r => r.CategoryId == recipe.CategoryId && r.Title == recipe.Title)
                                        .Any();

            if (duplicateExists)
            {
                ModelState.AddModelError(nameof(recipe.Title), "Recept se stejným názvem se v dané kategorii už nachází.");
                return View(recipe);
            }

            //nastavime datum vytvoreni
            recipe.CreatedAt = DateTime.Now;

            // pridat a ulozit
            _dbContext.Recipes.Add(recipe);
            _dbContext.SaveChanges();


            // zpracovani nahraneho obrazku (pokud je zaslan)
            if (image != null && image.Length > 0)
            {
                // vytvořit složku, pokud neexistuje
                var imagesFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "recipes");
                Directory.CreateDirectory(imagesFolder);

                // ulozit soubor s jedinecnym nazvem
                var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = $"{recipe.Id}{ext}";
                var filePath = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
            }

            return RedirectToAction(nameof(RecipeDetail), new { id = recipe.Id });
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
