using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaSite.DAL;
using ProniaSite.Models;
using ProniaSite.ViewModels;
using ProniaSite.ViewModels.Basket;

namespace ProniaSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM home = new HomeVM
            {
                IndexMainSlides = _context.IndexMainSlides,
                Shippings=_context.Shippings,
                Banners=_context.Banners,
                Brands=_context.Brands,
                ClientSlides=_context.ClientSlides,
                Products= _context.Products.Where(p=>p.IsDeleted==false).Include(p=>p.ProductImages),
            };
            return View(home);
        }
        public IActionResult SingleProduct()
        {
            return View();
        }
        public IActionResult Shop()
        {
            List<Category> Categories = new List<Category>();
           
            
                Categories = _context.Categories.ToList();
            
            List<Color> Colors = new List<Color>();
            
                Colors = _context.Colors.ToList();
            
            ViewBag.Colors=Colors;
            return View(Categories);
        }
        public IActionResult Card()
        {
            return View();
        }
        public IActionResult LoginRegister()
        {
            return View();
        }
        public IActionResult SetSession(string key,string value)
        {
            HttpContext.Session.SetString(key, value);
            return Content("Ok");
        }
        public IActionResult GetSession(string key)
        {
            string value = HttpContext.Session.GetString(key);
            return Content(value);
        }
        public IActionResult SetCookie (string key,string value)
        {
            HttpContext.Response.Cookies.Append(key, value);
            return Content("ok");
        }
        public IActionResult GetCookie (string key)
        {
            return Content( HttpContext.Request.Cookies[key]);
        }
        public IActionResult AddBasket(int? id)
        {
            List<BasketItemVM> items = new List<BasketItemVM>();
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["basket"]))
            {
                items = JsonConvert.DeserializeObject<List<BasketItemVM>>(HttpContext.Request.Cookies["basket"]);
            }
            BasketItemVM item= items.FirstOrDefault(s=>s.Id==id);
            if (item==null)
            {
                item = new BasketItemVM()
                {
                    Id=(int)id,
                    Count=1
                };
                items.Add(item);
            }
            else
            {
                item.Count++ ;
            }
            string basket=JsonConvert.SerializeObject(items);
            HttpContext.Response.Cookies.Append("basket", basket, new CookieOptions
            {
                MaxAge= TimeSpan.FromDays(5)
            });
            return RedirectToAction(nameof(Index));
        }
    }
}
