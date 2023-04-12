using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Controllers
{

    public class CardController : Controller
    {
        private readonly AppDbContext _context;
        public CardController(AppDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {

            List<BasketVM> basket;



            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            foreach (var basketVM in basket)  //elimizdeki basket list weklimndedir deye fora saliriq
            {
                Product dbproduct = _context.Products.Include(m=>m.Images).FirstOrDefault(m=>m.Id == basketVM.Id); // yoxlayiriq databazadaki productun idisinnen cookiedeki productun idsi eynidirse
                basketVM.Product = dbproduct;   //basketvm deki productdumuzu beraber edirik yoxladigimiz werte. yeni databazadki productnan cookiedeki eyni olan producta
            }

            //listimizi gonderirik view a
            return View(basket);
        }
    }
}
