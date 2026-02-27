using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RecipeBook.Models;
using RecipeBook.Models.DatabaseModels;
using RecipeBook.Models.RecipeBook.Models;
using RecipeBook.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Globalization;

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
                                    .ToList()
                                    .Select(c => new 
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                        RecipeCount = _dbContext.Recipes.Count(r => r.CategoryId == c.Id)
                                    })
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

            // Set UserId: use logged-in user or default "noname" user
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    recipe.UserId = userId;
                }
            }
            else
            {
                // Get or create "noname" anonymous user
                var anonymousUser = _dbContext.Users.FirstOrDefault(u => u.Email == "noname");
                if (anonymousUser == null)
                {
                    anonymousUser = new User
                    {
                        Email = "noname",
                        PasswordHash = "", // Empty since this is not a real login account
                        CreatedAt = DateTime.UtcNow
                    };
                    _dbContext.Users.Add(anonymousUser);
                    _dbContext.SaveChanges();
                }
                recipe.UserId = anonymousUser.Id;
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                var msg = "Vyplňte email i heslo.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model?.Email });
                }

                ViewBag.LoginError = msg;
                ViewData["Email"] = model?.Email;
                // indicate to view that server set errors so modal should show
                TempData["ShowServerErrors"] = "true";
                TempData["ServerErrors"] = JsonSerializer.Serialize(new[] { msg });
                return View("Account");
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                var msg = "Uživatel s tímto emailem neexistuje.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model.Email });
                }

                ViewBag.LoginError = msg;
                ViewData["Email"] = model.Email;
                TempData["ShowServerErrors"] = "true";
                TempData["ServerErrors"] = JsonSerializer.Serialize(new[] { msg });
                return View("Account");
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (result != PasswordVerificationResult.Success)
            {
                var msg = "Neplatné heslo.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model.Email });
                }

                ViewBag.LoginError = msg;
                ViewData["Email"] = model.Email;
                TempData["ShowServerErrors"] = "true";
                TempData["ServerErrors"] = JsonSerializer.Serialize(new[] { msg });
                return View("Account");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Ok(new { success = true, redirect = Url.Action("Index") });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                var msg = "Vyplňte email a heslo.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model?.Email });
                }

                ViewBag.RegisterError = msg;
                ViewData["Email"] = model?.Email;
                TempData["ShowServerErrors"] = "true";
                return View("Account");
            }

            if (model.Password != model.ConfirmPassword)
            {
                var msg = "Hesla se neshodují.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model.Email });
                }

                ViewBag.RegisterError = msg;
                ViewData["Email"] = model.Email;
                ViewData["ConfirmPassword"] = model.ConfirmPassword;
                TempData["ShowServerErrors"] = "true";
                return View("Account");
            }

            // validate email format and that the domain resolves (best-effort)
            try
            {
                var addr = new MailAddress(model.Email);
                var domain = addr.Host;
                try
                {
                    var entry = Dns.GetHostEntry(domain);
                    if (entry == null || entry.AddressList == null || entry.AddressList.Length == 0)
                    {
                        throw new Exception("Domain not resolvable");
                    }
                }
                catch
                {
                    var msg = "Zadaná e-mailová doména neexistuje nebo nelze ověřit.";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return BadRequest(new { success = false, errors = new[] { msg }, email = model.Email });
                    }

                    ViewBag.RegisterError = msg;
                    ViewData["Email"] = model.Email;
                    TempData["ShowServerErrors"] = "true";
                    TempData["ServerErrors"] = JsonSerializer.Serialize(new[] { msg });
                    return View("Account");
                }
            }
            catch
            {
                var msg = "Zadejte platný email.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model.Email });
                }

                ViewBag.RegisterError = msg;
                ViewData["Email"] = model.Email;
                TempData["ShowServerErrors"] = "true";
                TempData["ServerErrors"] = JsonSerializer.Serialize(new[] { msg });
                return View("Account");
            }

            var exists = _dbContext.Users.Any(u => u.Email == model.Email);
            if (exists)
            {
                var msg = "Uživatel s tímto emailem již existuje.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { success = false, errors = new[] { msg }, email = model.Email });
                }

                ViewBag.RegisterError = msg;
                ViewData["Email"] = model.Email;
                TempData["ShowServerErrors"] = "true";
                return View("Account");
            }

            var user = new User
            {
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, model.Password);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // sign in
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Ok(new { success = true, redirect = Url.Action("Index") });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        public IActionResult RecipeDetail(int id)
        {
            var recipe = _dbContext.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Where(r => r.Id == id)
                .FirstOrDefault();

            return View(recipe);
        }

        public IActionResult Search(string query, string type = "recipes")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<Recipe>());
            }

            var searchTerm = RemoveDiacritics(query).ToLower();
            List<Recipe> recipes = new List<Recipe>();

            if (type == "recipes")
            {
                recipes = _dbContext.Recipes
                    .Include(x => x.Category)
                    .ToList()
                    .Where(r => RemoveDiacritics(r.Title).ToLower().Contains(searchTerm) || 
                                RemoveDiacritics(r.Category.Name).ToLower().Contains(searchTerm))
                    .OrderBy(r => r.Title)
                    .ToList();
            }
            else if (type == "ingredients")
            {
                recipes = _dbContext.Recipes
                    .Include(x => x.Category)
                    .ToList()
                    .Where(r => RemoveDiacritics(r.Ingredients ?? "").ToLower().Contains(searchTerm))
                    .OrderBy(r => r.Title)
                    .ToList();
            }

            ViewData["SearchQuery"] = query;
            ViewData["SearchType"] = type;
            return View(recipes);
        }

        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        [HttpGet]
        public IActionResult SearchSuggestions(string query, string type = "recipes")
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Json(new List<object>());
            }

            var searchTerm = RemoveDiacritics(query).ToLower();
            List<dynamic> results = new List<dynamic>();

            if (type == "recipes")
            {
                var recipes = _dbContext.Recipes
                    .Include(x => x.Category)
                    .ToList()
                    .Where(r => RemoveDiacritics(r.Title).ToLower().Contains(searchTerm) || 
                                RemoveDiacritics(r.Category.Name).ToLower().Contains(searchTerm))
                    .Take(6)
                    .Select(r => new { id = r.Id, title = r.Title, type = "recipe", icon = "📖" });
                results.AddRange(recipes);
            }
            else if (type == "ingredients")
            {
                var recipes = _dbContext.Recipes
                    .ToList()
                    .Where(r => RemoveDiacritics(r.Ingredients ?? "").ToLower().Contains(searchTerm))
                    .Take(6)
                    .Select(r => new { id = r.Id, title = r.Title, type = "ingredient", icon = "🛒" });
                results.AddRange(recipes);
            }

            return Json(results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
