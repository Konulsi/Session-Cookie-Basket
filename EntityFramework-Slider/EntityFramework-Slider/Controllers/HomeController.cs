using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //HttpContext.Session.SetString("name", "Pervin");   // session storageye datani qoymaq

            //Response.Cookies.Append("surname", "Rehimli", new CookieOptions { MaxAge = TimeSpan.FromMinutes(30)});  // cookie storageye datani qoymaq


            //Book book = new Book
            //{
            //    Id = 1,
            //    Name = "Xoshrov ve Shirin"
            //};

            //objecti cookie ye qoymaq uchun Json a chevirib(yeni string formatina chevirib) qoya bilerik. bunu uchun convert serialize edirik.
            //Response.Cookies.Append("book",JsonConvert.SerializeObject(book));



            //bu yazdiqlarimiz linkue queryler adlanir.bu kodlar vasitesile lazim  olan iwleri gorursen
            List<Slider> sliders = await _context.Sliders.ToListAsync();

            //IQueryable<Slider> slide = _context.Sliders.AsQueryable();
            //List<Slider> query = slide.Where(m=> m.Id > 5).ToList();
            //Iqueryable ile query yaradir ramda saxlayiriq, hele data getirmirik,
            //sonra werte uygun datani getiririk.
            //ToList() yazdiqda request gedir data bazaya. yazmadiqda getmir.

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync();

            //burada where deyib qeyd edirikki haradaki softdelete true olmayanlari goster. veye softdelete == beraberdirse false goster
            IEnumerable<Blog> blogs = await _context.Blogs.Where(m=>m.SoftDelete == false).ToListAsync();

            //Listin ferqli metodlari var.Add,Remove ve s. Listi controllerde hazirlayob view a gonderirik.
            //Listin ichinde olan elave metodlarda iwleyir.
            //IEnumerable da ise hech bir metod yoxdur, ancaq dovre sala bilirsen.View da datani Inumerable kimi qebul etmek daha yaxwidir.
            //Viewda datani sadece foreach edib gosteririk deye, Inumerable iwletmek yaxwidir.LIstin elave metodlari iwleyib sistemi agirlawdirmasin deye.
            IEnumerable<Category> categories = await _context.Categories.Where(m =>m.SoftDelete == false).ToListAsync();


            //burada wekil null gelir Ona gore firstOrDefault ede bilmir.Ona burada Whereden evvel include edirik.
            //Burada relationlu tablelerin icherisinde, ichinde olan o biri tablenide (meselen productun ichinde yazdigimiz  productImage)yeni, productImageni productnan birlikde getirmek isteyirikse,
            //Products.Include edirik
            IEnumerable<Product> products = await _context.Products.Include(m=>m.Images).Where(m => !m.SoftDelete).ToListAsync();

            About abouts = await _context.Abouts.Include(m => m.Adventages).FirstOrDefaultAsync();

            IEnumerable<Experts>  experts = await _context.Experts.Where(m=>m.SoftDelete == false).ToListAsync();

            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FirstOrDefaultAsync();

            Subscribe subscribe = await _context.Subscribes.FirstOrDefaultAsync();

            OurBlog ourBlog = await _context.OurBlogs.FirstOrDefaultAsync();

            IEnumerable<Say> says = await _context.Says.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Instagram> instagrams = await _context.Instagrams.Where(m => m.SoftDelete == false).ToListAsync();





            HomeVM model = new()
            {
                Sliders = sliders,
                SliderInfo = sliderInfo,
                Blogs = blogs,
                Categories = categories,
                Products = products,
                Abouts = abouts,
                Experts = experts,
                ExpertsHeader = expertsHeader,
                Subscribe = subscribe,
                OurBlog = ourBlog,
                Says = says,
                Instagrams = instagrams,
            };

            return View(model);
        }


        //public IActionResult Test()   //storageden datani goturub UI da gostermek uchun metod
        //{
        //    var sessionData = HttpContext.Session.GetString("name"); // session storageden datani gotumek uchun

        //    var cookieData = Request.Cookies["surname"];    // cookie storageden datani goturmek

        //    var objectData = JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);   //object formatinda datani cookie den goturmek. 
        //    //bunu uchun desrialize edirik(yeni json(string) formatini objecte cheviririk ).


        //    return Json(sessionData);
        //}


        [HttpPost] //yeni data elave edende yoxlayir ki token heqiqeten saytdan girilende yaranib eks halda metoda girmeyecek
        [ValidateAntiForgeryToken]  // hemin tokeni yoxlayan atribut budur.
        public async Task<IActionResult> AddBasket(int? id)
        {
            if(id == null) return BadRequest(); 

            Product dbproduct = await _context.Products.FindAsync(id);   //data bazadan productlarimizin ichinden id- sine gore product goturmek

            if (dbproduct == null) return NotFound();


            List<BasketVM> basket;  //bosh bir list yaradiriq

            if (Request.Cookies["basket"] != null) //cookie deki basket null deyilse
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);    // eger cookide data varsa elimizde olan datani beraber edirik bize gelen teze dataya. bunun uchun ge
                //bunu Listimize deserialize edirikki tipleri beraber olsun
                //eyer coockide data varsa yani null deyilse coockide olan datani goturub = DeserializeObject<List<BasketVM>>.esayn edirik elmizde olan List<BasketVM>e
            }
            else
            {
                basket = new List<BasketVM>();
                //data yoxdursa teze liist yaradir
            }


            BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbproduct.Id);
            //yoxlayiriqki basketin ichinde gelen id var ya yox. varsa basketimiz exist olur. yoxdursa null olur


            if (existProduct == null) //baskextde existProduct nulldirsa(yeni elave etdiyimiz productdan basketde yoxdursa yenisini elave edir. varsa else de sayini artirir)
            {
                basket?.Add(new BasketVM   //listimize yeni data elave edirik
                {
                    Id = dbproduct.Id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }

           //yuxaridakilari odedikden sonra add edir cookie ye

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            //elimizde olan listimizi response kimi append edirik cookieye


            return RedirectToAction(nameof(Index)); // bawqa actiona yonlendirmek uchun(sehife refrsh olacaq amma)
        }

    }


    //class Book   // object weklinde datani cookie qoymaq uchun class yaratdiq
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }

    //}
}