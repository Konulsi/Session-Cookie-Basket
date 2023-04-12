using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        public SliderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();
            return View(sliders);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();
            //badrequest routingde falan nese sehvlik olduqda chixan exseptiondur
            //meselen routingden sehven axtariw edilen id silinerse bu zaman chixan errordur



            //gelen id li elementi bazadan tapmaq uchun edirik bunu
            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);


            //eyer kimse sehv regem yazibsa Url-e
            //NotFound-tapilmadi deye Exception 
            if (slider is null) return NotFound();
            //gelen id bazamizda yoxdursa chican error

            return View(slider);
        }


        //[HttpGet]-datani goturende 

        [HttpGet]

        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/
        {

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id);

            if (slider is null) return NotFound();

            return View(slider);

        }
    }
}
